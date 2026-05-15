using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;

    private List<GameObject> players = new();
    private List<PlayerHealth> healths = new();

    [SerializeField] private float respawnTime;
    private float respawnTimer;

    [SerializeField] private PlayerInput serverInput;
    [SerializeField] private List<Transform> spawnPoints;
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
        int amountAlive = 0;
        int idx = 0;

        foreach (var player in players)
        {
            if (player == null)
            {
                Debug.LogWarning("Found a null player reference. Removing from list.");

                healths.RemoveAt(idx);

                players.Remove(player);
                ++idx;
                break;
            }
        }

        foreach (var health in healths)
        {
            if (health.GetIsAlive()) ++amountAlive;
        }

        if (amountAlive <= 1)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer > respawnTime)
            {
                respawnTimer = 0;
                RespawnAllPlayers();
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

        PlayerHealth health = player.GetComponent<PlayerHealth>();

        healths.Add(health);

        Vector3 spawnPos = spawnPoints[(players.Count - 1) % spawnPoints.Count].position;
        health.Respawn(spawnPos);

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
        Vector3 spawnPos = Vector3.zero;
        int playerIndex = 0;

        if (!IsServer) return;
        foreach (var player in players)
        {
            spawnPos = spawnPoints[playerIndex % spawnPoints.Count].position;

            var health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Respawn(spawnPos);
            }
            ++playerIndex;

        }
    }
}
