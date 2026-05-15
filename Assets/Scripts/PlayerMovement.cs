using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private bool rotateWithMouse = false;

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

    public void Escape(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;

        FindFirstObjectByType<NetworkUI>().Escape(ctx);
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

        NameManager nameManager = FindFirstObjectByType<NameManager>();
        if (nameManager != null)
        {
            string playerName = nameManager.GetName();
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = $"Player {OwnerClientId}";
            }
            RegisterPlayerServerRpc(playerName);
        }
        else
        {
            Debug.LogWarning("NameManager not found in the scene. Player will be registered with default name.");
            RegisterPlayerServerRpc($"Player {OwnerClientId}");
        }
        RegisterKillUI();
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector2 move = moveAction.ReadValue<Vector2>();

        if (rotateWithMouse)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = mousePosition - (Vector2)transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            targetRotation = Quaternion.Euler(0, 0, angle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            //smoothly Rotate towards movement direction
            if (move != Vector2.zero)
            {
                float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg - 90f;
                targetRotation = Quaternion.Euler(0, 0, angle);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        Vector2 currentForward = transform.up;

        shootComponent.SetFacingDirection(currentForward);

        transform.position += (Vector3)(move * speed * Time.deltaTime);
    }

    private void RegisterKillUI()
    {
        PlayerStats stats = GetComponent<PlayerStats>();
        KillUI killUI = FindFirstObjectByType<KillUI>();
        killUI.Initialize(stats);
    }

    [ServerRpc]
    private void RegisterPlayerServerRpc(string playerName)
    {
        PlayerManager manager = PlayerManager.Instance;
        manager.AddPlayer(gameObject, playerName);
    }
}