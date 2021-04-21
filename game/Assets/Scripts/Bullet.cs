using Mirror;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float speed = 1;
    [SerializeField]
    private float damage = 10;
    [SerializeField]
    private float lifeTime = 15;

    private uint shooterId;

    [Server]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer)
            return;

        Debug.Log("Hit: " + other);
        var player = other.gameObject.GetComponent<PlayerController>();
        if(player != null)
        {
            player.TakeDamage(damage);
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