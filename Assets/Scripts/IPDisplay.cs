using UnityEngine;

public class IPDisplay : MonoBehaviour
{
    private TMPro.TextMeshProUGUI ipText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ipText = GetComponent<TMPro.TextMeshProUGUI>();
        ipText.text = $"Local IP: {NetworkUI.GetLocalIPAddress()}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshIP()
    {
        ipText.text = $"Local IP: {NetworkUI.GetLocalIPAddress()}";
    }
}
