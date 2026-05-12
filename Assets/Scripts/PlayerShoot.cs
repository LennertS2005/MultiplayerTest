using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // empty child object at gun tip
    [SerializeField] private float shootCooldown = 0.5f;

    bool canShoot = false;

    private PlayerInput playerInput;
    private InputAction shootAction;
    private Vector2 facingDirection = Vector2.right; // default facing right
    private float lastShootTime = 0f;
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
                ShootServerRpc(facingDirection, firePoint.position);
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

    private void OnShootCanceled(InputAction.CallbackContext ctx)
    {
        canShoot = false;
    }

    [ServerRpc]
    private void ShootServerRpc(Vector2 direction, Vector3 spawnPosition)
    {
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        bullet.GetComponent<BulletNetwork>().Initialize(direction);
        bullet.GetComponent<NetworkObject>().Spawn();
    }
}