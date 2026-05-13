using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
/*
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] PlayerInput serverInput;
    private List<GameObject> players = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            serverInput.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        
        print("Player Count: " + players.Count);
    }

    public void AddPlayer(GameObject player)
    {
        if (!IsServer) return;
        players.Add(player);
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
}*/

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

    }

    public void AddPlayer(GameObject player)
    {
        players.Add(player);

        Debug.Log("Added player. Count: " + players.Count);
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
