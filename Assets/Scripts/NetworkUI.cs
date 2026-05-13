using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private string ipAddress = "";
    [SerializeField] private ushort port = 7777;

    [SerializeField] Canvas connectCanvas;

    private UnityTransport transport;

    private IPDisplay ipDisplay;

    private void Awake()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        ipDisplay = FindFirstObjectByType<IPDisplay>();
    }

    public void Host()
    {
        // Listen on all network interfaces
        transport.SetConnectionData("0.0.0.0", port);

        NetworkManager.Singleton.StartHost();
        connectCanvas.gameObject.SetActive(false);
        ipDisplay.RefreshIP();

        Debug.Log($"Hosting on port {port}");
    }

    public void Client()
    {
        if (string.IsNullOrEmpty(ipAddress))
        {
            SetIPAddress(GetLocalIPAddress());
            return;
        }

        // Connect to host IP
        transport.SetConnectionData(ipAddress, port);

        NetworkManager.Singleton.StartClient();
        connectCanvas.gameObject.SetActive(false);
        ipDisplay.RefreshIP();

        Debug.Log($"Connecting to {ipAddress}:{port}");
    }

    public void Server()
    {
        transport.SetConnectionData("0.0.0.0", port);

        NetworkManager.Singleton.StartServer();
        connectCanvas.gameObject.SetActive(false);
        ipDisplay.RefreshIP();

        Debug.Log($"Server started on port {port}");
    }

    // Optional helper for UI input field
    public void SetIPAddress(string newIP)
    {
        ipAddress = newIP;
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "No IPv4 found";
    }
}

/*using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void Server()
    {
        NetworkManager.Singleton.StartServer();
    }
}*/