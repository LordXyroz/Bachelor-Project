﻿using UnityEngine;
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
using System.Collections.Generic;
using System.Linq;


public class ServerBehaviour
{
    public UdpCNetworkDriver m_ServerDriver;
    public NativeList<NetworkConnection> m_connections;
    private JobHandle m_updateHandle;

    TcpListener server;

    Socket listener;
    Socket handler;
    private bool listen;

    private List<(string name, int connectionNumber)> connectionNames;

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
        /// TODO make sure that I don't add too many?
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
                if (m_connections.IsCreated)
                {
                    /// Lobby is full:
                    if (m_connections.Length == 3) /// 2 is total amount of players
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

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    //Debug.Log("newconnection - Received: " + data);
                    

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    //Debug.Log("newconnection - sent: " + data);
                }
               
                Thread.Sleep(80);
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

    /// <summary>
    /// Server behaviour constructor to set up basic settings for having a server behaviour in the game.
    /// </summary>
    public ServerBehaviour()
    {

        connectionNames = new List<(string name, int connectionNumber)> { };

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
        /// Put message in textbox for chat:
        NetworkingManager nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();
        nm.GetChatMessage(message);

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

    /// <summary>
    /// Destructor for server behaviour, makes sure that important data is cleaned up.
    /// </summary>
    ~ServerBehaviour()
    {
        // All jobs must be completed before we can dispose the data they use
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
                    var writer = new DataStreamWriter(150, Allocator.Temp);
                    if (data.Contains("<UpdateConnection>"))
                    {
                        writer.Write(Encoding.ASCII.GetBytes("Connection updated<UpdateConnection>"));
                        m_ServerDriver.Send(m_connections[i], writer);
                    }
                    else if (data.Contains("<Connecting>"))
                    {
                        NetworkingManager nm = GameObject.Find("GameManager").GetComponent<NetworkingManager>();

                        /// Add new client to list of players in lobby:
                        string name = data.Substring(0, data.Length - 12);
                        Debug.Log("Server - Connecting client with name: " + name);
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
                    else if (data.Contains("<SwapAccepted>")) ///TODO 3 things...
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
                        GameObject.Find("GameManager").GetComponent<NetworkingManager>().FindSwapNames(name, clientName);
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

                        /// TODO check if substring here gets correct value...
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
                        GameObject.Find("GameManager").GetComponent<NetworkingManager>().GetChatMessage(data);

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
                    string name = connectionNames.Where(x => x.connectionNumber == i).First().name;  /// TODO check that they don't have the same name?;
                    GameObject.Find("GameManager").GetComponent<NetworkingManager>().FindPlayerForRemoval(name);

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


                    /// This connection no longer exists.
                    m_connections.RemoveAtSwapBack(i);
                    if (i >= m_connections.Length)
                        break;
                }
            }
        }
    }
}
