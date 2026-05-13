using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;

    private List<GameObject> players = new();

    [SerializeField] private PlayerInput serverInput;
     public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            serverInput.enabled = true;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var player in players)
        {
            if (player == null)
            {
                Debug.LogWarning("Found a null player reference. Removing from list.");
                players.Remove(player);
                break;
            }
        }
    }

    public void AddPlayer(GameObject player, string playerName)
    {
        if (players.Count >= 4)
        {
            var netObj = player.GetComponent<NetworkObject>();

            if (netObj != null && netObj.IsSpawned)
            {
                netObj.Despawn(true);
            }

            Debug.LogWarning("Maximum player count reached. Cannot add more players.");
            return;
        }

        players.Add(player);

        Debug.Log("Added player. Count: " + players.Count);

        PlayerColor playerColor = player.GetComponent<PlayerColor>();

        if (playerColor != null)
        {
            playerColor.SetColor(players.Count - 1);
        }

        PlayerName playerNameComponent = player.GetComponent<PlayerName>();

        if (playerNameComponent != null)
        {
            playerNameComponent.SetPlayerName(playerName);
        }
    }

    public void RespawnAllPlayers()
    {
        if (!IsServer) return;
        foreach (var player in players)
        {
            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Respawn();
            }
        }
    }
}
