using Unity.Netcode;
using UnityEngine;
using System;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    // Event the health bar listens to
    public event Action<float> OnHealthChanged;

    private NetworkVariable<float> currentHealth = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        currentHealth.OnValueChanged += HandleHealthChanged;

        // Set initial bar value on late joiners
        OnHealthChanged?.Invoke(GetHealthPercent());
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= HandleHealthChanged;
    }

    public void TakeDamage(float damage)
    {
        if (!IsServer) return;

        currentHealth.Value -= damage ;

        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(float damage, ulong attackerClientId)
    {
        if (!IsServer) return;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            Die(attackerClientId);
        }
    }

    public void Respawn()
    {
        if (!IsServer) return;
        currentHealth.Value = maxHealth;
        MoveClientRpc(Vector3.zero);
    }

    private void HandleHealthChanged(float previous, float current)
    {
        // Fires on all clients automatically via NetworkVariable
        OnHealthChanged?.Invoke(GetHealthPercent());
    }

    public float GetHealthPercent() => currentHealth.Value / maxHealth;

    private void Die()
    {
        MoveClientRpc(new Vector3(50, 50, 0));
    }
    private void Die(ulong killerClientId)
    {
        NetworkObject killer =
            NetworkManager.Singleton.ConnectedClients[killerClientId].PlayerObject;

        if (killer != null)
        {
            PlayerStats stats = killer.GetComponent<PlayerStats>();

            if (stats != null)
            {
                stats.KillCount.Value++;
            }
        }

        MoveClientRpc(new Vector3(50, 50, 0));
    }

    [ClientRpc]
    private void MoveClientRpc(Vector3 position)
    {
        gameObject.transform.position = position;
    }
}