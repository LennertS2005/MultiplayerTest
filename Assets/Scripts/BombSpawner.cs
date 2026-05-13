using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BombSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float spawnInterval = 10f;

    [SerializeField] private float minX = -9f;
    [SerializeField] private float maxX = 9f;

    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private float timer;

    private List<Bomb> bombComp = new List<Bomb> { };


    private void Update()
    {
        // Only the server spawns bombs
        if (!IsServer) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnBomb();
        }
        foreach (Bomb bomb in bombComp)
        {
            if (bombComp != null && bomb.shouldDespawn)
            {
                bomb.GetComponent<NetworkObject>().Despawn();
                bombComp.Remove(bomb);
                break;
            }
        }
    }

    private void SpawnBomb()
    {
        if (!IsServer) return;

        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f
        );

        GameObject bomb = Instantiate(
            bombPrefab,
            spawnPosition,
            Quaternion.identity
        );

        bombComp.Add(bomb.GetComponent<Bomb>());

        // Spawn across network
        bomb.GetComponent<NetworkObject>().Spawn();
    }
}