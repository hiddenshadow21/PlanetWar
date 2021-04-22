using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    private HUD hud;
    private float moveSpeed = 10f;
    private float jumpHeight = 5f;
    private Vector2 moveDir;
    
    public float maxHealth = 100;
    [SyncVar(hook =nameof(OnHealthChange))]
    private float health;

    private void OnHealthChange(float _old, float _new)
    {
    }

    [SyncVar(hook = nameof(OnColorChange))]
    public Kolory Kolor;

    [SerializeField]
    private Transform pointBelowPlayer;

    private void OnColorChange(Kolory _old, Kolory _new)
    {
        var sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        switch (_new)
        {
            case Kolory.niebieski:
                sprite.color = Color.blue;
                break;
            case Kolory.zielony:
                sprite.color = Color.green;
                break;
            case Kolory.pomarańczowy:
                sprite.color = new Color(255,165,0);
                break;
            case Kolory.fioletowy:
                sprite.color = Color.magenta;
                break;
            case Kolory.czerwony:
                sprite.color = Color.red;
                break;
            default:
                break;
        }
        var bronie = gameObject.GetComponentsInChildren<Gun>();
        foreach (var gun in bronie)
        {
            gun.SetSprite(_new);
        }
    }

    [SyncVar]
    public string playerName;

    public new Collider2D collider;
    public Rigidbody2D rb;

    public  bool isGrounded { 
        get;
        private set;
    }


    private Vector2 groundNormal;

    public GameObject[] Grounds;

    [SerializeField]
    private LayerMask layerMask;

    private void Start()
    {
        Grounds = GameObject.FindGameObjectsWithTag("Ground");
        if(isLocalPlayer)
            Camera.main.GetComponent<CameraController>().player = gameObject;

        health = maxHealth;
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
        Debug.Log($"--- PlayerController.color: {Kolor} ---");
    }

    private void OnEnable()
    {
        if (isServer)
            health = maxHealth;
    }

    void Update()
    {
        HandleMovement();
    }

    private void FixedUpdate()
    {
        CheckIfOnGround();
        RotatePlayerOnGround();
        RotatePlayerInAir();
    }

    #region Movement&Rotation
    void HandleMovement()
    {
        if (!isLocalPlayer || !isGrounded)
            return;

        float moveHorizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        moveDir = new Vector2(moveHorizontal, 0);
        transform.Translate(moveDir);
        //rb.velocity = Vector2.zero;
        if (Input.GetButtonDown("Jump"))
        {
            float x = 6 * jumpHeight;
            rb.AddForce(transform.up * x, ForceMode2D.Impulse);
            Debug.Log(x);
        }
    }

    void CheckIfOnGround()
    {
        if (!isLocalPlayer)
            return;

        RaycastHit2D hit = Physics2D.Raycast(
            pointBelowPlayer.position,
            -transform.up, 100, layerMask);
        if (hit.collider != null)
        {
            groundNormal = hit.normal;

            if (hit.distance <= 0.1f)
                isGrounded = true;
            else
                isGrounded = false;
        }
    }
    
    void RotatePlayerOnGround()
    {
        if (!isGrounded && !isLocalPlayer)
            return;
        Quaternion toRotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
        transform.rotation = toRotation;
    }

    void RotatePlayerInAir()
    {
        if (isGrounded && !isLocalPlayer)
            return;

        GameObject ground = GetClosestGround();
        Vector3 vectorToTarget = (transform.position - ground.transform.position).normalized;

        Quaternion toRotation = Quaternion.FromToRotation(transform.up, vectorToTarget) * transform.rotation;
        transform.rotation = toRotation;
    }

    GameObject GetClosestGround()
    {
        GameObject closestGround = Grounds[0];
        float closestDistance = Vector2.Distance(transform.position, closestGround.transform.position);
        foreach (var ground in Grounds)
        {
            float distance = Vector2.Distance(transform.position, ground.transform.position);
            if (distance < closestDistance)
            {
                closestGround = ground;
                closestDistance = distance;
            }
        }
        return closestGround;
    }
    #endregion

    [Server]
    public void TakeDamage(float damage)
    {
        health -= damage;
        hud.UpdateHealth((int)health);
        if (health <= 0)
        {
            rb.velocity = Vector2.zero;
            DisableComponents();
            Die();
        }
        Debug.Log(health);
    }

    [Server]
    public void AddHealth(float amount)
    {
        health += amount;
        hud.UpdateHealth((int)health);
        Debug.Log(health);
    }

    public void DisableComponents()
    {
        if (isServer)
        {
            Gun[] guns = gameObject.GetComponentsInChildren<Gun>();
            foreach (var gun in guns)
            {
                NetworkServer.Destroy(gun.gameObject);
            }
        }
        gameObject.GetComponent<PlayerWeaponController>().enabled = false;
        gameObject.GetComponent<GravityBody>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        this.enabled = false;
    }

    public void EnableComponents()
    {
        this.enabled = true;
        gameObject.GetComponent<PlayerWeaponController>().enabled = true;
        gameObject.GetComponent<Collider2D>().enabled = true;
        gameObject.GetComponent<GravityBody>().enabled = true;
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;

    }

    [ClientRpc]
    private void Die()
    {
        DisableComponents();
        if (isLocalPlayer)
        {
            gameObject.GetComponent<PlayerRespawnSystem>().ToogleCanvas();
        }
    }
}
