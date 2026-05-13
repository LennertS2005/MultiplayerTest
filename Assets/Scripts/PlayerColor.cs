using Unity.Netcode;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color[] availableColors;

    private NetworkVariable<int> colorIndex = new(
        -1,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        colorIndex.OnValueChanged += HandleColorChanged;

        // Apply existing value for late joiners
        if (colorIndex.Value >= 0)
        {
            ApplyColor(colorIndex.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        colorIndex.OnValueChanged -= HandleColorChanged;
    }

    private void HandleColorChanged(int oldValue, int newValue)
    {
        ApplyColor(newValue);
    }

    private void ApplyColor(int index)
    {
        if (index >= 0 && index < availableColors.Length)
        {
            spriteRenderer.color = availableColors[index];
        }
    }

    public void SetColor(int index)
    {
        if (!IsServer) return;

        colorIndex.Value = index;
    }
}