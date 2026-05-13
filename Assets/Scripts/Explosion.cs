using Unity.Netcode;
using UnityEngine;

public class Explosion : NetworkBehaviour
{
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float duration = 1f;

    [SerializeField] private Bomb bomb;

    private float timer;
    private bool exploding;

    public void BeginExplosion()
    {
        exploding = true;
    }

    void Update()
    {
        if (!exploding) return;

        timer += Time.deltaTime;

        // VISUALS ON ALL CLIENTS
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        float progress = timer / duration;

        float currentRadius =
            Mathf.Lerp(0f, explosionRadius, progress);

        transform.localScale =
            new Vector3(currentRadius, currentRadius, currentRadius);

        // ONLY SERVER DESTROYS
        if (IsServer && progress >= 1f)
        {
            DestroyBomb();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer) return;

        PlayerHealth health =
            collision.GetComponent<PlayerHealth>();

        if (health != null)
        {
            health.TakeDamage(explosionDamage);
        }
    }

    private void DestroyBomb()
    {
        bomb.shouldDespawn = true;
    }
}