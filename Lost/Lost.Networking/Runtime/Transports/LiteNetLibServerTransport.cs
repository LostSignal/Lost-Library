//-----------------------------------------------------------------------
// <copyright file="LiteNetLibServerTransport.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_LITE_NET_LIB

namespace Lost.Networking
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using LiteNetLib;

    public class LiteNetLibServerTransport : IServerTransportLayer, INetEventListener
    {
        private Dictionary<int, NetPeer> peersHash = new Dictionary<int, NetPeer>();
        private ConcurrentQueue<ServerEvent> serverEvents = new ConcurrentQueue<ServerEvent>();
        private NetManager server;
        private bool isStarting;

        private void Shutdown()
        {
            if (this.server != null)
            {
                this.isStarting = false;
                this.server.Stop();
                this.server = null;
            }
        }

        bool IServerTransportLayer.IsStarting
        {
            get
            {
                if (this.isStarting && this.server?.IsRunning == true)
                {
                    this.isStarting = false;
                }

                return this.isStarting;
            }
        }

        bool IServerTransportLayer.IsRunning => this.server?.IsRunning == true;

        void IServerTransportLayer.Update()
        {
            this.server?.PollEvents();
        }

        void IServerTransportLayer.SendData(long connectionId, byte[] data, uint offset, uint length)
        {
            if (this.peersHash.TryGetValue((int)connectionId, out NetPeer peer))
            {
                peer.Send(data, (int)offset, (int)length, DeliveryMethod.ReliableOrdered);
            }
        }

        void IServerTransportLayer.Shutdown()
        {
            this.Shutdown();
        }

        void IServerTransportLayer.Start(int port)
        {
            if (this.server != null)
            {
                this.Shutdown();
            }

            this.isStarting = true;
            this.server = new NetManager(this);
            this.server.Start(port);
            this.server.BroadcastReceiveEnabled = true;
            this.server.UpdateTime = 15;
        }

        bool IServerTransportLayer.TryDequeueServerEvent(out ServerEvent serverEvent)
        {
            return this.serverEvents.TryDequeue(out serverEvent);
        }

        void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var data = new byte[reader.UserDataSize];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = reader.RawData[reader.UserDataOffset + i];
            }

            this.serverEvents.Enqueue(new ServerEvent
            {
                ConnectionId = peer.Id,
                EventType = ServerEventType.ReceivedData,
                Data = data,
            });
        }

        void INetEventListener.OnPeerConnected(NetPeer peer)
        {
            UnityEngine.Debug.Log("ServerTransport: OnPeerConnected");

            if (this.peersHash.ContainsKey(peer.Id) == false)
            {
                this.peersHash.Add(peer.Id, peer);

                this.serverEvents.Enqueue(new ServerEvent
                {
                    ConnectionId = peer.Id,
                    EventType = ServerEventType.ConnectionOpened
                });
            }
        }

        void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            UnityEngine.Debug.Log("ServerTransport: OnPeerDisconnected");

            ServerEventType disconnectType = disconnectInfo.Reason == DisconnectReason.DisconnectPeerCalled ?
                ServerEventType.ConnectionClosed :
                ServerEventType.ConnectionLost;

            this.serverEvents.Enqueue(new ServerEvent
            {
                ConnectionId = peer.Id,
                EventType = disconnectType,
            });

            this.peersHash.Remove(peer.Id);
        }

        void INetEventListener.OnConnectionRequest(ConnectionRequest request)
        {
            UnityEngine.Debug.Log("ServerTransport: OnConnectionRequest");

            request.AcceptIfKey("gamekey");
        }

        void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            UnityEngine.Debug.Log("ServerTransport: OnNetworkError");
        }

        void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            UnityEngine.Debug.Log("ServerTransport: OnNetworkReceiveUnconnected");
        }
    }
}

#endif
