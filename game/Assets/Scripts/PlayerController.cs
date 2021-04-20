using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private float moveSpeed = 10f;
    private float jumpHeight = 5f;
    private Vector2 moveDir;
    
    public float maxHealth = 100;
    [SyncVar]
    private float health;

    [SyncVar(hook = nameof(OnColorChange))]
    public Kolory Kolor;

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

    private void Start()
    {
        Grounds = GameObject.FindGameObjectsWithTag("Ground");
        if(isLocalPlayer)
            Camera.main.GetComponent<CameraController>().player = gameObject;

        health = maxHealth;
        Debug.Log($"--- PlayerController.color: {Kolor} ---");
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
        if (!isLocalPlayer)
            return;
        float moveHorizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        moveDir = new Vector2(moveHorizontal, 0);
        transform.Translate(moveDir);

        if (isGrounded && Input.GetButtonDown("Jump"))
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
        isGrounded = false;
        var pointBelowCollider = transform.TransformDirection(
            transform.InverseTransformDirection(transform.position)
            - new Vector3(0, collider.bounds.extents.y, 0)
        );
        RaycastHit2D hit = Physics2D.Raycast(
            pointBelowCollider,
            -transform.up);
        if (hit.collider != null)
        {
            groundNormal = hit.normal;

            if (hit.distance <= 0.1f)
                isGrounded = true;
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
        if (health < 0)
        {
            DestroyPlayer();
        }
        Debug.Log(health);
    }


    [Server]
    public void AddHealth(float amount)
    {
        health += amount;
        Debug.Log(health);
    }

    [Server]
    private void DestroyPlayer()
    {
        NetworkServer.Destroy(gameObject);
    }
}
