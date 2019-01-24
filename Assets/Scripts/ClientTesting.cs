﻿using System.Net;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class ClientTesting : MonoBehaviour
{
    struct PendingPing
    {
        public int id;
        public float time;
    }

    private UdpCNetworkDriver m_ClientDriver;
    private NetworkConnection m_clientToServerConnection;
    // pendingPing is a ping sent to the server which have not yet received a response.
    private PendingPing m_pendingPing;
    // The ping stats are two integers, time for last ping and number of pings
    private int m_lastPingTime;
    private int m_numPingsSent;

    
    void Start()
    {
        m_ClientDriver = new UdpCNetworkDriver(new INetworkParameter[0]);
    }

    void OnDestroy()
    {
        m_ClientDriver.Dispose();
    }

    void FixedUpdate()
    {
        // Update the ping client UI with the ping statistics computed by teh job scheduled previous frame since that
        // is now guaranteed to have completed
        UITest.UpdateStats(m_numPingsSent, m_lastPingTime);

        // Update the NetworkDriver. It schedules a job so we must wait for that job with Complete
        m_ClientDriver.ScheduleUpdate().Complete();

        // If the client ui indicates we should be sending pings but we do not have an active connection we create one
        if (UITest.ServerEndPoint.IsValid && !m_clientToServerConnection.IsCreated)
            m_clientToServerConnection = m_ClientDriver.Connect(UITest.ServerEndPoint);
        // If the client ui indicates we should not be sending pings but we do have a connection we close that connection
        if (!UITest.ServerEndPoint.IsValid && m_clientToServerConnection.IsCreated)
        {
            m_clientToServerConnection.Disconnect(m_ClientDriver);
            m_clientToServerConnection = default(NetworkConnection);
        }

        DataStreamReader strm;
        NetworkEvent.Type cmd;
        // Process all events on the connection. If the connection is invalid it will return Empty immediately
        while ((cmd = m_clientToServerConnection.PopEvent(m_ClientDriver, out strm)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                // When we get the connect message we can start sending data to the server
                // Set the ping id to a sequence number for the new ping we are about to send
                m_pendingPing = new PendingPing { id = m_numPingsSent, time = Time.fixedTime };
                // Create a 4 byte data stream which we can store our ping sequence number in
                var pingData = new DataStreamWriter(4, Allocator.Temp);
                pingData.Write(m_numPingsSent);
                m_clientToServerConnection.Send(m_ClientDriver, pingData);
                pingData.Dispose();
                // Update the number of sent pings
                ++m_numPingsSent;
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                // When the pong message is received we calculate the ping time and disconnect
                m_lastPingTime = (int)((Time.fixedTime - m_pendingPing.time) * 1000);
                m_clientToServerConnection.Disconnect(m_ClientDriver);
                m_clientToServerConnection = default(NetworkConnection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                // If the server disconnected us we clear out connection
                m_clientToServerConnection = default(NetworkConnection);
            }
        }
    }
}
