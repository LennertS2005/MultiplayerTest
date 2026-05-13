using Unity.Netcode;
using UnityEngine;

public class PlayerAmmo : NetworkBehaviour
{
    [SerializeField] private Transform ammoBackground;
    [SerializeField] private Transform ammoBar;
    [SerializeField] private PlayerShoot playerShoot;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAmmo();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            ammoBackground.gameObject.SetActive(false);
            ammoBar.gameObject.SetActive(false);
            enabled = false;
            return;
        }
    }

    private void UpdateAmmo()
    {
        if (!IsOwner) return;

        if (playerShoot == null || ammoBar == null) return;

        float ammoPercent = playerShoot.GetShotgunAmmoPercent();

        // Scale X down as ammo drops
        ammoBar.localScale = new Vector3(ammoPercent, ammoBar.localScale.y, 1f);

        // Keep it anchored to the left by offsetting position
        // Half of (1 - percent) moves it back to the left
        float originalWidth = 1f;
        ammoBar.localPosition = new Vector3(
            -originalWidth / 2f * (1f - ammoPercent),
            ammoBar.localPosition.y,
            ammoBar.localPosition.z
        );
    }
}
