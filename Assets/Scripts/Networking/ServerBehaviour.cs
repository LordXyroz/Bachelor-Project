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

public class ServerBehaviour : IPing, IConnection, IChatMessage, ISwap, IDisposable
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
            Debug.Log("Canceling tcplistener");
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

            /// Enter the listening loop.
            while (server != null)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log("Canceling tcplistener, cancellation");
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
                        Debug.Log("Canceling tcplistener, server full");
                        server.Stop();
                        return;
                    }
                }

                /// Perform a blocking call to accept requests.
                TcpClient client = server.AcceptTcpClient();
                
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
            /// Debug.Log("SocketException: " + e);
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
        m_connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

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
            
            NetworkEvent.Type cmd;
            /// Pop all events for the connection
            while ((cmd = m_ServerDriver.PopEventForConnection(m_connections[i], out DataStreamReader strm)) != NetworkEvent.Type.Empty)
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
                        /// using message

                        var str = data.Split('|');
                        var type = Type.GetType(str[1]);
                        dynamic msg = JsonUtility.FromJson(str[0], type);

                        MessagingManager.BroadcastMessage((Message) msg, i);
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
                    DisconnectClient(i);

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
        BroadcastMessage(new Message("Client", nm.userName, MessageTypes.Game.Start));

        GameObject.Find("Canvas").SetActive(false);

        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }

    public void DisconnectClient(int index)
    {
        /// Remove client that disconnected from list of people who ARE indeed connected.
        string name = connectionNames.Find(x => x.connectionNumber == index).name;
        connectionNames.Remove(connectionNames.Find(x => x.name == name));
        nm.FindPlayerForRemoval(name);

        Debug.Log("Server - Disconnecting client named " + name);

        BroadcastMessage(new DiscClientMessage("Client", nm.userName, MessageTypes.Network.ClientDisconnect, name), index);

        m_ServerDriver.Disconnect(m_connections[index]);
        /// This connection no longer exists.
        Debug.Log("---LOG---: Connections before - " + m_connections.Length);

        m_connections.RemoveAtSwapBack(index);

        Debug.Log("---LOG---: Connections after - " + m_connections.Length);

        for (int j = 0; j < m_connections.Length; j++)
        {
            connectionNames[j] = (connectionNames[j].name, j);
        }

        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;

        findConnections = Task.Factory.StartNew(() => FindConnections(), cancellationToken);
    }

    public void BroadcastMessage(dynamic message)
    {
        var str = JsonUtility.ToJson(message);
        str = str + "|" + message.GetType();
        
        var writer = new DataStreamWriter(1024, Allocator.Temp);
        writer.Write(Encoding.ASCII.GetBytes(str));

        for (int i = 0; i < m_connections.Length; i++)
            m_ServerDriver.Send(m_connections[i], writer);

        writer.Dispose();
    }

    public void BroadcastMessage(dynamic message, int index)
    {
        string str = JsonUtility.ToJson(message);
        str = str + "|" + message.GetType() + "\0\0\0\0\0\0\0\0";


        var writer = new DataStreamWriter(str.Length, Allocator.Temp);
        writer.Write(Encoding.ASCII.GetBytes(str));

        if (message.messageType == MessageTypes.Game.SaveFile)
            Debug.Log("---LOG---: Length of package in writer: " + writer.Length);
        
        m_ServerDriver.Send(m_connections[index], writer);
        
        writer.Dispose();
    }

    public void BroadcastMessage(DiscClientMessage message, int index)
    {
        var str = JsonUtility.ToJson(message);
        str = str + "|" + message.GetType();

        var writer = new DataStreamWriter(1024, Allocator.Temp);
        writer.Write(Encoding.ASCII.GetBytes(str));

        for (int i = 0; i < m_connections.Length; i++)
            if (i != index)
                m_ServerDriver.Send(m_connections[i], writer);

        writer.Dispose();
    }

    public void OnPing(Message message, int index)
    {
        var msg = new Message("client", nm.userName, MessageTypes.Network.PingAck);
        var str = JsonUtility.ToJson(msg);

        str = str + "|" + msg.GetType();
        
        var writer = new DataStreamWriter(256, Allocator.Temp);

        writer.Write(Encoding.ASCII.GetBytes(str));
        m_ServerDriver.Send(m_connections[index], writer);

        writer.Dispose();
    }

    public void OnConnection(ConnectMessage message, int index)
    {
        string name = message.senderName;
        nm.AddPlayerName(name);

        connectionNames.Add((name, index));

        var attackName = nm.attackerNames[0].transform.Find("Text").GetComponent<Text>().text;
        var defendName = nm.defenderNames[0].transform.Find("Text").GetComponent<Text>().text;
        var hostName = nm.chatField.transform.Find("HostText").GetComponent<Text>().text;

        var msg = new ConnectMessage(name, hostName, MessageTypes.Network.ConnectAck, nm.userName, attackName, defendName);
        BroadcastMessage(msg);

        BroadcastMessage(new SaveFileMessage(name, hostName, MessageTypes.Game.SaveFile, nm.saveFile), index);
    }

    public void OnChatMessage(ChatMessage message)
    {
        nm.GetChatMessage(message.message);
        BroadcastMessage(message);
    }

    public void OnSwap(SwapMessage message)
    {
        string target = message.targetName;
        string sender = message.senderName;

        int connectionNo = connectionNames.Where(x => x.name == target).First().connectionNumber;
        BroadcastMessage(message, connectionNo);

        if (message.swapMsg == ClientBehaviour.SwapMsgType.Acknowledge)
            nm.FindSwapNames(sender, target);
    }

    public void SendSaveFile(int index)
    {
        
    }

    #region IDisposable
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                
                
            }
            Debug.Log("Running Dispose");
            //cancellationTokenSource.Dispose();

            for (int i = m_connections.Length - 1; i >= 0; i--)
                DisconnectClient(i);

            // All jobs must be completed before we can dispose the data they use
            m_ServerDriver.ScheduleUpdate().Complete();
            //m_connections.Clear();
            //m_updateHandle.Complete();
            m_ServerDriver.Dispose();
            m_connections.Dispose();

            disposedValue = true;
        }
    }
    
    ~ServerBehaviour()
    {
      Dispose(false);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);

    }
    #endregion
}
