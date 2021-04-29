using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private HUD hud;
    private float moveSpeed = 10f;
    private float jumpHeight = 5f;
    private Vector2 moveDir;
    private System.Random rand = new System.Random();

    [SerializeField]
    private Transform pointBelowPlayer;

    [SerializeField]
    private LayerMask layerMask;
		
    [SerializeField]
    public Animator animator;

    public float maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChange))]
    private float health;

    [SyncVar(hook = nameof(OnColorChange))]
    public Kolory Kolor;

    [SyncVar(hook = nameof(OnKillsChange))]
    public uint Kills = 0;

    [SyncVar(hook = nameof(OnDeathsChange))]
    public uint Deaths = 0;

    [SyncVar(hook = nameof(OnLastKilledPlayerChange))]
    public string LastKilledPlayer;

    [SyncVar]
    public string playerName;

    public new Collider2D collider;
    public Rigidbody2D rb;
    public PlayerRespawnSystem playerRespawnSystem;

    public bool isGrounded
    {
        get;
        private set;
    }

    private Vector2 groundNormal;

    public GameObject[] Grounds;


    #region Chat
    private int ChatID;

    [SyncVar(hook = nameof(OnChatMessageChanged))]
    string chatMessage;

    void OnChatMessageChanged(string oldFormattedMessage, string newFormattedMessage)
    {
        string[] nicknameAndMessage = newFormattedMessage.Split('~');
        if(nicknameAndMessage[0] == ChatID.ToString())
        {
            hud.Chat_SetNewMessage(nicknameAndMessage[1], nicknameAndMessage[3], true);
        }
        else
        {
            hud.Chat_SetNewMessage(nicknameAndMessage[1], nicknameAndMessage[3]);
        }
    }   

    [Command]
    void SendChatMessage(string username, string message)
    {
        chatMessage = username + '~' + rand.Next().ToString() + '~' + message;
    }

    private void hud_Chat_messageEntered(object sender, string lassChatMessage)
    {
        if (isLocalPlayer)
        {
            SendChatMessage(ChatID.ToString() + '~' + playerName, lassChatMessage);
        }
    }
    #endregion

    #region SyncVar Hooks

    public void OnLastKilledPlayerChange(string _old, string _new)
    {
        hud.DeathGlobal_Show(playerName, _new);
    }

    private void OnColorChange(Kolory _old, Kolory _new)
    {
        var sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        switch (_new)
        {
            case Kolory.niebieski:
                sprite.material.SetColor("_Color", new Color(0, .7f, 1f));
                break;
            case Kolory.zielony:
                sprite.material.SetColor("_Color", new Color(.5f, 1f, .5f));
                break;
            case Kolory.pomarańczowy:
                sprite.material.SetColor("_Color", new Color(1f, .55f, .35f));
                break;
            case Kolory.fioletowy:
                sprite.material.SetColor("_Color", new Color(1f, .5f, 1f));
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

    public void OnHealthChange(float _old, float _new)
    {
        if(isLocalPlayer)
        {
            if (_new < 0)
                hud.HP_update(0);
            else
                hud.HP_update((int)_new);
        }
    }

    public void OnKillsChange(uint _old, uint _new)
    {
        if(isLocalPlayer)
        {
            hud.Kills_update(_new);
        }
    }

    public void OnDeathsChange(uint _old, uint _new)
    {
        if(isLocalPlayer)
        {
            hud.Deahts_update(_new);
        }           
    }

    #endregion

    private void Start()
    {
        ChatID = rand.Next();
        Grounds = GameObject.FindGameObjectsWithTag("Ground");
        if (isLocalPlayer)
            Camera.main.GetComponent<CameraController>().player = gameObject;

        health = maxHealth;
        Debug.Log($"--- PlayerController.color: {Kolor} ---");
    }

    private void Awake()
    {
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
        hud.Chat_messageEntered += hud_Chat_messageEntered;
        hud.HP_update((int)maxHealth);
        hud.Kills_update(Kills);
        hud.Deahts_update(Deaths);
    }

    private void OnEnable()
    {
        if (isServer)
            health = maxHealth;
    }

    void Update()
    {
        if(!hud.Chat_IsChatActive)
        {
            HandleMovement();
        }
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


        CheckIfOnGround();
        if (!isGrounded)
        {
            animator.SetBool("IsJumping", true);
            Debug.Log("powieterze");
        }
        else
            animator.SetBool("IsJumping", false);


        if (moveDir != Vector2.zero)
        {
   
            var sprite = gameObject.GetComponentInChildren<SpriteRenderer>();

            if (Input.GetAxis("Horizontal") < 0)
                sprite.flipX = true;
            else if (Input.GetAxis("Horizontal") > 0)
                sprite.flipX = false;
        }

        animator.SetBool("IsWalking", moveDir != Vector2.zero);
        

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
    public void TakeDamage(float damage, uint shooterId)
    {
        health -= damage;
        if (health <= 0)
        {
            var shooterPlayer = FindObjectsOfType<PlayerController>().Where(x => x.netId == shooterId).FirstOrDefault();
            shooterPlayer.Kills++;
            shooterPlayer.LastKilledPlayer = playerName;
            Deaths++;
            hud.DeathGlobal_Show(shooterPlayer.playerName, playerName);
            //rb.velocity = Vector2.zero;
            DisableComponents();
            Die();
        }
        Debug.Log(health);
    }

    [Server]
    public void AddHealth(float amount)
    {
        health += amount;
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
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            StartCoroutine(hud.HP_ShowRespawnAnim(5, 4));
            StartCoroutine(SpawnPlayerWithDelay(5));
        }
    }
	
	[ClientRpc]
    public void RpcUpdateHudTimer(int time)
    {
        hud.Timer_update(time);
    }

    private IEnumerator SpawnPlayerWithDelay(float t)
    {
        yield return new WaitForSeconds(t);
        playerRespawnSystem = gameObject.GetComponent<PlayerRespawnSystem>();
        playerRespawnSystem.SpawnPlayerLocal();
    }
}