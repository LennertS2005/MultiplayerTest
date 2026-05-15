using Unity.Netcode;
using UnityEngine;

public class BulletNetwork : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 3f;

    private Vector2 direction;

    private ulong clientID;

    public void Initialize(Vector2 shootDirection, ulong shooterClientID)
    {
        direction = shootDirection;
        clientID = shooterClientID;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Invoke(nameof(DestroyBullet), lifetime);
        }
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Obstacle"))
        {
            DestroyBullet();
            return;
        }

        NetworkObject otherNetworkObject = other.GetComponent<NetworkObject>();
        if (otherNetworkObject != null && otherNetworkObject.OwnerClientId == clientID)
        {
            return;
        }

        // Don't hit the shooter's own collider
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage, clientID);
            DestroyBullet();
        }
    }

    private void DestroyBullet()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
}