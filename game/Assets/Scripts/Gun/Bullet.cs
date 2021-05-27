using Mirror;
using System.Linq;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed = 1;
    [SerializeField]
    private float damage = 10;
    [SerializeField]
    private float lifeTime = 15;
    [SerializeField]
    private bool hitPlanet = true;
    [SerializeField]
    private bool hitBullet = true;

    private uint shooterId;

    [Server]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer)
            return;

        Debug.Log("Hit: " + other);
        var player = other.gameObject.GetComponent<PlayerController>();
        var planet = other.gameObject.GetComponent<GravityAttractor>();
        var bullet = other.gameObject.GetComponent<Bullet>();

        if(planet != null && hitPlanet == false)
            return;

        if(bullet != null && hitBullet == false)
            return;


        if(player != null)
        {
            if (shooterId == player.netId)
                return;
            if (player.IsCarpetBombingShieldActive == true && hitPlanet == false)
                return;
            player.TakeDamage(damage, shooterId);
        }
        NetworkServer.Destroy(gameObject);
    }

    public void SetShooterId(uint Id)
    {
        shooterId = Id;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}