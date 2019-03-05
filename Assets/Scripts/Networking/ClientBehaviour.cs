using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine.SceneManagement;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class ClientBehaviour
{

    ///  results contain the ipaddresses possible to connect to.
    readonly List<string[]> results = new List<string[]>();
    
    public static NetworkEndPoint ServerEndPoint { get; private set; }
    private bool connect;

    public UdpCNetworkDriver m_ClientDriver;
    public NetworkConnection m_clientToServerConnection;
    private bool m_clientWantsConnection;

    /// <summary>
    /// Task to look for a host on the network, tokens to cancel tasks.
    /// </summary>
    private Task setupHostIp;
    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;
    

    /// <summary>
    /// Keeps info about current swap(Null, Waiting, Accepted)
    /// </summary>
    private SwapInfo swapInfo;

    /// <summary>
    /// Client behaviour constructor to set up basic settings for having a client behaviour in the game.
    /// </summary>
    /// <param name="myMonoBehaviour"></param>
    public ClientBehaviour()
    {
        setupHostIp = null;

        swapInfo = SwapInfo.Null;
        /// Set values to default.
        connect = false;
        m_ClientDriver = new UdpCNetworkDriver(new INetworkParameter[0]);
        m_clientToServerConnection = default;
        m_clientWantsConnection = false;
    }

    
    /// <summary>
    /// Destructor for client behaviour, makes sure that important data is cleaned up.
    /// </summary>
    ~ClientBehaviour()
    {
        cancellationTokenSource.Dispose();

        m_ClientDriver.ScheduleUpdate().Complete();
        m_clientToServerConnection.Close(m_ClientDriver);
        m_clientToServerConnection = default;
        m_ClientDriver = default;
        //m_ClientDriver.Dispose();
    }

    public enum SwapInfo{
        Null,
        Waiting,
        Accepted
    }

    public SwapInfo GetSwapInfo()
    {
        return swapInfo;
    }

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
    /// Tries to connect to a computer with the ip given as parameter. If possible, global variable 'ServerEndPoint' will be set.
    /// </summary>
    /// <param name="ip"></param>
    public void FindHost(string ip)
    {
        string message = "Connecting";
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                /// Cancel task.
                cancellationToken.ThrowIfCancellationRequested();
            }
            /// Create a TcpClient.
            Int32 port = 13000;
            TcpClient client = new TcpClient(ip, port);

            /// Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            
            /// Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();

            /// Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length);
            
            /// Buffer to store the response bytes.
            data = new Byte[256];
            
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            // Close everything.
            stream.Close();
            client.Close();
            ServerEndPoint = new IPEndPoint(IPAddress.Parse(ip), 9000);
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("ArgumentNullException:" + e);
        }
        catch (SocketException e)
        {
            /// Client will get socket exception when trying to ask other computers if they are hosting or not, and they don't answer yes.
            /// Debug.Log("SocketException:" + e);
        }
        catch (OperationCanceledException e)
        {
            Debug.Log("Canceled task: " + e);
        }
        finally
        {
            cancellationTokenSource.Dispose();
        }
    }

    /// <summary>
    /// If the client is looking for host but not finding one, the client can still stop the connecting.
    /// </summary>
    public void StopConnecting()
    {
        if (setupHostIp == null)
        {
            return;
        }
        /// Cancel task.
        if (!setupHostIp.IsCompleted && !setupHostIp.IsCanceled)
        {
            Debug.Log("Cancelling connection request");
            cancellationTokenSource.Cancel();
        }
    }
    
    /// <summary>
    /// Disconnect is run when client wants to disconnect, the view will be changed to prelobby-view; Values will be reset.
    /// </summary>
    /// <param name="messageTexts"></param>
    public void Disconnect(List<GameObject> messageTexts)
    {

        Debug.Log("Disconnecting");


        // Disconnect client from host:
        m_clientToServerConnection.Disconnect(m_ClientDriver);
        ServerEndPoint = default;
        m_clientToServerConnection = default;
        m_ClientDriver.Dispose();


        /// Change connectionText according to connection status:
        GameObject.Find("ConnectionText").GetComponent<Text>().text = "Offline";
        GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.red;

        /// Set the UI for the user correctly according to the connection status:
        GameObject gm = GameObject.Find("GameManager");
        gm.GetComponent<NetworkingManager>().connectionField.SetActive(true);
        gm.GetComponent<NetworkingManager>().chatField.SetActive(false);
    }

    /// <summary>
    /// Function run from NetworkingManager when player wants to find and connect to a host/server.
    /// </summary>
    /// <param name="myMonoBehaviour"></param>
    public void Connect(MonoBehaviour myMonoBehaviour)
    {
        if (m_clientWantsConnection != true)
        {
            m_clientWantsConnection = true;
            myMonoBehaviour.StartCoroutine(WaitForHost());
        }
    }

    /// <summary>
    /// Returns a list of IPs as strings on current local network.
    /// </summary>
    /// <returns></returns>
    private List<string> FindIPs()
    {
        List<string> results = new List<string>();

        using (System.Diagnostics.Process p = new System.Diagnostics.Process())
        {
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.Arguments = "/c arp -a";
            p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
            p.Start();

            string line;

            while ((line = p.StandardOutput.ReadLine()) != null)
            {
                if (line != "" && !line.Contains("Interface") && !line.Contains("Physical Address"))
                {
                    var lineArr = line.Trim().Split(' ').Select(n => n).Where(n => !string.IsNullOrEmpty(n)).ToArray();
                    var arrResult = lineArr[0];
                    results.Add(arrResult);
                }
            }

            p.WaitForExit();
        }

        /// Add ip address for own pc for testing purposes.
        results.Add(GetLocalIPAddress());
        return results;
    }

    IEnumerator WaitForHost()
    {
        /// Text visible to the client that shows the connection status.(Offline/Connecting/Online)
        Text connectionText = GameObject.Find("ConnectionText").GetComponent<Text>();
        Debug.Log("<color=blue>Looking for connection</color>");
        connectionText.text = "Connecting";


        /// Set token for connections.
        cancellationTokenSource = new CancellationTokenSource();
        cancellationToken = cancellationTokenSource.Token;

        /// Setup ips for connections.
        List<string> ips = FindIPs();
        setupHostIp = Task.Factory.StartNew(() => FindHost(ips[0]), cancellationToken);

        /// Go through possible ips.
        for (int i = 0; i < ips.Count; i++)
        {
            /// Stop connecting if user wants to.
            if (cancellationToken.IsCancellationRequested || cancellationTokenSource.IsCancellationRequested || cancellationTokenSource == null)
            {
                break;
            }

            /// Cosmetics:
            if (i%4 == 0)
            {
                connectionText.text = "Connecting";
                //yield return new WaitForSeconds(0.1f);
            }
            connectionText.text = connectionText.text + ".";
            
            /// Set token for connections.
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            /// Get IP of host that wants to serve a match
            setupHostIp.ContinueWith((t) => FindHost(ips[i]), cancellationToken);
            //setupHostIp = Task.Run(() => FindHost(ips[i]));


            yield return new WaitForSeconds(0.25f);
            if (ServerEndPoint.IsValid)
            {
                /// Cancel task.
                if (!setupHostIp.IsCompleted && !setupHostIp.IsCanceled)
                {
                    cancellationTokenSource.Cancel();
                }

                break;
            }
        }
        if (!ServerEndPoint.IsValid)
        {
            GameObject.Find("GameManager").GetComponent<NetworkingManager>().StopConnecting();

            Debug.Log("Could not find a valid host");
        }

        GameObject.Find("GameManager").GetComponent<NetworkingManager>().joinButton.SetActive(true);
        GameObject.Find("GameManager").GetComponent<NetworkingManager>().stopJoinButton.SetActive(false);

        try
        {
            /// Connect to host if one is found and connection isn't already made.
            if (!m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid)
            {
                
                /// Try to connect to the ServerEndPoint.
                m_clientToServerConnection = m_ClientDriver.Connect(ServerEndPoint);

                /// Change UI to show client that they are connected, and stop the coroutine that shows a connecting version of the UI.
                connect = true;
                connectionText.text = "Online";
                connectionText.color = Color.green;

                GameObject gm = GameObject.Find("GameManager");
                gm.GetComponent<NetworkingManager>().chatField.SetActive(true);
                gm.GetComponent<NetworkingManager>().connectionField.SetActive(false);

                /// Client has found a connection, so won't need to look for a new one:
                m_clientWantsConnection = false;

                /// Remove task:
                setupHostIp.Dispose();
                /// Finish this instance of coroutines:
                yield break;
            }
        }
        catch
        {
            Debug.Log("Could not find a host!");
        }

        /// Change text to Offline if the connection failed/was closed.
        if (!connect)
        {
            connectionText.text = "Offline";
        }
    }


    /// <summary>
    /// Client sends message to other client about accepting swap.
    /// </summary>
    public void AcceptSwap(string name)
    {
        swapInfo = SwapInfo.Accepted;
        if (m_clientToServerConnection.IsCreated)
        {
            string msg = name + "<AcceptSwap>";
            var classWriter = new DataStreamWriter(msg.Length + 2, Allocator.Temp);

            // Setting prefix for server to easily know what kind of msg is being written.
            byte[] message = Encoding.ASCII.GetBytes(msg);
            classWriter.Write(message);
            m_ClientDriver.Send(m_clientToServerConnection, classWriter);
            classWriter.Dispose();
        }
    }

    /// <summary>
    /// Client sends message to other client about declining swap.
    /// </summary>
    public void DeclineSwap(string name)
    {
        swapInfo = SwapInfo.Null;
        if (m_clientToServerConnection.IsCreated)
        {
            string msg = name + "<DeclineSwap>";
            var classWriter = new DataStreamWriter(msg.Length + 2, Allocator.Temp);

            // Setting prefix for server to easily know what kind of msg is being written.
            byte[] message = Encoding.ASCII.GetBytes(msg);
            classWriter.Write(message);
            m_ClientDriver.Send(m_clientToServerConnection, classWriter);
            classWriter.Dispose();
        }
    }
    
    /// <summary>
    /// Sends message to client with to and from names within message.
    /// </summary>
    public void SendSwapMessage(string msg)
    {
        swapInfo = SwapInfo.Waiting;
        Debug.Log("sending swap message");
        if (m_clientToServerConnection.IsCreated)
        {
            // Setting prefix for server to easily know what kind of msg is being written.
            msg += "<SwapMessage>";
            var classWriter = new DataStreamWriter(msg.Length + 2, Allocator.Temp);

            byte[] message = Encoding.ASCII.GetBytes(msg);
            classWriter.Write(message);
            m_ClientDriver.Send(m_clientToServerConnection, classWriter);
            classWriter.Dispose();
        }
    }

    /// <summary>
    /// SendMessage will send message about actions happening on the current client, and server will give information about this to other clients.
    /// </summary>
    /// <param name="message"></param>
    public void SendMessage(dynamic msg)
    {
        if (m_clientToServerConnection.IsCreated)
        {
            string messageObj = JsonUtility.ToJson(msg) + "<Message>" + msg.GetType().ToString();
            var classWriter = new DataStreamWriter(messageObj.Length + 2, Allocator.Temp);
            
            byte[] message = Encoding.ASCII.GetBytes(messageObj);
            classWriter.Write(message);
            m_ClientDriver.Send(m_clientToServerConnection, classWriter);
            classWriter.Dispose();
        }
    }
    
    /// <summary>
    /// SendChatMessage is used to send chat messages through the network.
    /// </summary>
    /// <param name="message"></param>
    public void SendChatMessage(string message)
    {
        if (m_clientToServerConnection.IsCreated)
        {
            // Example of sending a string through the network:
            var messageWriter = new DataStreamWriter(100, Allocator.Temp);
            // Setting prefix for server to easily know what kind of msg is being written.
            message += "<ChatMessage>";
            byte[] msg = Encoding.ASCII.GetBytes(message);
            messageWriter.Write(msg);
            m_ClientDriver.Send(m_clientToServerConnection, messageWriter);
            messageWriter.Dispose();
        }
    }

    /// <summary>
    /// Example of sending a class object through the network.
    /// </summary>
    public void SendScenario()
    {
        if (m_clientToServerConnection.IsCreated)
        {
            Scenario obj = new Scenario
            {
                playerName = "Chris",
                timeElapsed = 3.14f,
                level = 5,
                position = new Vector2(5, 3),
                values = new int[] { 5, 3, 6, 8, 2 }
            };

            Debug.Log("Client - Sending Scenario.");
            // Maybe change this to <Message> later on, as simple string messages won't be sent over the network then.
            string classObj = JsonUtility.ToJson(obj) + "<Scenario>";
            var classWriter = new DataStreamWriter(classObj.Length + 2, Allocator.Temp);
            // Setting prefix for server to easily know what kind of msg is being written.
            byte[] msg = Encoding.ASCII.GetBytes(classObj);
            classWriter.Write(msg);
            m_ClientDriver.Send(m_clientToServerConnection, classWriter);
            classWriter.Dispose();
        }
    }

    /// <summary>
    /// Test of sending scenario through the network, works fine. TODO delete later.
    /// </summary>
    public class Scenario
    {
        public int level;
        public float timeElapsed;
        public string playerName;
        public Vector2 position;
        public int[] values;
    }

    /// <summary>
    /// FixedUpdate is a function called ca. 50 times per second. Used to see for events through the network.
    /// If any messages is sent in, and will according to event happened send message through the network.
    /// </summary>
    public void FixedUpdate( MonoBehaviour monoBehaviour)
    {
        if(!m_ClientDriver.IsCreated)
        {
            return;
        }
        /// Update the NetworkDriver. Needs to be done before trying to pop events.
        m_ClientDriver.ScheduleUpdate().Complete(); 

        /// Update clients connecion to the server:
        if (m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid && m_clientWantsConnection == false)
        {
            var updateWriter = new DataStreamWriter(32, Allocator.Temp);
            // Setting prefix for server to easily know what kind of msg is being written.
            string message = "<UpdateConnection>";
            updateWriter.Write(Encoding.ASCII.GetBytes(message));
            m_ClientDriver.Send(m_clientToServerConnection, updateWriter);
            updateWriter.Dispose();
        }
           

        NetworkEvent.Type cmd;
        /// Process all events on the connection. If the connection is invalid it will return Empty immediately
        while ((cmd = m_clientToServerConnection.PopEvent(m_ClientDriver, out DataStreamReader strm)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                // Create a 4 byte data stream which we can store our ping sequence number in
                var connectionWriter = new DataStreamWriter(100, Allocator.Temp);
                string name = GameObject.Find("GameManager").GetComponent<NetworkingManager>().userName;
                byte[] msg = Encoding.ASCII.GetBytes(name + "<Connecting>");
                connectionWriter.Write(msg);
                m_clientToServerConnection.Send(m_ClientDriver, connectionWriter);

                connectionWriter.Dispose();
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                // If data received says that client is connected to server, we are connected!
                var readerCtx = default(DataStreamReader.Context);
                /// Read message sent from client and respond to that:
                byte[] bytes = strm.ReadBytesAsArray(ref readerCtx, strm.Length);
                string data = Encoding.ASCII.GetString(bytes);
                
                if (data.Contains("<Connected>"))
                {
                    /// A client just connected.
                    
                    NetworkingManager nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();

                    /// Receives hostname:
                    nm.hostText.GetComponent<Text>().text = "Host: " + data.Substring(0, data.IndexOf("<HostName>"));
                    data = data.Substring(data.IndexOf("<HostName>") + 10, data.Length - (data.IndexOf("<HostName>") + 10));
                    

                    /// Receives names of other people in attackerlist(if any).
                    int i = 0;
                    while (data.Contains("<AttackerName>"))
                    {
                        nm.attackerNames[i].SetActive(true);
                        nm.attackerNames[i].transform.Find("Text").GetComponent<Text>().text = data.Substring(0, data.IndexOf("<AttackerName>"));

                        int offsetPosition = data.IndexOf("<AttackerName>") + 14;
                        data = data.Substring(offsetPosition, data.Length - offsetPosition);
                        i++;
                    }

                    /// Receives names of other people in defenderlist(if any).
                    i = 0;
                    while (data.Contains("<DefenderName>"))
                    {
                        nm.defenderNames[i].SetActive(true);
                        nm.defenderNames[i].transform.Find("Text").GetComponent<Text>().text = data.Substring(0, data.IndexOf("<DefenderName>"));

                        int offsetPosition = data.IndexOf("<DefenderName>") + 14;
                        data = data.Substring(offsetPosition, data.Length - offsetPosition);
                        i++;
                    }

                    /// Receive what playerType the client is set as; May be changed by client itself after it has been set once.
                    nm.playerType = nm.FindPlayerType();

                    Debug.Log("Client - You are connected to server!");
                }
                else if (data.Contains("<Message>"))
                {
                    /// Convert data to message of type Message:
                    var type = data.Substring(data.LastIndexOf("<Message>") + 9);
                    Type typed = Type.GetType(type);

                    data = data.Substring(0, data.IndexOf("<Message>"));

                    dynamic msg = JsonUtility.FromJson(data, typed);
                    
                    /// Send message to the chat field.
                    GameObject.Find("GameManager").GetComponent<NetworkingManager>().GetMessage((Message)msg);
                }
                else if (data.Contains("<MessageReply>"))
                {
                    data = data.Substring(0, data.Length - 14);

                    /// Send message to the chat field.
                    GameObject.Find("GameManager").GetComponent<NetworkingManager>().GetChatMessage(data);
                }
                else if (data.Contains("<DeclineSwap>"))
                {
                    GameObject.Find("GameManager").GetComponent<NetworkingManager>().DestroySwap();
                    swapInfo = SwapInfo.Null;
                }
                else if (data.Contains("<AcceptSwap>"))
                {
                    /// If first client has not declined while second client accepted, start the swapping.
                    if (swapInfo == SwapInfo.Waiting)
                    {
                        /// Send message to client nr.2 to do the swap.
                        string fromName = data.Substring(0, data.IndexOf("<AcceptSwap>"));
                        string msg = fromName + "<SwapAccepted>";
                        var classWriter = new DataStreamWriter(msg.Length + 2, Allocator.Temp);

                        byte[] message = Encoding.ASCII.GetBytes(msg);
                        classWriter.Write(message);
                        m_ClientDriver.Send(m_clientToServerConnection, classWriter);
                        classWriter.Dispose();

                        swapInfo = SwapInfo.Null;
                        /// Swap the two clients on client nr.1's screen.
                        string toName = GameObject.Find("GameManager").GetComponent<NetworkingManager>().userName;
                        GameObject.Find("GameManager").GetComponent<NetworkingManager>().FindSwapNames(fromName, toName);

                        GameObject.Find("GameManager").GetComponent<NetworkingManager>().DestroySwap();
                    }
                    else
                    {
                        /// Just to be sure.
                        swapInfo = SwapInfo.Null;

                        /// Client has most likely declined, so send decline message back:
                        string name = data.Substring(0, data.IndexOf("<AcceptSwap>"));
                        string msg = name + "<DeclineSwap>";
                        var classWriter = new DataStreamWriter(msg.Length + 2, Allocator.Temp);

                        byte[] message = Encoding.ASCII.GetBytes(msg);
                        classWriter.Write(message);
                        m_ClientDriver.Send(m_clientToServerConnection, classWriter);
                        classWriter.Dispose();
                    }
                }
                else if (data.Contains("<SwapAccepted>"))
                {
                    Debug.Log("swapAccepted");
                    /// Should not really be neccessary, since this is run when client has already accepted. But nice to have :)
                    if (swapInfo == SwapInfo.Accepted)
                    {
                        swapInfo = SwapInfo.Null;
                        /// Swap the two clients on client nr.2's screen.
                        string fromName = data.Substring(0, data.IndexOf("<SwapAccepted>"));
                        string toName = GameObject.Find("GameManager").GetComponent<NetworkingManager>().userName;
                        GameObject.Find("GameManager").GetComponent<NetworkingManager>().FindSwapNames(fromName, toName);

                        /// Finish up the swap:
                        GameObject.Find("GameManager").GetComponent<NetworkingManager>().DestroySwap();
                        swapInfo = SwapInfo.Null;
                    }
                }
                else if (data.Contains("<SwapMessage>"))
                {
                    NetworkingManager nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();
                    /// Start swap loading screen for client with name given from client that wants to swap.
                    string name = data.Substring(0, data.IndexOf("<SwapMessage>"));
                    monoBehaviour.StartCoroutine(nm.StartSwapLoadBar(name, false));
                }
                else if (data.Contains("<Scenario>"))
                {
                    data = data.Substring(0, data.Length - 10);
                    Debug.Log("Client - got scenario: " + data);
                }
                else if (data.Contains("<ClientConnect>"))///don't need this anymore? TODO
                {
                    NetworkingManager nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();
                    /// Add new client connected to list of players in lobby:
                    nm.AddPlayerName(data.Substring(0, data.IndexOf("<ClientConnect>")));
                }
                else if (data.Contains("<ClientDisconnect>"))
                {
                    string name = data.Substring(0, data.IndexOf("<ClientDisconnect>"));
                    /// Remove client that disconnected from list of people who ARE indeed connected.
                    Debug.Log("Client - removing other client");
                    GameObject.Find("GameManager").GetComponent<NetworkingManager>().FindPlayerForRemoval(name);
                }
                else if (data.Contains("<Disconnect>"))
                {
                    Debug.Log("Client - host wants me to disconnect!");

                    /// Get reference to chatfield for client to disconnect.
                    List<GameObject> messageTexts = new List<GameObject>();
                    Transform trans = GameObject.Find("MessageField").transform;
                    foreach (Transform child in trans)
                    {
                        if (child != trans)
                        {
                            messageTexts.Add(child.gameObject);
                        }
                    }
                    Disconnect(messageTexts);


                    /// If the server disconnected us we clear out connection
                    m_clientToServerConnection = default(NetworkConnection);
                }
                else if (data.Contains("<StartGame>"))
                {
                    StartGame();
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client - Disconnecting");

                
                /// Get reference to chatfield for client to disconnect.
                List<GameObject> messageTexts = new List<GameObject>();
                Transform trans = GameObject.Find("MessageField").transform;
                foreach (Transform child in trans)
                {
                    if (child != trans)
                    {
                        messageTexts.Add(child.gameObject);
                    }
                }
                Disconnect(messageTexts);
                

                /// If the server disconnected us we clear out connection
                m_clientToServerConnection = default(NetworkConnection);
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

    void StartGame()
    {
        GameObject.Find("Canvas").SetActive(false);

        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }
}
