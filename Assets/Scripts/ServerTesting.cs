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
using System.Threading.Tasks;

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
        byte[] bytes = new Byte[1024];

        // Establish the local endpoint for the socket.  
        // Dns.GetHostName returns the name of the   
        // host running the application.  
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[0];


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
                //Debug.Log("Serverside(socket) - Text received: " + data);
                // Echo the data back to the client.
                byte[] msg = Encoding.ASCII.GetBytes(GetLocalIPAddress());

                handler.Send(msg);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                break;
            }

        }
        catch (Exception e)
        {
            Debug.Log("Server exception: " + e.ToString());
        }
    
    }
    // End of thread test.


    void Start()
    {
        
        //Task.Factory.StartNew(() => ThreadProc());
        Thread t = new Thread(new ThreadStart(ThreadProc));

        t.Start();


        // Create the server driver, bind it to a port and start listening for incoming connections
        m_ServerDriver = new UdpCNetworkDriver(new INetworkParameter[0]);
        if (m_ServerDriver.Bind(new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), 9000)) != 0)
            Debug.Log("Failed to bind to port 9000");
        else
        {
            m_ServerDriver.Listen();
            Debug.Log("Server - Listening...");
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
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("Server - Connectevent");
                }
                if (cmd == NetworkEvent.Type.Data)
                {
                    // If client is trying to connect to the server, send back data about this.

                    // A DataStreamReader.Context is required to keep track of current read position since DataStreamReader is immutable
                    var readerCtx = default(DataStreamReader.Context);
                    byte[] bytes = strm.ReadBytesAsArray(ref readerCtx, strm.Length);
                    string data = Encoding.ASCII.GetString(bytes);

                    /// Create a temporary DataStreamWriter to write back a receival message to the client:
                    var writer = new DataStreamWriter(100, Allocator.Temp);
                    if (data.Contains("<UpdateConnection>"))
                    {
                        writer.Write(Encoding.ASCII.GetBytes("Connection updated<UpdateConnection>"));
                        m_ServerDriver.Send(m_connections[i], writer);
                    }
                    else if (data.Contains("<Connecting>"))
                    {
                        Debug.Log("Server - Connecting client...");
                        writer.Write(Encoding.ASCII.GetBytes("<Connected>"));

                        /// Send a message back to the client.
                        m_ServerDriver.Send(m_connections[i], writer);
                    }
                    else if (data.Contains("<Message>"))
                    {
                        Debug.Log("Server - Got message: " + data);
                        writer.Write(Encoding.ASCII.GetBytes("Sending this message to client.<MessageReply>" + "  " + i + " : " + m_connections.Length.ToString()));

                        /// Send a message to all clients:
                        m_ServerDriver.Send(m_connections[i], writer);
                    }
                    else if (data.Contains("<Class>"))
                    {
                        Debug.Log("Server - Got class: " + data);
                    }

                    /// Dispose the writer when the message to client has been sent.
                    writer.Dispose();
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Server - Disconnecting client...");

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
