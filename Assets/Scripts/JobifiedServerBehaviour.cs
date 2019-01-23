using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Net;
using Unity.Networking.Transport;
using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using UnityEngine.Assertions;

public class JobifiedServerBehaviour : MonoBehaviour
{
    public UdpCNetworkDriver m_Driver;
    public NativeList<NetworkConnection> m_Connections;
    private JobHandle ServerJobHandle;  // Keep track of ongoing jobs.

    void Start()
    {
        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]); // Create driver without parameters.

        // Bind driver to port 9000 and listen through it:
        if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, 9000)) != 0)
        {
            Debug.Log("Server - Failed to bind to port 9000!");
        }
        else
        {
            m_Driver.Listen();
        }
    }

    public void OnDestroy()
    {
        // Complete jobs before removing data necessary for the jobs.
        ServerJobHandle.Complete();

        m_Connections.Dispose();
        m_Driver.Dispose();
    }

    void Update()
    {
        ServerJobHandle.Complete();

        var connectionJob = new ServerUpdateConnectionsJob
        {
            driver = m_Driver,
            connections = m_Connections
        };

        var serverUpdateJob = new ServerUpdateJob
        {
            driver = m_Driver.ToConcurrent(),
            connections = m_Connections.ToDeferredJobArray()
        };

        ServerJobHandle = m_Driver.ScheduleUpdate();
        ServerJobHandle = connectionJob.Schedule(ServerJobHandle);
        ServerJobHandle = serverUpdateJob.Schedule(m_Connections, 1, ServerJobHandle);
    }
}


struct ServerUpdateJob : IJobParallelFor
{
    public UdpCNetworkDriver.Concurrent driver; // .Concurrent allows to call from multiple threads.
    public NativeArray<NetworkConnection> connections;

    public void Execute(int index)  // Use index to get through all connections.
    {
        DataStreamReader stream;
        if (!connections[index].IsCreated)  
        {
            Assert.IsTrue(true);
        }

        NetworkEvent.Type cmd;
        while ((cmd = driver.PopEventForConnection(connections[index], out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)  // Got data from client:
            {
                var readerCtx = default(DataStreamReader.Context);
                uint number = stream.ReadUInt(ref readerCtx);

                Debug.Log("Server - Got " + number + " from the Client adding + 2 to it.");
                Debug.Log("Sending it to client: " + index);
                number += 2;

                using (var writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(number);
                    driver.Send(connections[index], writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)   // Client disconnected:
            {
                Debug.Log("Server - Client disconnected from server");
                connections[index] = default(NetworkConnection);
            }
        }
    }
}

struct ServerUpdateConnectionsJob : IJob
{
    public UdpCNetworkDriver driver;
    public NativeList<NetworkConnection> connections;

    public void Execute()
    {
        // Clean up connections:
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept new connections:
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
            Debug.Log("Server - Accepted a connection.");
        }
    }
}