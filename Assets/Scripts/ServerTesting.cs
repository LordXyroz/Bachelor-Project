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


    // Testing thread:
    public static void ThreadProc()
    {
        // Incoming data from the client.  
        string data = null;
        System.Net.IPAddress wantedAddress;
        for (int i = 0; i < 10; i++)
        {
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            Debug.Log("Serverside - server ip address: " + ipHostInfo.AddressList[3]);
            wantedAddress = ipHostInfo.AddressList[3];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    // Show the data on the console.  
                    Debug.Log("Serverside - Text received: " + data);

                    // Echo the data back to the client.
                    byte[] msg = Encoding.ASCII.GetBytes(wantedAddress.ToString());

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Debug.Log("Server exception: " + e.ToString());
            }
        }
    }
    // End of thread test.


    void Start()
    {
        Thread t = new Thread(new ThreadStart(ThreadProc));

        t.Start();

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
                    // A DataStreamReader.Context is required to keep track of current read position since
                    // DataStreamReader is immutable
                    var readerCtx = default(DataStreamReader.Context);
                    int id = strm.ReadInt(ref readerCtx);
                    // Create a temporary DataStreamWriter to keep our serialized pong message
                    var writer = new DataStreamWriter(4, Allocator.Temp);
                    writer.Write(id);
                    // Send the pong message with the same id as the ping
                    m_ServerDriver.Send(m_connections[i], writer);
                    writer.Dispose();
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    // This connection no longer exist, remove it from the list
                    // The next iteration will operate on the new connection we swapped in so as long as it exist the
                    // loop can continue
                    m_connections.RemoveAtSwapBack(i);
                    if (i >= m_connections.Length)
                        break;
                }
            }
        }
    }
}
