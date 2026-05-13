using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject pelletPrefab;
    [SerializeField] private Transform firePoint; // empty child object at gun tip
    [SerializeField] private float shootCooldown = 0.5f;
    [SerializeField] private float shotgunCooldown = 2f;

    bool canShoot = false;

    private PlayerInput playerInput;
    private InputAction shootAction;
    private InputAction shotgunAction;
    private Vector2 facingDirection = Vector2.right; // default facing right
    private float lastShootTime = 0f;

    private ulong clientID;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
    }

    private void Update()
    {
        if (IsOwner)
        {
            if (!canShoot) return;
            if (Time.time - lastShootTime >= shootCooldown)
            {
                ShootServerRpc(facingDirection, firePoint.position, clientID);
                lastShootTime = Time.time;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        playerInput.enabled = true;
        shootAction = playerInput.actions["Shoot"];
        shootAction.performed += OnShoot;
        shootAction.canceled += OnShootCanceled;
        shotgunAction = playerInput.actions["Shotgun"];
        shotgunAction.performed += OnShotgun;

        clientID = NetworkManager.Singleton.LocalClientId;
        print("ClientID added: " + clientID);
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            shootAction.performed -= OnShoot;
        }
    }

    // Call this from PlayerMovement so shooting knows which way player faces
    public void SetFacingDirection(Vector2 direction)
    {
        if (direction != Vector2.zero)
            facingDirection = direction.normalized;
    }

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        canShoot = true;
    }

    private void OnShotgun(InputAction.CallbackContext ctx)
    {
        if (canShoot == true) return; //Cant shotgun if already shooting
        if (Time.time - lastShootTime < shotgunCooldown) return;

        //Shoot 10 bullets in a spread pattern
        float totalSpreadAngle = 90f;
        int bulletCount = 10;
        float angleStep = totalSpreadAngle / (bulletCount - 1);
        float startAngle = -totalSpreadAngle / 2;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 spreadDirection = Quaternion.Euler(0, 0, angle) * facingDirection;
            ShotgunServerRpc(spreadDirection, firePoint.position, clientID);
        }
        lastShootTime = Time.time;
    }

    private void OnShootCanceled(InputAction.CallbackContext ctx)
    {
        canShoot = false;
    }

    [ServerRpc]
    private void ShootServerRpc(Vector2 direction, Vector3 spawnPosition, ulong shooterClientId)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.GetComponent<BulletNetwork>().Initialize(direction, shooterClientId);
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    private void ShotgunServerRpc(Vector2 direction, Vector3 spawnPosition, ulong shooterClientId)
    {
        GameObject pellet = Instantiate(pelletPrefab, spawnPosition, Quaternion.identity);
        pellet.GetComponent<BulletNetwork>().Initialize(direction, shooterClientId);
        pellet.GetComponent<NetworkObject>().Spawn();
    }
}