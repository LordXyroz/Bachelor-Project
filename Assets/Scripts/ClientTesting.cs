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

public class ClientTesting : MonoBehaviour
{
    struct PendingPing
    {
        public int id;
        public float time;
    }
    

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



    // Used to check if client is trying to start to play a game, no matter if host or not.
    public static NetworkEndPoint ServerEndPoint { get; private set; }
    private bool connect;
    private bool start;

    private UdpCNetworkDriver m_ClientDriver;
    private NetworkConnection m_clientToServerConnection;
    // pendingPing is a ping sent to the server which have not yet received a response.
    private PendingPing m_pendingPing;
    // The ping stats are two integers, time for last ping and number of pings
    private int m_lastPingTime;
    private int m_numPingsSent;
    

    // Testing thread:
    public static void ThreadProc()
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
                Debug.Log("Clientside - Echoed test = " + Encoding.ASCII.GetString(bytes, 0, bytesRec));

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
    }


    void Start()
    {
        start = true;
        connect = false;
       
        /*Thread t = new Thread(new ThreadStart(ThreadProc));

        t.Start();*/

        m_ClientDriver = new UdpCNetworkDriver(new INetworkParameter[0]);
    }

    void OnDestroy()
    {
        m_ClientDriver.Dispose();
    }

    public void ConnectToServer()
    {
        if (connect)
        {
            connect = false;

            // If the client presses the button to branch to this section, the connection will be closed.(as wanted by user.)
            m_clientToServerConnection.Disconnect(m_ClientDriver);
            m_clientToServerConnection = default(NetworkConnection);
            Debug.Log("UI - disconnecting...");
        }
        else
        {
            connect = true;

            ServerEndPoint = new IPEndPoint(IPAddress.Parse("10.22.210.138"), 9000);
            // If the client ui indicates we should be sending pings but we do not have an active connection we create one
            if (!m_clientToServerConnection.IsCreated && ServerEndPoint.IsValid)
            {
                m_clientToServerConnection = m_ClientDriver.Connect(ServerEndPoint);
            }
            Debug.Log("UI - connecting...");
        }
        
    }

    public void SendMessage(string message)
    {
        var messageWriter = new DataStreamWriter(50, Allocator.Temp);
        // Setting prefix for server to easily know what kind of msg is being written.
        message += "<Message>";
        byte[] msg = Encoding.ASCII.GetBytes(message);
        messageWriter.Write(msg);
        m_ClientDriver.Send(m_clientToServerConnection, messageWriter);
    }

    void FixedUpdate()
    {

        // When client wants to start a game, check for being host or not.
        if (start)
        {
            start = false;

            // Testing to get ipaddresses
            List<string[]> results = new List<string[]>();

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
            for (int i = 0; i < results.Count; i++)
            {
                Debug.Log("result nr." + i + ": " + results[i][0]);
            }
            //Your own ip is not in these results, so when you want to host yourself, do something else than this...
            //For now just add own ip address so you can host.TODO fix.
            
            ///
            /// start takes results and tries to get connection this one time.
            /// in start try to send message, if nothing is received back within a short period, we have no connection?
            /// when start is done, it will go through else part, and send/receive data whenever needed.
            /// 



        }
        // Update the ping client UI with the ping statistics computed by teh job scheduled previous frame since that
        // is now guaranteed to have completed
        UITest.UpdateStats(m_numPingsSent, m_lastPingTime);

        // Update the NetworkDriver. It schedules a job so we must wait for that job with Complete
        m_ClientDriver.ScheduleUpdate().Complete();




        NetworkEvent.Type cmd;
        // Process all events on the connection. If the connection is invalid it will return Empty immediately
        while ((cmd = m_clientToServerConnection.PopEvent(m_ClientDriver, out DataStreamReader strm)) != NetworkEvent.Type.Empty)
        {
            
            if (cmd == NetworkEvent.Type.Connect)
            {
                // Create a 4 byte data stream which we can store our ping sequence number in
                var pingData = new DataStreamWriter(50, Allocator.Temp);
                byte[] msg = Encoding.ASCII.GetBytes("<Connecting>");
                pingData.Write(msg);
                m_clientToServerConnection.Send(m_ClientDriver, pingData);

                pingData.Dispose();
                // Update the number of sent pings
                ++m_numPingsSent;
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
                }
                else if (data.Contains("<MessageReply>"))
                {
                    Debug.Log("Client message - " + data);
                }
                
                /*m_clientToServerConnection.Disconnect(m_ClientDriver);
                m_clientToServerConnection = default(NetworkConnection);*/
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
