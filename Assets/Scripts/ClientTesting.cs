using System.Net;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.UI;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;

public class ClientTesting : MonoBehaviour
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

    IEnumerator FindHost()
    {
        for (int i = 0; i < results.Count; i++)
        {

            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(results[i][0]), 11000);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(IPAddress.Parse(results[i][0]).AddressFamily,//maybe change to local ip!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
                    Debug.Log("Clientside - Echoed test = " + Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    if (Encoding.ASCII.GetString(bytes, 0, bytesRec).Length > 0)
                    {
                        ServerEndPoint = new IPEndPoint(IPAddress.Parse(results[i][0]), 9000);
                    }
                    else
                    {
                        ServerEndPoint = default;
                    }

                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            
            yield return new WaitForSeconds(1);
            

            Debug.Log("finished result nr: " + i);
        }
    }

    /// <summary>
    /// GetResults is gathering the ipaddresses of possible hosts of the game and puts them into the list 'results'.
    /// </summary>
    public void GetResults()
    {
        using (System.Diagnostics.Process p = new System.Diagnostics.Process())
        {
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.Arguments = "/c arp -a";
            p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe"; // If user is not of windows one should use something else!
            p.Start();

            string line;

            while ((line = p.StandardOutput.ReadLine()) != null)
            {
                if (line != "" && !line.Contains("Interface") && !line.Contains("Physical Address"))
                {
                    var lineArr = line.Trim().Split(' ').Select(n => n).Where(n => !string.IsNullOrEmpty(n)).ToArray();
                    var arrResult = new string[]
                {
                   lineArr[0],
                   lineArr[1],
                   lineArr[2]
                };
                    results.Add(arrResult);
                }
            }

            p.WaitForExit();
        }

        // Add local ip for testing purposes.
        string[] localIP = new string[3];
        localIP[0] = GetLocalIPAddress();
        results.Add(localIP);
        //TODO try connect to all of these, if can't connect to any of them
        /*for (int i = 0; i < results.Count; i++)
        {
            Debug.Log("result nr." + i + ": " + results[i][0]);
        }*/
    }

    public void GetHostIPAddress()
    {
        /// Collects possible ips of hosts in the local network.
        GetResults();
        
        // TODO, for matchmaking make the players have to wait until all players are connected and in game.

        /// Tries to find a host out of the possible ips in the local network.
        //StartCoroutine(FindHost());

        /*for (int i = 0; i < results.Count; i++)
        {
            Debug.Log("ipaddress nr: " + i);



            ServerEndPoint = new IPEndPoint(IPAddress.Parse(results[i][0]), 9000);
            // If the client ui indicates we should be sending pings but we do not have an active connection we create one.
            if (!m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid)
            {
                m_clientToServerConnection = m_ClientDriver.Connect(ServerEndPoint);
            }
            
            Debug.Log(m_ClientDriver.GetConnectionState(m_clientToServerConnection));

            // Update the NetworkDriver. Needs to be done before trying to pop events.
            m_ClientDriver.ScheduleUpdate().Complete();
            

            NetworkEvent.Type cmd;
            // Process all events on the connection. If the connection is invalid it will return Empty immediately.
            while ((cmd = m_clientToServerConnection.PopEvent(m_ClientDriver, out DataStreamReader strm)) != NetworkEvent.Type.Empty)
            {

                if (cmd == NetworkEvent.Type.Connect)
                {
                    /// Create a 4 byte data stream which we can send our connection request through.
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

                    if (data.Contains("<Connect>"))
                    {
                        Debug.Log("Client - You are connected to server!");
                        break;
                        //TODO see if break puts us in correct spot..
                    }
                    else
                    {
                        ServerEndPoint = default;
                    }

                    //m_clientToServerConnection.Disconnect(m_ClientDriver);
                    //m_clientToServerConnection = default(NetworkConnection);
                }
            }
            /*try
            {
                var writer = new DataStreamWriter(100, Allocator.Temp);
                byte[] message = Encoding.ASCII.GetBytes("Hello son!<Message>");
                writer.Write(message);

                var failure = m_ClientDriver.Send(m_clientToServerConnection, writer);
                Debug.Log("failure: " + failure);
            } catch(Exception e)
            {
                Debug.Log(e);
            }

            ServerEndPoint = default;
        }*/
        //Debug.Log("Ipaddresses looped");// If serverendpoint doesn't have a valid ipaddress at this point, we could not find any hosts! TODO say this to user!
    }

    public void idk()
    {
        Debug.Log("idk");
    }
    

    // Testing thread:
    public void ThreadProc()
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
    public void ConnectToServer()
    {
        if (connect)
        {
            connect = false;

            // Disconnect client from host:
            m_clientToServerConnection.Disconnect(m_ClientDriver);
            m_clientToServerConnection = default;
            ServerEndPoint = default;
        }
        else
        {
            //GetResults();
            //GetHostIPAddress();
            
            /// Could use thread, but testing showed that Task was more efficient to use in this scenario. Easy to wait for the task to finish before doing something else aswell.
            Task setupHostIp = Task.Run(ThreadProc);
            setupHostIp.Wait();
            
            connect = true;
            /// Connect to host if one is found and connection isn't already made.
            if (!m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid)
            {
                m_clientToServerConnection = m_ClientDriver.Connect(ServerEndPoint);
            }
        }
    }

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
            yield return new WaitForSeconds(15);
        }
    }

    /// <summary>
    /// SendMessage is used to send simple strings through the network, will later on use the type 'Message' to send more data through the network.
    /// </summary>
    /// <param name="message"></param>
    public new void SendMessage(string message)
    {
        // Example of sending a string through the network:
        var messageWriter = new DataStreamWriter(100, Allocator.Temp);
        // Setting prefix for server to easily know what kind of msg is being written.
        message += "<Message>";
        byte[] msg = Encoding.ASCII.GetBytes(message);
        messageWriter.Write(msg);
        m_ClientDriver.Send(m_clientToServerConnection, messageWriter);
        messageWriter.Dispose();

        // Example of sending a class object through the network:
        /*TestClass testObj = new TestClass
        {
            playerName = "Chris",
            timeElapsed = 3.14f,
            level = 5,
            position = new Vector2(5, 3),
            values = new int[]{ 5, 3, 6, 8, 2 }
        };
        SendClass(testObj);*/

    }


    public void SendClass(dynamic obj)
    {
        Debug.Log("Client - Sending classObject.");
        // Maybe change this to <Message> later on, as simple string messages won't be sent over the network then.
        string classObj = JsonUtility.ToJson(obj) + "<Class>";
        var classWriter = new DataStreamWriter(classObj.Length + 2, Allocator.Temp);
        // Setting prefix for server to easily know what kind of msg is being written.
        byte[] msg = Encoding.ASCII.GetBytes(classObj);
        classWriter.Write(msg);
        m_ClientDriver.Send(m_clientToServerConnection, classWriter);
        classWriter.Dispose();
    }

    public class TestClass
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
                    Debug.Log("Client - Got message: " + data);
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
