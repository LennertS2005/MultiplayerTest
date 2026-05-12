using UnityEngine;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject healthBarObject;
    [SerializeField] private Transform healthBar;
    [SerializeField] private PlayerHealth playerHealth;

    private Camera mainCamera;

    private float healthBarTimeout = 3f;
    private float healthBarTimer = 0f;

    private void Start()
    {
        healthBarObject.SetActive(false);
        mainCamera = Camera.main;
        playerHealth.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(playerHealth.GetHealthPercent());
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            healthBarObject.transform.rotation = mainCamera.transform.rotation;
        }

        // Hide health bar after timeout
        if (healthBarObject.activeSelf)
        {
            healthBarTimer += Time.deltaTime;
            if (healthBarTimer >= healthBarTimeout)
            {
                healthBarObject.SetActive(false);
            }
        }
    }

    private void UpdateHealthBar(float percent)
    {
        healthBarTimer = 0f;
        healthBarObject.SetActive(true);

        // Scale X down as health drops
        healthBar.localScale = new Vector3(percent, healthBar.localScale.y, 1f);

        // Keep it anchored to the left by offsetting position
        // Half of (1 - percent) moves it back to the left
        float originalWidth = 1f;
        healthBar.localPosition = new Vector3(
            -originalWidth / 2f * (1f - percent),
            healthBar.localPosition.y,
            healthBar.localPosition.z
        );
    }
}