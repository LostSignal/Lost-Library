//-----------------------------------------------------------------------
// <copyright file="DarkRiftClientTransport.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_DARKRIFT_2

namespace Lost.Networking
{
    using System.Net;
    using DarkRift;
    using DarkRift.Client;
    using DarkRift.Dispatching;

    public class DarkRiftClientTransport : IClientTransportLayer
    {
        private ConcurrentQueue<ClientEvent> events = new ConcurrentQueue<ClientEvent>();
        private ObjectCacheSettings objectCacheSettings = new SerializableObjectCacheSettings().ToObjectCacheSettings();
        private DarkRiftClient client;

        bool IClientTransportLayer.IsConnecting => this.client?.ConnectionState == ConnectionState.Connecting;

        bool IClientTransportLayer.IsConnected => this.client?.ConnectionState == ConnectionState.Connected;

        void IClientTransportLayer.Connect(string connectionString)
        {
            if (this.client != null)
            {
                this.Shutdown();
            }

            this.client = new DarkRiftClient(this.objectCacheSettings);
            this.client.MessageReceived += Client_MessageReceived;
            this.client.Disconnected += Client_Disconnected;
            this.client.Connect(IPAddress.Parse("127.0.0.1"), 9999, IPVersion.IPv4);

            if (this.client.ConnectionState == ConnectionState.Connected)
            {
                this.events.Enqueue(new ClientEvent { EventType = ClientEventType.ConnectionOpened });
            }
        }

        void IClientTransportLayer.SendData(byte[] data, uint offset, uint length)
        {
            using (DarkRiftWriter writer = DarkRiftWriter.Create())
            {
                writer.WriteRaw(data, (int)offset, (int)length);

                using (var message = DarkRift.Message.Create(0, writer))
                {
                    this.client.SendMessage(message, SendMode.Reliable);
                }
            }
        }

        void IClientTransportLayer.Shutdown()
        {
            this.events.Enqueue(new ClientEvent { EventType = ClientEventType.ConnectionClosed });
            this.Shutdown();
        }

        bool IClientTransportLayer.TryDequeueClientEvent(out ClientEvent clientEvent)
        {
            return this.events.TryDequeue(out clientEvent);
        }

        private void Client_Disconnected(object sender, DisconnectedEventArgs e)
        {
            this.events.Enqueue(new ClientEvent { EventType = ClientEventType.ConnectionLost });
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            this.events.Enqueue(new ClientEvent
            {
                EventType = ClientEventType.ReceivedData,
                Data = e.GetMessage().GetReader().ReadBytes(),
            });
        }

        private void Shutdown()
        {
            if (this.client != null)
            {
                this.client.MessageReceived -= Client_MessageReceived;
                this.client.Disconnected -= Client_Disconnected;
                this.client.Disconnect();
                this.client.Dispose();
                this.client = null;
            }
        }
    }
}

#endif
