using Unity.Netcode;
using UnityEngine;

public class KillUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI killText;
    private PlayerStats localStats;

    private void Start()
    {
        
    }

    public void Initialize(PlayerStats stats)
    {
        localStats = stats;

        localStats.KillCount.OnValueChanged += OnKillsChanged;

        // Set initial kill count display
        killText.text = $"Kills: {localStats.KillCount.Value}";
    }

    private void OnKillsChanged(int oldValue, int newValue)
    {
        killText.text = $"Kills: {newValue}";
    }
}
