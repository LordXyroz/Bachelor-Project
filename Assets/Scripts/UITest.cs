using System;
using System.Net;
using System.Security;
using UnityEngine;
using Unity.Networking.Transport;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class UITest : MonoBehaviour
{
    public static NetworkEndPoint ServerEndPoint { get; private set; }

    // Update the ping statistics displayed in the ui. Should be called from the ping client every time a new ping is complete
    public static void UpdateStats(int count, int time)
    {
        m_pingCounter = count;
        m_pingTime = time;
    }

    private static int m_pingTime;
    private static int m_pingCounter;
    private string m_CustomIp = "";

    void Start()
    {
        m_pingTime = 0;
        m_pingCounter = 0;
        ServerEndPoint = default(NetworkEndPoint);
    }

    void OnGUI()
    {
        UpdatePingClientUI();
    }

    private void UpdatePingClientUI()
    {
        GUILayout.Label("PING " + m_pingCounter + ": " + m_pingTime + "ms");
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
        else
        {
            // Ping is running, display ui for stopping it
            if (GUILayout.Button("Stop ping"))
            {
                //Debug.Log("Disconnecting");
                ServerEndPoint = default(NetworkEndPoint);
            }
        }
    }
    

}
