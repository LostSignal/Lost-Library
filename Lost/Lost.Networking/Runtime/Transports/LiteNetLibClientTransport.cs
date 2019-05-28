//-----------------------------------------------------------------------
// <copyright file="LiteNetLibClientTransport.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_LITE_NET_LIB

namespace Lost.Networking
{
    using System.Net;
    using System.Net.Sockets;
    using LiteNetLib;

    public class LiteNetLibClientTransport : IClientTransportLayer, INetEventListener
    {
        private ConcurrentQueue<ClientEvent> clientEvents = new ConcurrentQueue<ClientEvent>();
        private NetManager client;
        private bool isConnecting;
        private string ip;
        private int port;

        private void Shutdown()
        {
            if (this.client != null)
            {
                this.isConnecting = false;
                this.client.Stop();
                this.client = null;
            }
        }

        bool IClientTransportLayer.IsConnecting
        {
            get
            {
                if (this.isConnecting && this.client?.IsRunning == true)
                {
                    this.isConnecting = false;
                }

                return this.isConnecting;
            }
        }

        bool IClientTransportLayer.IsConnected => this.client?.IsRunning == true;

        void IClientTransportLayer.Update()
        {
            this.client?.PollEvents();
        }

        void IClientTransportLayer.Connect(string connectionString)
        {
            if (this.client != null)
            {
                this.Shutdown();
            }

            this.isConnecting = true;

            this.ip = connectionString.Split(':')[0];
            this.port = int.Parse(connectionString.Split(':')[1]);

            this.client = new NetManager(this);
            this.client.UnconnectedMessagesEnabled = true;
            this.client.UpdateTime = 15;
            this.client.Start();
            this.client.Connect(ip, this.port, "gamekey");
        }

        void IClientTransportLayer.SendData(byte[] data, uint offset, uint length)
        {
            this.client.SendToAll(data, (int)offset, (int)length, DeliveryMethod.ReliableOrdered);
        }

        void IClientTransportLayer.Shutdown()
        {
            this.Shutdown();
        }

        bool IClientTransportLayer.TryDequeueClientEvent(out ClientEvent clientEvent)
        {
            return this.clientEvents.TryDequeue(out clientEvent);
        }

        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            this.clientEvents.Enqueue(new ClientEvent { EventType = ClientEventType.ConnectionOpened });
        }

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var data = new byte[reader.UserDataSize];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = reader.RawData[reader.UserDataOffset + i];
            }

            this.clientEvents.Enqueue(new ClientEvent
            {
                EventType = ClientEventType.ReceivedData,
                Data = data,
            });
        }

        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            UnityEngine.Debug.Log("ClientTransport: OnPeerDisconnected - " + disconnectInfo.Reason);

            ClientEventType disconnectType = disconnectInfo.Reason == DisconnectReason.DisconnectPeerCalled ?
                ClientEventType.ConnectionClosed :
                ClientEventType.ConnectionLost;

            this.clientEvents.Enqueue(new ClientEvent { EventType = disconnectType });
        }


        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            UnityEngine.Debug.Log("ClientTransport: OnNetworkError");
        }

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            UnityEngine.Debug.Log("ClientTransport: OnNetworkReceiveUnconnected");
        }

        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            UnityEngine.Debug.Log("ClientTransport: OnConnectionRequest");
        }
    }
}

#endif
