using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private HUD hud;
    private CameraController cameraController;
    private float moveSpeed = 10f;
    private float jumpHeight = 5f;
    private Vector2 moveDir;

    [SerializeField]
    private Transform pointBelowPlayer;

    [SerializeField]
    private LayerMask layerMask;
		
    [SerializeField]
    public Animator animator;

    public float maxHealth = 100;
    [SyncVar(hook = nameof(OnHealthChange))]
    public float health;

    public float maxArmor = 200;
    [SyncVar(hook = nameof(OnArmorChange))]
    public float armor;

    [SyncVar(hook = nameof(OnColorChange))]
    public Kolory Kolor;

    [SyncVar(hook = nameof(OnKillsChange))]
    public uint Kills = 0;

    [SyncVar(hook = nameof(OnDeathsChange))]
    public uint Deaths = 0;

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


    private void hud_Chat_messageEntered(object sender, string chatMessage)
    {
        if (!isLocalPlayer)
            return;

        AddChatMessage(playerName, chatMessage);
    }

    #region SyncVar Hooks

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
        if (!isLocalPlayer)
            return;

        if (_old < _new)
            StartCoroutine(hud.HP_showHpIncrementAnim(_new - _old));
        else
        {
            StartCoroutine(cameraController.ShakeCamera(((_old - _new) / 45) * 0.25f, 0.5f));
            hud.Damage_show(_old - _new);
        }

        if (_new < 0)
            hud.HP_update(0);
        else
            hud.HP_update((int)_new);
    }

    public void OnArmorChange(float _old, float _new)
    {
        if (!isLocalPlayer)
            return;

        if (_old < _new)
            StartCoroutine(hud.Armor_showArmorIncrementAnim(_new - _old));
        else
        {
            StartCoroutine(cameraController.ShakeCamera(((_old - _new) / 45) * 0.25f, 0.5f));
            hud.Damage_show(_old - _new);
        }


        if (_new < 0)
            hud.Armor_update(0);
        else
            hud.Armor_update((int)_new);
    }

    public void OnKillsChange(uint _old, uint _new)
    {
        if (!isLocalPlayer)
            return;

        hud.Kills_update(_new);
    }

    public void OnDeathsChange(uint _old, uint _new)
    {
        if (!isLocalPlayer)
            return;

        hud.Deahts_update(_new);        
    }

    #endregion

    private void Start()
    {
        Grounds = GameObject.FindGameObjectsWithTag("Ground");
        if (isLocalPlayer)
            Camera.main.GetComponent<CameraController>().player = gameObject;

        health = maxHealth;
        armor = 0;
        Debug.Log($"--- PlayerController.color: {Kolor} ---");
    }

    private void Awake()
    {
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
        hud.Chat_messageEntered += hud_Chat_messageEntered;
        hud.HP_update((int)maxHealth);
        hud.Armor_update(0);
        hud.Kills_update(Kills);
        hud.Deahts_update(Deaths);
    }

    private void OnEnable()
    {
        if (isServer)
        {
            health = maxHealth;
            armor = 0;
        }
    }

    void Update()
    {
        if(isLocalPlayer)
        {
            if (!hud.Chat_isChatActive)
            {
                HandleMovement();
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                hud.Chat_TabAction();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                hud.Chat_ReturnAction();
            }
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
        if(armor > 0)
        {
            if (armor >= damage)
            {
                armor -= damage;
                damage = 0;
            }
            else
            {
                damage -= armor;
                armor = 0;
            }
        }
        health -= damage;
        if (health <= 0)
        {
            if (shooterId == 10) //poisonArea
            {
                showDeathInfo("poison area", playerName);
            }
            else
            {
                var shooterPlayer = FindObjectsOfType<PlayerController>().Where(x => x.netId == shooterId).FirstOrDefault();
                shooterPlayer.Kills++;
                showDeathInfo(shooterPlayer.playerName, playerName);
                //rb.velocity = Vector2.zero;
            }

            Deaths++;
            DisableComponents();
            Die();
        }
        Debug.Log(String.Format("H: {0}, A: {1}", health, armor));
    }

    #region Bonus - HealthKit
    [Server]
    public void AddHealth(float amount)
    {
        if(maxHealth - amount < health)
        {
            health = 100;
        }
        else
        {
            health += amount;
        }
        Debug.Log(health);
    }
    #endregion

    #region Bonus - ArmorKit
    [Server]
    public void AddArmor(float amount)
    {
        if (maxArmor - amount < armor)
        {
            armor = 100;
        }
        else
        {
            armor += amount;
        }
        Debug.Log(armor);
    }
    #endregion

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

    [Command]
    public void AddChatMessage(string username, string message)
    {
        var serverGameplay = GameObject.FindGameObjectWithTag("ServerGameplay").GetComponent<ServerGameplay>();
        serverGameplay.SendChatMessage(username, message);
    }

    [ClientRpc]
    private void showDeathInfo(string shooter, string killed)
    {
        hud.Death_show(shooter, killed);
    }

    [ClientRpc]
    private void Die()
    {
        DisableComponents();
        if (isLocalPlayer)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            StartCoroutine(hud.HP_showRespawnAnim(5, 4));
            StartCoroutine(SpawnPlayerWithDelay(5));
        }
    }
	
	[TargetRpc]
    public void TargetUpdateHudTimer(NetworkConnection targer, int time)
    {
        hud.Timer_update(time);
    }

    [TargetRpc]
    public void TargetUpdateChat(NetworkConnection target, List<string> chats)
    {
        hud.Chat_update(chats, playerName);
    }

    [TargetRpc]
    public void TargetUpdatePoisonArea(NetworkConnection target, int poisonedAreaNumber, int allAreasNumber)
    {
        hud.PoisonArea_SetValue(poisonedAreaNumber, allAreasNumber);
    }

    private IEnumerator SpawnPlayerWithDelay(float t)
    {
        yield return new WaitForSeconds(t);
        playerRespawnSystem = gameObject.GetComponent<PlayerRespawnSystem>();
        playerRespawnSystem.SpawnPlayerLocal();
    }

    #region Bonus - PoisonAreaShield
    Coroutine hudPoisonAreaShieldCoroutine;
    public bool IsPoisonAreaShieldActive { get; private set; } = false;

    [Server]
    public void StartPoisonAreaShield(float time)
    {
        CancelInvoke(nameof(stopPoisonAreaShield));
        Invoke(nameof(stopPoisonAreaShield), time);
        IsPoisonAreaShieldActive = true;
        showHudPoisonAreaShield(connectionToClient, time);
    }

    [TargetRpc]
    private void showHudPoisonAreaShield(NetworkConnection target, float time)
    {
        if (hudPoisonAreaShieldCoroutine != null)
        {
            StopCoroutine(hudPoisonAreaShieldCoroutine);
        }
        hudPoisonAreaShieldCoroutine = StartCoroutine(hud.Bonus_poisonAreaShield_show(time));
    }

    [Server]
    private void stopPoisonAreaShield()
    {
        IsPoisonAreaShieldActive = false;
    }
    #endregion

    #region Bonus - CarpetBombingShield
    Coroutine hudCarpetBombingShieldCoroutine;
    public bool IsCarpetBombingShieldActive { get; private set; } = false;

    [Server]
    public void StartCarpetBombingShield(float time)
    {
        CancelInvoke(nameof(stopCarpetBombingShield));
        Invoke(nameof(stopCarpetBombingShield), time);
        IsCarpetBombingShieldActive = true;
        showHudCarpetBombingShield(connectionToClient, time);
    }

    [TargetRpc]
    private void showHudCarpetBombingShield(NetworkConnection target, float time)
    {
        if (hudCarpetBombingShieldCoroutine != null)
        {
            StopCoroutine(hudCarpetBombingShieldCoroutine);
        }
        hudCarpetBombingShieldCoroutine = StartCoroutine(hud.Bonus_carpetBombingShield_show(time));
    }

    [Server]
    private void stopCarpetBombingShield()
    {
        IsCarpetBombingShieldActive = false;
    }
    #endregion

    #region Bonus - CarpetBombing
    [ClientRpc]
    public void RpcSendInfoAboutCarpetBombing(string summonerName)
    {
        var cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        StartCoroutine(cameraController.ShakeCamera(0.35f, 1.5f));
        hud.Bonus_carpetBombing_show(summonerName);
    }
    #endregion
}