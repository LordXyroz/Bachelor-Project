using System.Net;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine.UI;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class ClientBehaviour
{
    /// <summary>
    /// Debug.Log($"{DateTime.Now:HH:mm:ss.fff}: starting a new callback.");
    /// This is a way to see time right now, could be useful.
    /// </summary>

    ///  results contain the ipaddresses possible to connect to.
    readonly List<string[]> results = new List<string[]>();
    
    public static NetworkEndPoint ServerEndPoint { get; private set; }
    private bool connect;

    public UdpCNetworkDriver m_ClientDriver;
    public NetworkConnection m_clientToServerConnection;
    private bool m_clientWantsConnection;

    /// <summary>
    /// Client behaviour constructor to set up basic settings for having a client behaviour in the game.
    /// </summary>
    /// <param name="myMonoBehaviour"></param>
    public ClientBehaviour(MonoBehaviour myMonoBehaviour)
    {
        /// Makes sure that the client updates connection every now and then to not lose connection;
        /// Client will lose connection if pursuing actions giving that effect.
        myMonoBehaviour.StartCoroutine(UpdateConnection());

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
        m_ClientDriver.ScheduleUpdate().Complete();
        m_clientToServerConnection.Close(m_ClientDriver);
        m_clientToServerConnection = default;
        m_ClientDriver.Dispose();
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
            /// Create a TcpClient.
            Int32 port = 13000;
            TcpClient client = new TcpClient(ip, port);

            /// Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            
            /// Get a client stream for reading and writing.
            NetworkStream stream = client.GetStream();

            /// Send the message to the connected TcpServer. 
            //Debug.Log("newconnection - client sending message");
            stream.Write(data, 0, data.Length);
            
            /// Buffer to store the response bytes.
            data = new Byte[256];
            
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //Debug.Log("newconnection - client received: " + responseData);

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


    /// <summary>
    /// WaitForHost is a routine where the client searches for a host until one is found, or the client stops the connecting.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForHost()
    {
        /// Text visible to the client that shows the connection status.(Offline/Connecting/Online)
        Text connectionText = GameObject.Find("ConnectionText").GetComponent<Text>();
        Debug.Log("<color=blue>Looking for connection</color>");

        Debug.Log("m_clienttoserver Connection: " + m_clientToServerConnection.IsCreated);

        /// While the player still wants to connect to a host and no host is found, look for a new one:
        while (m_clientToServerConnection.IsCreated == false && m_clientWantsConnection)
        {

            /// Cosmetics... Makes it look like there is a connection trying to be done in the background.
            yield return new WaitForSeconds(1);
            connectionText.text = "Connecting.";
            yield return new WaitForSeconds(1);
            connectionText.text = "Connecting..";
            yield return new WaitForSeconds(0.15f);
            connectionText.text = "Connecting...";

            List<string> ips = FindIPs();
            
            /// Go through possible ips.
            for (int i = 0; i < ips.Count; i++)
            {
                /// Get IP of host that wants to serve a match.
                Task setupHostIp = Task.Run(() => FindHost(ips[i]));
                setupHostIp.Wait();
                if (ServerEndPoint.IsValid)
                {
                    break;
                }
            }
            
            try
            {
                /// Connect to host if one is found and connection isn't already made.
                if (!m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid)
                {
                    /// Try to connect to the ServerEndPoint.
                    m_clientToServerConnection = m_ClientDriver.Connect(ServerEndPoint);

                    /// When the client has connection there is no need to try and create a new, so we set that want to false.
                    m_clientWantsConnection = false;

                    /// Change UI to show client that they are connected, and stop the coroutine that shows a connecting version of the UI.
                    connect = true;
                    connectionText.text = "Online";
                    connectionText.color = Color.green;

                    GameObject gm = GameObject.Find("GameManager");
                    gm.GetComponent<NetworkingManager>().chatField.SetActive(true);
                    gm.GetComponent<NetworkingManager>().connectionField.SetActive(false);

                    /// Finish this instance of coroutines:
                    yield break;
                }
            }
            catch
            {
                Debug.Log("Could not find a host!");
            }
        }
        /// Change text to Offline if the connection failed/was closed.
        if (!connect)
        {
            connectionText.text = "Offline";
        }
    }

    /// <summary>
    /// UpdateConnection sends a update message to the server every 10 seconds to not lose the connection for being afk for a short period.
    /// </summary
    /// <returns></returns>
    IEnumerator UpdateConnection()
    {
        while (m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid)
        {
            var updateWriter = new DataStreamWriter(30, Allocator.Temp);
            // Setting prefix for server to easily know what kind of msg is being written.
            string message = "<UpdateConnection>";
            byte[] msg = Encoding.ASCII.GetBytes(message);
            updateWriter.Write(msg);
            m_ClientDriver.Send(m_clientToServerConnection, updateWriter);
            updateWriter.Dispose();
            yield return new WaitForSeconds(10);
        }
    }

    /// <summary>
    /// SendMessage is used to send simple strings through the network, will later on use the type 'Message' to send more data through the network.
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
    public void FixedUpdate()
    {
        // Update the NetworkDriver. Needs to be done before trying to pop events.
        m_ClientDriver.ScheduleUpdate().Complete();


        NetworkEvent.Type cmd;
        // Process all events on the connection. If the connection is invalid it will return Empty immediately
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
                    NetworkingManager nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();

                    /// Receives hostname:
                    Debug.Log("Names gotten from server: " + data);
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
                    while(data.Contains("<DefenderName>"))
                    {
                        nm.attackerNames[i].SetActive(true);
                        nm.attackerNames[i].transform.Find("Text").GetComponent<Text>().text = data.Substring(0, data.IndexOf("<DefenderName>"));

                        int offsetPosition = data.IndexOf("<DefenderName>") + 14;
                        data = data.Substring(offsetPosition, data.Length - offsetPosition);
                        i++;
                    }

                    /// Add your own name to list:
                    //nm.AddPlayerName(nm.userName);

                    Debug.Log("Client - You are connected to server!");
                }
                else if (data.Contains("<MessageReply>"))
                {
                    data = data.Substring(0, data.Length - 14);

                    /// Send message to the chat field.
                    GameObject.Find("GameManager").GetComponent<NetworkingManager>().GetMessage(data);
                }
                else if (data.Contains("<Scenario>"))
                {
                    data = data.Substring(0, data.Length - 10);
                    Debug.Log("Client - got scenario: " + data);
                }
                else if (data.Contains("<ClientConnect>"))
                {
                    NetworkingManager nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();
                    /// Add new client connected to list of players in lobby:
                    nm.AddPlayerName(data.Substring(0, data.IndexOf("<ClientConnect>")));
                }
                else if (data.Contains("<ClientDisconnect>"))
                {
                    string name = data.Substring(0, data.IndexOf("<ClientDisconnect>"));
                    /// Remove client that disconnected from list of people who ARE indeed connected.
                    Debug.Log("removing other client");
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
}
