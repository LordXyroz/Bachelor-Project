using UnityEngine;
using System.Net;
using Unity.Jobs;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;



public class ServerTesting : MonoBehaviour
{
    public UdpCNetworkDriver m_ServerDriver;
    private NativeList<NetworkConnection> m_connections;

    private JobHandle m_updateHandle;

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
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
    
    


    void Start()
    {
        /*Thread t = new Thread(new ThreadStart(ThreadProc));

        t.Start();*/

        // Create the server driver, bind it to a port and start listening for incoming connections
        m_ServerDriver = new UdpCNetworkDriver(new INetworkParameter[0]);
        if (m_ServerDriver.Bind(new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
        {
            m_ServerDriver.Listen();
            Debug.Log("Listening...");
        }

        m_connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    void OnDestroy()
    {
        // All jobs must be completed before we can dispose the data they use
        m_updateHandle.Complete();
        m_ServerDriver.Dispose();
        m_connections.Dispose();
    }

    void FixedUpdate()
    {

       
        // Update the NetworkDriver. It schedules a job so we must wait for that job with Complete
        m_ServerDriver.ScheduleUpdate().Complete();
        
        // Accept all new connections
        while (true)
        {
            var con = m_ServerDriver.Accept();
            // "Nothing more to accept" is signaled by returning an invalid connection from accept
            if (!con.IsCreated)
                break;
            m_connections.Add(con);
        }
        

        for (int i = 0; i < m_connections.Length; ++i)
        {
            DataStreamReader strm;
            NetworkEvent.Type cmd;
            // Pop all events for the connection
            while ((cmd = m_ServerDriver.PopEventForConnection(m_connections[i], out strm)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    // If client is trying to connect to the server, send back data about this.

                    // A DataStreamReader.Context is required to keep track of current read position since DataStreamReader is immutable
                    var readerCtx = default(DataStreamReader.Context);
                    byte[] bytes = strm.ReadBytesAsArray(ref readerCtx, strm.Length);
                    string data = Encoding.ASCII.GetString(bytes);

                    /// Create a temporary DataStreamWriter to write back a receival message to the client:
                    var writer = new DataStreamWriter(50, Allocator.Temp);
                    if (data.Contains("<Connecting>"))
                    {
                        Debug.Log("Server - Client connecting to server...");
                        writer.Write(Encoding.ASCII.GetBytes("<Connected>"));
                    }
                    else if (data.Contains("<Message>"))
                    {
                        Debug.Log("Server - Got message: " + data);
                        writer.Write(Encoding.ASCII.GetBytes("Sending this message to client.<MessageReply>" + " : " + m_connections.Length.ToString()));
                    }
                    /// Send a message back to the client.
                    m_ServerDriver.Send(m_connections[i], writer);
                    writer.Dispose();
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {

                    // This connection no longer exist, remove it from the list
                    // The next iteration will operate on the new connection we swapped in so as long as it exist the loop can continue.
                    m_connections.RemoveAtSwapBack(i);
                    if (i >= m_connections.Length)
                        break;
                }
            }
        }
    }
}
