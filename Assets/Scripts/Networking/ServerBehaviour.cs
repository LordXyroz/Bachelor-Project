using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Jobs;
using Unity.Collections;
using Unity.Networking.Transport;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using MessagingInterfaces;

public class ServerBehaviour : IPing, IConnection
{
    NetworkingManager nm;

    public UdpCNetworkDriver m_ServerDriver;
    public NativeList<NetworkConnection> m_connections;
    private JobHandle m_updateHandle;

    TcpListener server;

    Socket listener;
    Socket handler;
    private bool listen;

    private List<(string name, int connectionNumber)> connectionNames;

    /// <summary>
    /// Task used to look for connections, token used to cancel task.
    /// </summary>
    private Task findConnections;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;


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
        if (!findConnections.IsCompleted && !findConnections.IsCanceled)
        {
            cancellationTokenSource.Cancel();
        }
    }
   
    
    /// <summary>
    /// Listen to a port and give message to clients that want to join.
    /// </summary>
    public void FindConnections()
    {
        server = null;
        try
        {
            /// Check if we were already canceled for some reason.
            cancellationToken.ThrowIfCancellationRequested();

            /// Set the TcpListener on port 13000.
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
            
            /// TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            /// Start listening for client requests.
            server.Start();

            /// Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;
            /// Enter the listening loop.
            while (server != null)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    /// Clean up.
                    server.Stop();
                    server = null;

                    /// Cancel task.
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (m_connections.IsCreated)
                {
                    /// Lobby is full:
                    if (m_connections.Length == 2) /// 2 is total amount of clients
                    {
                        server.Stop();
                        return;
                    }
                }

                /// Perform a blocking call to accept requests.
                TcpClient client = server.AcceptTcpClient();

                data = null;

                /// Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                /// Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string serverName = nm.chatField.transform.Find("HostText").GetComponent<Text>().text;
                    Debug.Log(serverName);
                    byte[] msg = Encoding.ASCII.GetBytes(serverName);

                    /// Send back a response.
                    stream.Write(msg, 0, msg.Length);
                }

                /// Shutdown and end connection
                client.Close();
            }

        }
        catch (SocketException e)
        {
            /// When host wants to close connection there will be sent a block call to WSACancelBlockingCall causing an wanted exception. Debug for testing.
            Debug.Log("SocketException: " + e);
        }
        catch (OperationCanceledException e)
        {
            Debug.Log("Canceled task: " + e);
        }
        finally
        {
            cancellationTokenSource.Dispose();

            /// Stop listening for new clients.
            server.Stop();
        }
    }

    /// <summary>
    /// Server behaviour constructor to set up basic settings for having a server behaviour in the game.
    /// </summary>
    public ServerBehaviour()
    {
        nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();
        /// Set token for connections.
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;


        connectionNames = new List<(string name, int connectionNumber)> { };

        findConnections = Task.Factory.StartNew(() => FindConnections(), cancellationToken);
        
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
        /// Put message in textbox for chat:
        nm.GetChatMessage(message);

        /// If any client has joined the lobby, the host will send the message to them:
        if (m_connections.IsCreated)
        {
            var messageWriter = new DataStreamWriter(message.Length + 15, Allocator.Temp);

            message += "<MessageReply>";
            messageWriter.Write(Encoding.ASCII.GetBytes(message));
            /// Go through all joined clients:
            for (int i = 0; i < m_connections.Length; i++)
            {
                m_ServerDriver.Send(m_connections[i], messageWriter);
            }
            messageWriter.Dispose();
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

    /// <summary>
    /// Destructor for server behaviour, makes sure that important data is cleaned up.
    /// </summary>
    ~ServerBehaviour()
    {
        cancellationTokenSource.Dispose();

        // All jobs must be completed before we can dispose the data they use
        m_ServerDriver.ScheduleUpdate().Complete();
        m_connections.Clear();
        m_updateHandle.Complete();
        m_ServerDriver.Dispose();
        m_connections.Dispose();
    }

    /// <summary>
    /// FixedUpdate is a function called ca. 50 times per second. Used to see for events through the network.
    /// If any messages is sent in the server behaviour will accordingly answer by sending a message back, and give some message to all other clients aswell.
    /// </summary>
    public void FixedUpdate()
    {
        if (!m_ServerDriver.IsCreated)
        {
            return;
        }

        /// Update the NetworkDriver. It schedules a job so we must wait for that job with Complete
        m_ServerDriver.ScheduleUpdate().Complete();

        /// Accept all new connections
        while (true)
        {
            var con = m_ServerDriver.Accept();
            /// "Nothing more to accept" is signaled by returning an invalid connection from accept
            if (!con.IsCreated)
                break;
            m_connections.Add(con);
        }

        if (!m_connections.IsCreated)
        {
            return;
        }

        for (int i = 0; i < m_connections.Length; ++i)
        {
            DataStreamReader strm;
            NetworkEvent.Type cmd;
            /// Pop all events for the connection
            while ((cmd = m_ServerDriver.PopEventForConnection(m_connections[i], out strm)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("Server - Connectevent");
                }
                if (cmd == NetworkEvent.Type.Data)
                {
                    /// A DataStreamReader.Context is required to keep track of current read position since DataStreamReader is immutable
                    var readerCtx = default(DataStreamReader.Context);
                    byte[] bytes = strm.ReadBytesAsArray(ref readerCtx, strm.Length);
                    string data = Encoding.ASCII.GetString(bytes);

                    /// Create a temporary DataStreamWriter to write back a receival message to the client:
                    var writer = new DataStreamWriter(1024, Allocator.Temp);
                    if (data.Contains("|"))
                    {
                        var str = data.Split('|');
                        var type = Type.GetType(str[1]);
                        dynamic msg = JsonUtility.FromJson(str[0], type);

                        MessagingManager.BroadcastMessage(msg, i);

                        //writer.Write(Encoding.ASCII.GetBytes("Connection updated<UpdateConnection>"));
                        //m_ServerDriver.Send(m_connections[i], writer);
                    }
                    else if (data.Contains("<Connecting>"))
                    {

                        /// Add new client to list of players in lobby:
                        string name = data.Substring(0, data.Length - 12);
                        Debug.Log("Server - Connecting client with name: " + name + " , nr of clients: " + m_connections.Length);
                        Debug.Log("server - connectionnames lenght: " + connectionNames.Count);
                        nm.AddPlayerName(name);

                        /// Add name of the client to serverlist:
                        connectionNames.Add((name, i));
                        
                        /// Setup info to send to all clients about new client connecting:
                        string message = nm.userName + "<HostName>";
                        
                        /// Get names of all players in lobby:
                        int j = 0;
                        while (j < nm.attackerNames.Count && nm.attackerNames[j].activeSelf)
                        {
                            message += nm.attackerNames[j].transform.Find("Text").GetComponent<Text>().text + "<AttackerName>";
                            j++;
                        }

                        j = 0;
                        while (j < nm.defenderNames.Count && nm.defenderNames[j].activeSelf)
                        {
                            message += nm.defenderNames[j].transform.Find("Text").GetComponent<Text>().text + "<DefenderName>";
                            j++;
                        }
                        
                        writer.Write(Encoding.ASCII.GetBytes(message + "<Connected>"));

                        /// Send a message to all of the clients with information about the connection.
                        for (int k = 0; k < m_connections.Length; k++)
                        {
                            m_ServerDriver.Send(m_connections[k], writer);
                        }
                    }
                    else if (data.Contains("<SwapAccepted>"))
                    {
                        /// Is run when client got message about other client accepted and will send back that swap has been completed.
                        Debug.Log("Server - Swap accepted");

                        /// Name of client sending to:
                        string name = data.Substring(0, data.IndexOf("<SwapAccepted>"));

                        /// Name of client message is from:
                        string clientName = connectionNames.Where(x => x.connectionNumber == i).First().name;

                        /// Send message to other client about accepting swap:
                        writer.Write(Encoding.ASCII.GetBytes(clientName + "<SwapAccepted>"));

                        /// Get connectionNr for sending message:
                        int connectionNr = connectionNames.Where(x => x.name == name).First().connectionNumber;
                        m_ServerDriver.Send(m_connections[connectionNr], writer);

                        /// Update UI for host aswell:
                        nm.FindSwapNames(name, clientName);
                    }
                    else if (data.Contains("<AcceptSwap>"))
                    {
                        /// Is run when one client accepts a swap.
                        
                        /// Name of client sending to:
                        string name = data.Substring(0, data.IndexOf("<AcceptSwap>"));

                        /// Name of client message is from:
                        string clientName = connectionNames.Where(x => x.connectionNumber == i).First().name;

                        /// Send message to other client about accepting swap:
                        writer.Write(Encoding.ASCII.GetBytes(clientName + "<AcceptSwap>"));

                        /// Get connectionNr for sending message:
                        int connectionNr = connectionNames.Where(x => x.name == name).First().connectionNumber;
                        m_ServerDriver.Send(m_connections[connectionNr], writer);
                    }
                    else if (data.Contains("<DeclineSwap>"))
                    {
                        /// Name of client to send message to:
                        string name = data.Substring(0, data.IndexOf("<DeclineSwap>"));
                        
                        /// Send message to client about declining swap:
                        writer.Write(Encoding.ASCII.GetBytes("<DeclineSwap>"));

                        /// Get connectionNr for sending message:
                        int connectionNr = connectionNames.Where(x => x.name == name).First().connectionNumber;
                        m_ServerDriver.Send(m_connections[connectionNr], writer);

                    }
                    else if (data.Contains("<SwapMessage>"))
                    {
                        Debug.Log("server - got message about a swap");
                        
                        string nameTo = data.Substring(0, data.IndexOf("<Name>"));
                        string nameFrom = data.Substring(data.IndexOf("<Name>") + 6, data.IndexOf("<SwapMessage>"));

                        /// Send message to client about other client wanting to swap position.
                        writer.Write(Encoding.ASCII.GetBytes(nameFrom + "<SwapMessage>"));

                        /// Get connectionNr for sending message:
                        int connectionNr = connectionNames.Where(x => x.name == nameTo).First().connectionNumber;
                        m_ServerDriver.Send(m_connections[connectionNr], writer);
                        
                    }
                    else if (data.Contains("<Message>"))
                    {
                        Debug.Log("Server - Got message from client.");
                        
                        writer.Write(Encoding.ASCII.GetBytes(data));

                        /// Send a message received to all clients:
                        for (int j = 0; j < m_connections.Length; j++)
                        {
                            m_ServerDriver.Send(m_connections[j], writer);
                        }

                        var type = data.Substring(data.LastIndexOf("<Message>") + 9);
                        Type typed = Type.GetType(type);
                        
                        data = data.Substring(0, data.IndexOf("<Message>"));

                        dynamic msg = JsonUtility.FromJson(data, typed);
                        
                        MessagingManager.BroadcastMessage(msg);
                    }
                    else if (data.Contains("<ChatMessage>"))
                    {
                        /// Encode message received to for writer to write:
                        Debug.Log("Server - Got chat message: " + data);
                        data = data.Substring(0, data.IndexOf("<ChatMessage>"));
                        writer.Write(Encoding.ASCII.GetBytes(data + "<MessageReply>"));

                        /// Send a message received to all clients:
                        for (int j = 0; j < m_connections.Length; j++)
                        {
                            m_ServerDriver.Send(m_connections[j], writer);
                        }
                        
                        /// Send message to the chat field.
                        nm.GetChatMessage(data);

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
                    /// Remove client that disconnected from list of people who ARE indeed connected.
                    string name = connectionNames.Find(x => x.connectionNumber == i).name;
                    connectionNames.Remove(connectionNames.Find(x => x.name == name));
                    nm.FindPlayerForRemoval(name);

                    Debug.Log("Server - Disconnecting client named " + name);

                    /// Create writer to send message to other clients that client with connection i is disconnecting.
                    var writer = new DataStreamWriter(100, Allocator.Temp);

                    writer.Write(Encoding.ASCII.GetBytes(name + "<ClientDisconnect>"));
                    for (int k = 0; k < m_connections.Length; k++)
                    {
                        /// Send message to client as long as it is not the connected one.
                        if (k != i)
                        {
                            m_ServerDriver.Send(m_connections[k], writer);
                        }
                    }

                    /// Dispose the writer when message has been sent.
                    writer.Dispose();

                    m_ServerDriver.Disconnect(m_connections[i]);
                    /// This connection no longer exists.
                    m_connections.RemoveAtSwapBack(i);
                    if (i >= m_connections.Length)
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Tells all clients to start game, and loads game scene for host
    /// </summary>
    public void StartGame()
    {
        var writer = new DataStreamWriter(128, Allocator.Temp);
        writer.Write(Encoding.ASCII.GetBytes("Start game<StartGame>"));

        for (int i = 0; i < m_connections.Length; i++)
            m_ServerDriver.Send(m_connections[i], writer);

        writer.Dispose();


        GameObject.Find("Canvas").SetActive(false);

        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }

    public void OnPing(Message message, int index)
    {
        switch (message.messageType)
        {
            case MessageTypes.Network.Ping:
                var msg = new Message("client", nm.userName, MessageTypes.Network.PingAck);
                var str = JsonUtility.ToJson(msg);

                str = str + "|" + msg.GetType();

                var writer = new DataStreamWriter(256, Allocator.Temp);
                
                writer.Write(Encoding.ASCII.GetBytes(str));
                m_ServerDriver.Send(m_connections[index], writer);

                writer.Dispose();
                break;
        }

        //throw new NotImplementedException();
    }

    public void OnConnection(ConnectMessage message, int index)
    {
        string name = message.senderName;
        nm.AddPlayerName(name);

        connectionNames.Add((name, index));

        Debug.Log("OnConnection - Connecting client with name: " + name + " , nr of clients: " + m_connections.Length);
        Debug.Log("OnConnection - connectionnames lenght: " + connectionNames.Count);

        var attackName = nm.attackerNames[0].transform.Find("Text").GetComponent<Text>().text;
        var defendName = nm.defenderNames[0].transform.Find("Text").GetComponent<Text>().text;
        var hostName = nm.chatField.transform.Find("HostText").GetComponent<Text>().text;

        var msg = new ConnectMessage(name, hostName, MessageTypes.Network.ConnectAck, nm.userName, attackName, defendName);
        var str = JsonUtility.ToJson(msg);
        str = str + "|" + msg.GetType();

        var writer = new DataStreamWriter(512, Allocator.Temp);
        writer.Write(Encoding.ASCII.GetBytes(str));

        for (int i = 0; i < m_connections.Length; i++)
            m_ServerDriver.Send(m_connections[i], writer);

        //throw new NotImplementedException();
    }
}
