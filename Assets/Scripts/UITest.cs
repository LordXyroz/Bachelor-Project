using System;
using System.Net;
using System.Security;
using UnityEngine;
using Unity.Networking.Transport;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class UITest : MonoBehaviour
{
    public static NetworkEndPoint ServerEndPoint { get; private set; }
    
    
    private string m_CustomIp = "";

    void Start()
    {
        ServerEndPoint = default(NetworkEndPoint);
    }

    void OnGUI()
    {
        UpdatePingClientUI();
    }

    private void UpdatePingClientUI()
    {
        if (!ServerEndPoint.IsValid)
        {
            // Ping is not currently running, display ui for starting a ping
            if (GUILayout.Button("Start ping"))
            {
                ushort port = 9000;
                if (string.IsNullOrEmpty(m_CustomIp))
                    ServerEndPoint = new IPEndPoint(IPAddress.Loopback, port);
                else
                {
                    string[] endpoint = m_CustomIp.Split(':');
                    ushort newPort = 0;
                    if (endpoint.Length > 1 && ushort.TryParse(endpoint[1], out newPort))
                        port = newPort;

                    //Debug.Log($"Connecting... : Server at {endpoint[0]}:{port}.");
                    ServerEndPoint = new IPEndPoint(IPAddress.Parse(endpoint[0]), port);
                }
            }

            m_CustomIp = GUILayout.TextField(m_CustomIp);
            
        }
    }
    

}
