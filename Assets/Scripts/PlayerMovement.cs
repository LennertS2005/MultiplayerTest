using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    private PlayerInput playerInput;
    private InputAction moveAction;

    private Quaternion targetRotation;

    PlayerShoot shootComponent;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        // Disable immediately — we re-enable it only if we're the owner
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

        // Only the owner re-enables PlayerInput and grabs the action
        playerInput.enabled = true;
        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector2 move = moveAction.ReadValue<Vector2>();

        //smoothly Rotate towards movement direction
        if (move != Vector2.zero)
        {
            float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg - 90f;
            targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
        }

        Vector2 currentForward = transform.up;

        shootComponent.SetFacingDirection(currentForward);

        transform.position += (Vector3)(move * speed * Time.deltaTime);
    }
}