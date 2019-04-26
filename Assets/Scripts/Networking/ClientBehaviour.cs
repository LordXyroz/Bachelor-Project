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

using MessagingInterfaces;

public class ClientBehaviour : IPing, IConnection, IChatMessage, ISwap, IDisconnectClient, IDisconect, IStartGame, IDisposable, ISaveFile
{
    private NetworkingManager nm;

    ///  results contain the ipaddresses possible to connect to.
    readonly List<string[]> results = new List<string[]>();

    public static NetworkEndPoint ServerEndPoint { get; private set; }
    public string ServerName;
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
        nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();
        setupHostIp = null;
        ServerName = "";

        swapInfo = SwapInfo.Null;
        /// Set values to default.
        connect = false;
        m_ClientDriver = new UdpCNetworkDriver(new INetworkParameter[0]);

        m_clientToServerConnection = default;
        m_clientWantsConnection = false;
    }
    
    public enum SwapInfo{
        Null,
        Waiting,
        Accepted
    }

    public enum SwapMsgType
    {
        Request,
        Accept,
        Decline,
        Acknowledge
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
    /// <param name="serverName"></param>
    public void FindHost(string ip, out string serverName)
    {
        serverName = "";

        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];

        // Connect to a remote device.  
        try
        {
            if (cancellationToken.IsCancellationRequested)
            {
                /// Cancel task.
                cancellationToken.ThrowIfCancellationRequested();
            }
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localAddr = null;
            for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
            {
                if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    localAddr = ipHostInfo.AddressList[i];
                    break;
                }
            }
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), 11000);

            // Create a TCP/IP  socket.  
            Socket client = new Socket(IPAddress.Parse(ip).AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                client.Connect(remoteEP);

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes("Looking for lobbies<EOF>");

                // Send the data through the socket.  
                int bytesSent = client.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = client.Receive(bytes);
                serverName = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                if (serverName != "" && serverName != null)
                {
                    ServerEndPoint = new IPEndPoint(IPAddress.Parse(ip), 9000);
                }

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (ArgumentNullException ane)
            {
                Debug.Log(ane);
            }
            catch (SocketException se)
            {
                Debug.Log(se);
            }
            catch (OperationCanceledException oce)
            {
                Debug.Log("Canceled task: " + oce);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    
    /// <summary>
    /// If the client is looking for host but not finding one, the client can still stop the connecting.
    /// </summary>
    public void StopConnecting(MonoBehaviour myMonoBehaviour)
    {
        if (setupHostIp == null)
        {
            return;
        }
        myMonoBehaviour.StopAllCoroutines();

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
    public void Disconnect()
    {
        Debug.Log("Disconnecting");
        
        // Disconnect client from host:
        m_clientToServerConnection.Disconnect(m_ClientDriver);
        m_ClientDriver.Disconnect(m_clientToServerConnection);

        results.Clear();

        /// Change connectionText according to connection status:
        GameObject.Find("ConnectionText").GetComponent<Text>().text = "Offline";
        GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.red;

        /// Set the UI for the user correctly according to the connection status:
        GameObject gm = GameObject.Find("GameManager");
        gm.GetComponent<NetworkingManager>().connectionField.SetActive(true);
        gm.GetComponent<NetworkingManager>().chatField.SetActive(false);

        foreach (Transform t in nm.lobbyScrollField.transform.Find("ButtonListViewport").Find("ButtonListContent").transform)
        {
            GameObject.Destroy(t);
        }
    }

    /// <summary>
    /// Function run from NetworkingManager when player wants to find and connect to a host/server.
    /// </summary>
    /// <param name="myMonoBehaviour"></param>
    public void FindHosts(MonoBehaviour myMonoBehaviour)
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

    private IEnumerator WaitForHost()
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
        var serverName = "";

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
            }
            connectionText.text = connectionText.text + ".";
            
            /// Set token for connections.
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            

            /// Get IP of host that wants to serve a match
            setupHostIp = Task.Factory.StartNew(() => FindHost(ips[i], out serverName), cancellationToken);
            
            yield return new WaitForSeconds(0.25f);


            if (ServerEndPoint.IsValid)
            {
                GameObject lobby = MonoBehaviour.Instantiate(nm.lobbyButton, nm.lobbyScrollField.transform.Find("ButtonListViewport").Find("ButtonListContent").transform);
                lobby.GetComponent<Button>().transform.Find("Text").GetComponent<Text>().text = serverName;
                lobby.name = ServerEndPoint.GetIp();

                lobby.GetComponent<Button>().onClick.AddListener(() => Connect(lobby.name));
                
                /// Cancel task.
                if (!setupHostIp.IsCompleted && !setupHostIp.IsCanceled)
                {
                    cancellationTokenSource.Cancel();
                }
            }
        }
        
        if (!ServerEndPoint.IsValid)
        {
            //nm.StopConnecting();
            Debug.Log("Could not find a valid host");
            GameObject.Find("ConnectionText").GetComponent<Text>().text = "Found no hosts";


            Dispose();
        }
        else
        {
            GameObject.Find("ConnectionText").GetComponent<Text>().text = "Found host(s)";
        }

        nm.joinButton.SetActive(true);
        nm.stopJoinButton.SetActive(false);
    }

    public void Connect(string name)
    {
        Text connectionText = GameObject.Find("ConnectionText").GetComponent<Text>();
        GameObject lobby = nm.lobbyScrollField.transform.Find("ButtonListViewport").Find("ButtonListContent").Find(name).gameObject;

        ServerName = lobby.GetComponent<Button>().transform.Find("Text").GetComponent<Text>().text;
        ServerEndPoint = new IPEndPoint(IPAddress.Parse(lobby.name), 9000);
        
        Debug.Log("lobbyname: " + lobby.name);
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
                
                nm.chatField.SetActive(true);
                nm.lobbyScrollField.SetActive(false);
                nm.lobbyScrollFrame.SetActive(false);

                nm.matchmakingObject.SetActive(false);

                nm.connectionField.SetActive(false);
                GameObject.Find("HostText").GetComponent<Text>().text = ServerName;


                /// Client has found a connection, so won't need to look for a new one:
                m_clientWantsConnection = false;

                /// Remove task:
                setupHostIp.Dispose();

                /// Remove the list of lobbies to join as you have joined one.
                nm.RemoveLobbies();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
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

        SendMessage(new SwapMessage(name, nm.userName, MessageTypes.Network.Swap, SwapMsgType.Accept));
    }

    /// <summary>
    /// Client sends message to other client about declining swap.
    /// </summary>
    public void DeclineSwap(string name)
    {
        swapInfo = SwapInfo.Null;

        SendMessage(new SwapMessage(name, nm.userName, MessageTypes.Network.Swap, SwapMsgType.Decline));
    }
    
    /// <summary>
    /// Sends message to client with to and from names within message.
    /// </summary>
    public void SendSwapMessage(string target)
    {
        swapInfo = SwapInfo.Waiting;

        SendMessage(new SwapMessage(target, nm.userName, MessageTypes.Network.Swap, SwapMsgType.Request));
    }

    /// <summary>
    /// SendMessage will send message about actions happening on the current client, and server will give information about this to other clients.
    /// </summary>
    /// <param name="message"></param>
    public void SendMessage(dynamic msg)
    {
        if (m_clientToServerConnection.IsCreated)
        {
            var str = JsonUtility.ToJson(msg);
            str = str + "|" + msg.GetType();
            
            var writer = new DataStreamWriter(str.Length, Allocator.Temp);

            writer.Write(Encoding.ASCII.GetBytes(str));
            m_ClientDriver.Send(m_clientToServerConnection, writer);
            writer.Dispose();
        }
    }

    /// <summary>
    /// Example of sending a class object through the network.
    /// </summary>
    public void SendScenario()
    {
        if (m_clientToServerConnection.IsCreated)
        {
            // Todo: scenario stuff
        }
    }
    
    /// <summary>
    /// FixedUpdate is a function called ca. 20 times per second. Used to see for events through the network.
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
        if (m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid && m_clientWantsConnection == false && m_ClientDriver.IsCreated)
        {
            try
            { 
                SendMessage(new Message("Server", nm.userName, MessageTypes.Network.Ping));
            }
            catch(InvalidOperationException e)
            {
                Debug.Log("Could not update connection when not set yet: " + e);
            }
        }

        NetworkEvent.Type cmd;
        /// Process all events on the connection. If the connection is invalid it will return Empty immediately
        try
        {
            while ((cmd = m_clientToServerConnection.PopEvent(m_ClientDriver, out DataStreamReader strm)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    SendMessage(new ConnectMessage("Server", nm.userName, MessageTypes.Network.Connect, "", "", ""));
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    // If data received says that client is connected to server, we are connected!
                    var readerCtx = default(DataStreamReader.Context);
                    /// Read message sent from client and respond to that:
                    byte[] bytes = strm.ReadBytesAsArray(ref readerCtx, strm.Length);
                    string data = Encoding.ASCII.GetString(bytes);

                    if (data.Contains("<Scenario>"))
                    {
                        data = data.Substring(0, data.Length - 10);
                        Debug.Log("Client - got scenario: " + data);
                    }
                    else if (data.Contains("|"))
                    {
                        /// This is using message

                        var str = data.Split('|');
                        var type = Type.GetType(str[1]);
                        dynamic msg = JsonUtility.FromJson(str[0], type);

                        MessagingManager.BroadcastMessage((Message)msg);
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client - Disconnecting");

                    Disconnect();
                }
            }
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e.Message + ", Name: " + e.TargetSite.Name);
        }
    }

    /// <summary>
    /// Moves every text in chatField one step up for the new message to be put at bottom.
    /// </summary>
    private void MoveChatBox()
    {
        GameObject.Find("MessageText5").GetComponent<Text>().text = GameObject.Find("MessageText4").GetComponent<Text>().text;
        GameObject.Find("MessageText4").GetComponent<Text>().text = GameObject.Find("MessageText3").GetComponent<Text>().text;
        GameObject.Find("MessageText3").GetComponent<Text>().text = GameObject.Find("MessageText2").GetComponent<Text>().text;
        GameObject.Find("MessageText2").GetComponent<Text>().text = GameObject.Find("MessageText1").GetComponent<Text>().text;
    }

    #region Interface functions
    public void OnPing(Message message, int index)
    {
        // Debug.Log("Ping ack!");
        // throw new NotImplementedException();
    }

    public void OnConnection(ConnectMessage message, int index)
    {
        nm.hostText.GetComponent<Text>().text = message.senderName;

        if (message.attackerName != "")
        {
            nm.attackerNames[0].SetActive(true);
            nm.attackerNames[0].transform.Find("Text").GetComponent<Text>().text = message.attackerName;
        }
        
        if (message.defenderName != "")
        {
            nm.defenderNames[0].SetActive(true);
            nm.defenderNames[0].transform.Find("Text").GetComponent<Text>().text = message.defenderName;
        }

        nm.playerType = nm.FindPlayerType();
        Debug.Log("OnConnection - You are connected to server! ip: " + ServerEndPoint.GetIp());
    }

    public void OnChatMessage(ChatMessage message)
    {
        nm.GetChatMessage(message.message);
    }

    public void OnSwap(SwapMessage message)
    {
        if (message.swapMsg == SwapMsgType.Request)
        {
            /// Start swap loading screen for client with name given from client that wants to swap.
            string name = message.senderName;
            nm.StartCoroutine(nm.StartSwapLoadBar(name, false));
        }
        else if (message.swapMsg == SwapMsgType.Accept)
        {
            /// If first client has not declined while second client accepted, start the swapping.
            if (swapInfo == SwapInfo.Waiting)
            {
                SendMessage(new SwapMessage(message.senderName, message.targetName, MessageTypes.Network.Swap, SwapMsgType.Acknowledge));

                swapInfo = SwapInfo.Null;
                nm.FindSwapNames(message.senderName, message.targetName);
                nm.DestroySwap();
            }
            else
            {
                swapInfo = SwapInfo.Null;
                SendMessage(new SwapMessage(message.senderName, message.targetName, MessageTypes.Network.Swap, SwapMsgType.Decline));
            }
        }
        else if (message.swapMsg == SwapMsgType.Decline)
        {
            swapInfo = SwapInfo.Null;
            nm.DestroySwap();
        }
        else
        {
            nm.FindSwapNames(message.senderName, message.targetName);

            /// Finish up the swap:
            nm.DestroySwap();
            swapInfo = SwapInfo.Null;
        }
    }

    public void OnClientDisconnect(DiscClientMessage message)
    {
        if (nm.inGame)
        {
            SceneManager.UnloadSceneAsync("GameScene");
            nm.matchmakingCanvas.SetActive(true);
            nm.inGame = false;
        }

        nm.FindPlayerForRemoval(message.disconnectingClient);
    }

    public void OnDisconnect()
    {
        if (nm.inGame)
        {
            SceneManager.UnloadSceneAsync("GameScene");
            nm.matchmakingCanvas.SetActive(true);
            nm.inGame = false;
        }

        nm.DisconnectFromServer();
    }

    public void OnStartGame()
    {
        GameObject.Find("Canvas").SetActive(false);

        nm.inGame = true;
        GameObject.FindObjectOfType<AudioManager>().PlayGameplayBGM(true, 0.05f);

        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }

    public void OnSaveFile(SaveFileMessage message)
    {
        Debug.Log("Json string of file: \n" + message.fileString);
        nm.saveFile = JsonUtility.FromJson<Save>(message.fileString);
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {

            }
            Debug.Log("Running client Dispose");

            m_ClientDriver.ScheduleUpdate().Complete();
            //m_clientToServerConnection.Disconnect(m_ClientDriver);
            m_clientToServerConnection = default;
            m_ClientDriver.Dispose();
            m_ClientDriver = default;
            
            ServerEndPoint = default;

            try
            {
                setupHostIp.Wait();
                setupHostIp.Dispose();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            disposedValue = true;

            if (nm.inGame)
            {
                GameObject.FindObjectOfType<AudioManager>().PlayMenuBGM(true, 0.05f);
                SceneManager.UnloadSceneAsync("GameScene");
                nm.matchmakingCanvas.SetActive(true);
                nm.inGame = false;
            }

            nm.cb = null;
        }
    }
    
    ~ClientBehaviour()
    {
      Dispose(false);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
    #endregion
}
