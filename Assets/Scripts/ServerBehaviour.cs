using UnityEngine;
using System.Net;   // To easier get access to the IPEndPoint and IPAddress

using Unity.Networking.Transport;
using Unity.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class ServerBehaviour : MonoBehaviour
{

    public UdpCNetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;

    void Start()
    {
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]); // Create driver without parameters.
        // Bind driver to port 9000 and listen to it:
        if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, 9000)) != 0)
        {
            Debug.Log("Failed to bind to port 9000!");
        }
        else
        {
            m_Driver.Listen();
        }

        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent); // Create a nativelist to hold all connections.
    }

    void OnDestroy() // The driver and connections needs to be disposed since they have allocated unmanaged memory.
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    void Update()
    {
        // Make sure that the server is ready to process any updates.
        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections:
        for(int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                i--;
            }
        }

        // Accept new connections:
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection!");
        }

        // Query driver for events that might have happened since the last update:
        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
                continue;

            // For each connection call PopEventForConnection while there are more events that need processing.
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)  // Client has sent data, one can read this data with DataStreamReader.Context.
                {
                    var readerCtx = default(DataStreamReader.Context);

                    // Reading number sent from client:
                    uint number = stream.ReadUInt(ref readerCtx);
                    Debug.Log("Got " + number + " from the Client adding +2 to it.");
                    number += 2;

                    // Sending number received from client back with number+=2;
                    using (var writer = new DataStreamWriter(4, Allocator.Temp))
                    {
                        writer.Write(number);
                        m_Driver.Send(m_Connections[i], writer);
                    }
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }
    }
}
