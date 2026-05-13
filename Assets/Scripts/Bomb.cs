using Unity.Netcode;
using UnityEngine;

public class Bomb : NetworkBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private float explosionDelay = 3f;

    private float timer;

    public bool shouldDespawn;

    void Update()
    {
        if (!IsServer) return;

        timer += Time.deltaTime;

        if (timer >= explosionDelay)
        {
            Explode();
        }
    }

    private void Explode()
    {
        ShowExplosionClientRpc();

        Explosion explosionScript =
            explosion.GetComponent<Explosion>();

        explosionScript.BeginExplosion();
    }

    [ClientRpc]
    private void ShowExplosionClientRpc()
    {
        explosion.SetActive(true);
    }
}