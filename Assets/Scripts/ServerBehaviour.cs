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
using UnityEngine.UI;

public class ServerBehaviour : MonoBehaviour
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
    public void ThreadProc()
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

            int connectionAmount = 0;
            // Start listening for connections
            while (connectionAmount < 2)
            {
                /// As of right now we want exactly 2 people to connect to the host, so the host will be listening for clients until that amount has been reached.
                if (m_connections.IsCreated)
                {
                    connectionAmount = m_connections.Length;
                }

                /// Program is suspended while waiting for an incoming connection.
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


    /// <summary>
    /// When in lobby view this function will put message wanted to be sent in messageField, and send it to all connected clients.
    /// </summary>
    /// <param name="message"></param>
    public void SendChatMessage(string message)
    {
        // Put message in textbox for chat:
        MoveChatBox();
        GameObject.Find("MessageText1").GetComponent<Text>().text = message;

        /// If any client has joined the lobby, the host will send the message to them:
        if (m_connections.IsCreated)
        {
            /// Go through all joined clients:
            for (int i = 0; i < m_connections.Length; i++)
            {
                var messageWriter = new DataStreamWriter(message.Length+15, Allocator.Temp);

                message += "<MessageReply>";
                messageWriter.Write(Encoding.ASCII.GetBytes(message));

                m_ServerDriver.Send(m_connections[i], messageWriter);
                messageWriter.Dispose();
            }
        }
    }

    /// <summary>
    /// Moves every text in chatField one step up for the new message to be put at bottom.
    /// </summary>
    void MoveChatBox()
    {
        GameObject.Find("MessageText5").GetComponent<Text>().text = GameObject.Find("MessageText4").GetComponent<Text>().text;
        GameObject.Find("MessageText4").GetComponent<Text>().text = GameObject.Find("MessageText3").GetComponent<Text>().text;
        GameObject.Find("MessageText3").GetComponent<Text>().text = GameObject.Find("MessageText2").GetComponent<Text>().text;
        GameObject.Find("MessageText2").GetComponent<Text>().text = GameObject.Find("MessageText1").GetComponent<Text>().text;
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
                    var writer = new DataStreamWriter(150, Allocator.Temp);
                    if (data.Contains("<UpdateConnection>"))
                    {
                        /*writer.Write(Encoding.ASCII.GetBytes("Connection updated<UpdateConnection>"));
                        m_ServerDriver.Send(m_connections[i], writer);*/
                    }
                    else if (data.Contains("<Connecting>"))
                    {
                        Debug.Log("Server - Connecting client with name: " + data);

                        /// Add new client to list of players in lobby:
                        data = data.Substring(0, data.Length - 12);
                        GameObject.Find("GameManager").GetComponent<NetworkingManager>().AddPlayerName(data);

                        /// Send info back to client just connected about the lobby.
                        NetworkingManager nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();
                        string message = nm.matchName + "<Match>";
                        message += nm.userName + "<HostName>";
                        if (nm.playerName2.activeSelf)
                        {
                            message += nm.playerName2.transform.Find("Text").GetComponent<Text>().text + "<PlayerName>";
                        }

                        writer.Write(Encoding.ASCII.GetBytes(message + "<Connected>"));
                        
                        /// Send a message back to the client, so it is known that connection is secured.
                        m_ServerDriver.Send(m_connections[i], writer);
                    }
                    else if (data.Contains("<ChatMessage>"))
                    {
                        /// Encode message received to for writer to write:
                        Debug.Log("Server - Got message: " + data);
                        data = data.Substring(0, data.Length - 13);
                        writer.Write(Encoding.ASCII.GetBytes(data + "<MessageReply>"));

                        /// Send a message received to all clients:
                        for (int j = 0; j < m_connections.Length; j++)
                        {
                            m_ServerDriver.Send(m_connections[j], writer);
                        }

                        // Put message in textbox for testing:
                        MoveChatBox();
                        GameObject.Find("MessageText1").GetComponent<Text>().text = data;

                        /// Give the message received to the host aswell:
                        Debug.Log("Host has data: " + data);
                    }
                    else if (data.Contains("<Scenario>"))
                    {
                        /// Send scenario wanted by the host to all clients:
                        writer.Write(Encoding.ASCII.GetBytes(data));
                        for (int j = 0; j < m_connections.Length; j++)
                        {
                            m_ServerDriver.Send(m_connections[j], writer);
                        }
                        Debug.Log("Server - Got scenario: " + data);
                    }

                    /// Dispose the writer when the message to client has been sent.
                    writer.Dispose();
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Server - Disconnecting client...");
                    
                    /// Remove client that disconnected from list of people who ARE indeed connected.
                    GameObject.Find("GameManager").GetComponent<NetworkingManager>().RemovePlayerAtPosition(i);


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
