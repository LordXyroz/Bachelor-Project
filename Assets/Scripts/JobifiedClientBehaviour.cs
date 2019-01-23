using UnityEngine;
using Unity.Collections;
using System.Net;   // To easier get access to the IPEndPoint and IPAddress
using Unity.Networking.Transport;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using Unity.Jobs;

public class JobifiedClientBehaviour : MonoBehaviour
{

    public UdpCNetworkDriver m_Driver;
    public NativeArray<NetworkConnection> m_Connection;
    public NativeArray<byte> m_Done;
    public JobHandle ClientJobHandle;

    void Start()
    {
        // Set up the system to connect to the server.
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        m_Connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        m_Done = new NativeArray<byte>(1, Allocator.Persistent);
        
        var endpoint = new IPEndPoint(IPAddress.Loopback, 9000);
        m_Connection[0] = m_Driver.Connect(endpoint);
    }

    public void OnDestroy()
    {
        ClientJobHandle.Complete(); // Make sure that jobs are completed before cleaning up and destroying stuff that might be used.

        m_Connection.Dispose();
        m_Driver.Dispose();
        m_Done.Dispose();
    }

    void Update()
    {
        // Before start of new frame check that the last frame is complete.
        ClientJobHandle.Complete();

        // Used to chain job:
        var job = new ClientUpdateJob
        {
            driver = m_Driver,
            connection = m_Connection,
            done = m_Done
        };

        // Pass the ClientJobHandle to the job.
        ClientJobHandle = m_Driver.ScheduleUpdate();
        ClientJobHandle = job.Schedule(ClientJobHandle);
    }
}

// Handle inputs from the network:
struct ClientUpdateJob : IJob
{
    public UdpCNetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NativeArray<byte> done;

    public void Execute() // Handle updates:
    {
        if (!connection[0].IsCreated)
        {
            if (done[0] != 1)
                Debug.Log("Client - Something went wrong during connect");
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = connection[0].PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("Client - We are connected to the server");

                var value = 7;
                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    connection[0].Send(driver, writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                var readerCtx = default(DataStreamReader.Context);
                uint value = stream.ReadUInt(ref readerCtx);
                Debug.Log("Client - Got the value = " + value + " back from the server");

                done[0] = 1;
                connection[0].Disconnect(driver);
                connection[0] = default(NetworkConnection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client - got disconnected from server");
                connection[0] = default(NetworkConnection);
            }
        }
    }
}
