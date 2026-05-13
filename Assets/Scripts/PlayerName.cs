using TMPro;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine;

public class PlayerName : NetworkBehaviour
{
    [SerializeField] private TMP_Text nameText;

    private NetworkVariable<FixedString32Bytes> playerName = new(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        playerName.OnValueChanged += OnNameChanged;

        ApplyName(playerName.Value.ToString());
    }

    public override void OnNetworkDespawn()
    {
        playerName.OnValueChanged -= OnNameChanged;
    }

    private void OnNameChanged(
        FixedString32Bytes oldValue,
        FixedString32Bytes newValue)
    {
        ApplyName(newValue.ToString());
    }

    private void ApplyName(string newName)
    {
        if (nameText != null)
        {
            nameText.text = newName;
        }
    }

    public void SetPlayerName(string newName)
    {
        if (!IsServer) return;

        playerName.Value = newName;
    }
}