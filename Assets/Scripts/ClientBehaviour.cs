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

public class ClientBehaviour : MonoBehaviour
{
    /// <summary>
    /// Debug.Log($"{DateTime.Now:HH:mm:ss.fff}: starting a new callback.");
    /// This is a way to see time right now, could be useful.
    /// </summary>

    // Testing to get ipaddresses
    List<string[]> results = new List<string[]>();


    // Used to check if client is trying to start to play a game, no matter if host or not.
    public static NetworkEndPoint ServerEndPoint { get; private set; }
    private bool connect;

    private UdpCNetworkDriver m_ClientDriver;
    private NetworkConnection m_clientToServerConnection;

    
    public void FindHost()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];
        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.  
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                sender.Connect(remoteEP);

                //Debug.Log("Clientside - Socket connected to" + sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes("I want your ip address<EOF>");

                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = sender.Receive(bytes);
                //Debug.Log("Clientside(socket) - Echoed test = " + Encoding.ASCII.GetString(bytes, 0, bytesRec));
                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

                // Setup server connection and try to connect:
                string hostIp = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                ServerEndPoint = new IPEndPoint(IPAddress.Parse(hostIp), 9000);

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException: ", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException: ", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception: ", e.ToString());
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    
    void Start()
    {
        StartCoroutine(UpdateConnection());
        connect = false;

        m_ClientDriver = new UdpCNetworkDriver(new INetworkParameter[0]);
        m_clientToServerConnection = default;
    }

    /// <summary>
    /// Dispose stuff when not needed anymore.
    /// </summary>
    void OnDestroy()
    {
        m_ClientDriver.Dispose();
    }

    /// <summary>
    /// A button runs this command to either connect or disconnect from the server.
    /// </summary>
    public void ConnectOrDisconnect()
    {
        if (connect)
        { 
            connect = false;

            // Disconnect client from host:
            m_clientToServerConnection.Disconnect(m_ClientDriver);
            m_clientToServerConnection = default;
            ServerEndPoint = default;

            /// Change connectionText:
            GameObject.Find("ConnectionText").GetComponent<Text>().text = "Offline";
            GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.red;
        }
        else
        {

            /// Could use thread, but testing showed that Task was more efficient to use in this scenario. Easy to wait for the task to finish before doing something else aswell.
            Task setupHostIp = Task.Run(FindHost);
            setupHostIp.Wait();
            try
            {
                /// Connect to host if one is found and connection isn't already made.
                if (!m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid)
                {
                    m_clientToServerConnection = m_ClientDriver.Connect(ServerEndPoint);

                    /// Change UI to show client that they are connected:
                    connect = true;
                    GameObject.Find("ConnectionText").GetComponent<Text>().text = "Online";
                    GameObject.Find("ConnectionText").GetComponent<Text>().color = Color.green;
                }
            }
            catch(Exception e)
            {
                Debug.Log("Could not find a host!");
            }
        }
    }

    /// <summary>
    /// UpdateConnection sends a update message to the server every 10 seconds to not lose the connection for being afk for a short period.
    /// </summary
    /// <returns></returns>
    IEnumerator UpdateConnection()
    {
        while (true)
        {
            if (m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid)
            {
                var updateWriter = new DataStreamWriter(30, Allocator.Temp);
                // Setting prefix for server to easily know what kind of msg is being written.
                string message = "<UpdateConnection>";
                byte[] msg = Encoding.ASCII.GetBytes(message);
                updateWriter.Write(msg);
                m_ClientDriver.Send(m_clientToServerConnection, updateWriter);
                updateWriter.Dispose();
            }
            yield return new WaitForSeconds(10);
        }
    }

    /// <summary>
    /// SendMessage is used to send simple strings through the network, will later on use the type 'Message' to send more data through the network.
    /// </summary>
    /// <param name="message"></param>
    public new void SendChatMessage(string message)
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

    /// <summary>
    /// Example of sending a class object through the network.
    /// </summary>
    public void SendScenario()
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

    public class Scenario
    {
        public int level;
        public float timeElapsed;
        public string playerName;
        public Vector2 position;
        public int[] values;
    }

    void FixedUpdate()
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
                byte[] msg = Encoding.ASCII.GetBytes("<Connecting>");
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
                    Debug.Log("Client - You are connected to server!");
                }
                else if (data.Contains("<MessageReply>"))
                {
                    data = data.Substring(0, data.Length - 14);

                    //Debug.Log("Client - Got message: " + data);
                    // Send data to wherever needed:
                    GameObject.Find("GameManager").GetComponent<NetworkingManager>().GetMessage(data);
                    // Put message in textbox for testing:
                    GameObject.Find("MessageText").GetComponent<Text>().text = data;
                }
                else if (data.Contains("<Scenario>"))
                {
                    data = data.Substring(0, data.Length - 10);
                    Debug.Log("Client - got scenario: " + data);
                }

            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client - Disconnecting");
                // If the server disconnected us we clear out connection
                m_clientToServerConnection = default(NetworkConnection);
            }
        }
    }
}
