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

        currentHealth.Value = Mathf.Max(0, currentHealth.Value - damage);

        if (currentHealth.Value <= 0)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    private void HandleHealthChanged(float previous, float current)
    {
        // Fires on all clients automatically via NetworkVariable
        OnHealthChanged?.Invoke(GetHealthPercent());
    }

    public float GetHealthPercent() => currentHealth.Value / maxHealth;
}