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
using System.Threading.Tasks;

public class ServerBehaviour
{
    public UdpCNetworkDriver m_ServerDriver;
    public NativeList<NetworkConnection> m_connections;
    private JobHandle m_updateHandle;

    TcpListener server;

    Socket listener;
    Socket handler;
    private bool listen;

    /// <summary>
    /// Get ipaddress for pc this function is run on.
    /// </summary>
    /// <returns></returns>
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


    /// <summary>
    /// Stop listening for incoming connections as the lobby is full.
    /// </summary>
    public void StopListening()
    {
        server.Stop();
        server = null;
    }
   
    
    /// <summary>
    /// Listen to a port and give message to clients that want to join.
    /// </summary>
    public void FindConnections()
    {
        server = null;
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 13000;
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localAddr = null;
            for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
            {
                if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    localAddr = ipHostInfo.AddressList[i];
                }
            }

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;
            // Enter the listening loop.
            while (server != null)
            {
                Debug.Log("newconnection - Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("newconnection - Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Debug.Log("newconnection - Received: " + data);
                    

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Debug.Log("newconnection - sent: " + data);
                }
                Debug.Log("Sent message");

               
                Thread.Sleep(100);
                // Shutdown and end connection
                client.Close();
            }

        }
        catch (SocketException e)
        {
            /// When host wants to close connection there will be sent a block call to WSACancelBlockingCall causing an wanted exception. Debug for testing.
            /// Debug.Log("SocketException: " + e);
        }
        finally
        {
            /// Stop listening for new clients.
            server.Stop();
        }
    }

    public ServerBehaviour()
    {
        Task.Factory.StartNew(() => FindConnections());

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

    ~ServerBehaviour()
    {
        // All jobs must be completed before we can dispose the data they use
        m_updateHandle.Complete();
        m_ServerDriver.Dispose();
        m_connections.Dispose();
    }

    public void FixedUpdate()
    {
        if (m_ServerDriver.IsCreated)
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

            if (m_connections.IsCreated)
            {
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
                                writer.Write(Encoding.ASCII.GetBytes("Connection updated<UpdateConnection>"));
                                m_ServerDriver.Send(m_connections[i], writer);
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

                                /// Host uses index 0, other clients starts at 1.
                                int j = 1;
                                while (j < nm.playerNames.Count && nm.playerNames[j].activeSelf)
                                {
                                    message += nm.playerNames[j].transform.Find("Text").GetComponent<Text>().text + "<PlayerName>";
                                    j++;
                                }

                                writer.Write(Encoding.ASCII.GetBytes(message + "<Connected>"));
                                /// Send a message back to the client, with information about the connection.
                                m_ServerDriver.Send(m_connections[i], writer);


                                /// Send another message to all other clients that this client has connected:
                                var connectionWriter = new DataStreamWriter(150, Allocator.Temp);
                                message = data + "<ClientConnect>";
                                connectionWriter.Write(Encoding.ASCII.GetBytes(message));
                                
                                for (int k = 0; k < m_connections.Length; k++)
                                {
                                    /// Send message to client as long as it is not the connected one.
                                    if (k != i)
                                    {
                                        Debug.Log("Sending message to client that someone connected!");
                                        m_ServerDriver.Send(m_connections[k], connectionWriter);
                                    }
                                }
                                connectionWriter.Dispose();
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
                            GameObject.Find("GameManager").GetComponent<NetworkingManager>().RemovePlayerAtPosition(i + 1);


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
    }
}
