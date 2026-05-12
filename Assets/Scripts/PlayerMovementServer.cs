using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementServer : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private Quaternion targetRotation;
    PlayerShoot shootComponent;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.enabled = false;
        shootComponent = GetComponent<PlayerShoot>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        playerInput.enabled = true;
        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {
        if (!IsOwner) return;
        Vector2 move = moveAction.ReadValue<Vector2>();

        // Calculate rotation locally for facing direction
        if (move != Vector2.zero)
        {
            float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg - 90f;
            targetRotation = Quaternion.Euler(0, 0, angle);
        }

        Vector2 currentForward = transform.up;
        shootComponent.SetFacingDirection(currentForward);

        // Send input to server — server does the actual movement
        MoveServerRpc(move, targetRotation);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 move, Quaternion rotation)
    {
        // Server applies position and rotation
        transform.position += (Vector3)(move * speed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rotation,
            360 * Time.deltaTime
        );
    }
}