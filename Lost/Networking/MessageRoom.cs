//-----------------------------------------------------------------------
// <copyright file="MessageRoom.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Networking.Match;

    public abstract class MessageRoom : MonoBehaviour
    {
        public enum State
        {
            Waiting,
            Searching,

            Creating,
            ConnectedAsServer,
            DisconnectedAsServer,

            Joining,
            SendingClientInfo,
            ConnectedAsClient,
            DisconnectedAsClient,

            ShuttingDown,
        }

        // connection
        private ConnectionConfig connectionConfig;
        private NetworkClient client;
        private SimpleServer server;
        private MatchInfo matchInfo;
        private RoomInfo roomInfo;
        private State state;

        // tasks
        private UnityTask<List<MatchInfoSnapshot>> listMatchesTask;
        private UnityTask<MatchInfo> createMatchTask;
        private UnityTask<MatchInfo> joinMatchTask;

        // client / server state
        private MatchInfo serverMatchInfo;
        private MatchInfoSnapshot joinMatchInfoSnapshot;

        // messages
        private Dictionary<short, Type> roomMessageTypes = new Dictionary<short, Type>();
        private Dictionary<short, List<RoomMessage>> roomMessagePools = new Dictionary<short, List<RoomMessage>>();

        // used to invalidate debug info and regenerate (so it wont happen every tick)
        private string debugInfoCache = null;
        private bool isDebugInfoDirty = true;

        private ConnectionConfig ConnectionConfig
        {
            get
            {
                if (this.connectionConfig == null)
                {
                    this.connectionConfig = this.CreateConnectionConfig();
                }

                return this.connectionConfig;
            }
        }
        
        public string GetDebugSummary()
        {
            if (this.state == State.Waiting)
            {
                return string.Empty;
            }
            else if (this.isDebugInfoDirty == false)
            {
                return this.debugInfoCache;
            }
            
            StringBuilder summary = new StringBuilder();

            summary.Append("State: ");
            summary.AppendLine(this.state.ToString());
            
            if (this.roomInfo != null)
            {
                summary.Append("Room: ");
                summary.AppendLine(this.roomInfo.RoomName);
            }

            if (this.roomInfo != null && string.IsNullOrEmpty(this.roomInfo.RoomPassword) == false)
            {
                summary.Append("Room Password: ");
                summary.AppendLine(this.roomInfo.RoomPassword);
            }

            if (this.server != null)
            {
                int count = 1;

                for (int i = 0; i < this.server.connections.Count; i++)
                {
                    if (this.server.connections[i] != null)
                    {
                        count++;
                    }
                }
                
                summary.Append("Room Count: ");
                summary.AppendLine(count.ToString());
            }

            if (this.client != null)
            {
                summary.Append("Client IsConnected: ");
                summary.AppendLine(this.client.isConnected.ToString());
            }

            this.debugInfoCache = summary.ToString();
            this.isDebugInfoDirty = false;

            return this.debugInfoCache;
        }
        
        public void CreateOrJoinRoom(RoomInfo roomInfo)
        {
            if (this.state != State.Waiting)
            {
                this.SetState(State.ShuttingDown);
            }

            this.roomInfo = roomInfo;
            this.SetState(State.Searching);
        }

        public void RegisterMessage<T>() where T : RoomMessage, new()
        {
            RoomMessage roomMessage = new T();
            short roomMessageId = roomMessage.GetMessageId();
            
            if (this.roomMessageTypes.ContainsKey(roomMessageId))
            {
                Debug.LogErrorFormat(this, "RoomMessage Type {0} Has Duplicate Id {0}", roomMessage.GetType().Name, roomMessageId);
                return;
            }

            roomMessageTypes.Add(roomMessageId, typeof(T));
            roomMessagePools.Add(roomMessageId, new List<RoomMessage> { roomMessage });
        }

        public void SendRoomMessage(RoomMessage roomMessage)
        {
            // TOOD [bgish]: If temporarily disconnected, should I buffer the messages?

            short roomMessageId = roomMessage.GetMessageId();

            if (this.roomMessageTypes.ContainsKey(roomMessageId) == false)
            {
                Debug.LogErrorFormat("Tried Sending Unregisted RoomMessage Type = {0} with Id = {1}", roomMessage.GetType().Name, roomMessageId);
                return;
            }

            if (this.server != null)
            {
                if (roomMessage.IsReliable)
                {
                    this.server.SendMessage(roomMessageId, roomMessage);
                }
                else
                {
                    this.server.SendMessageUnreliable(roomMessageId, roomMessage);
                }
            }
            else if (this.client != null && this.client.isConnected)
            {
                if (roomMessage.IsReliable)
                {
                    this.client.Send(roomMessageId, roomMessage);
                }
                else
                {
                    this.client.SendUnreliable(roomMessageId, roomMessage);
                }
            }
        }

        public void Shutdown()
        {
            // stopping all match tasks
            if (this.createMatchTask != null)
            {
                this.createMatchTask.Cancel();
                this.createMatchTask = null;
            }

            if (this.listMatchesTask != null)
            {
                this.listMatchesTask.Cancel();
                this.listMatchesTask = null;
            }

            if (this.joinMatchTask != null)
            {
                this.joinMatchTask.Cancel();
                this.joinMatchTask = null;
            }
            
            if (this.client != null)
            {
                // unregistering all handlers
                this.client.UnregisterHandler(MsgType.Connect);
                this.client.UnregisterHandler(MsgType.Disconnect);
                this.client.UnregisterHandler(MsgType.NotReady);
                this.client.UnregisterHandler(MsgType.Error);

                foreach (var roomMessageId in this.roomMessageTypes.Keys)
                {
                    this.client.UnregisterHandler(roomMessageId);
                }

                // shutting down client
                this.client.Disconnect();
                this.client.Shutdown();
                this.client = null;
            }

            if (this.server != null)
            {
                // unregistering all events and handlers
                this.server.OnConnectedEvent -= Server_OnConnectedEvent;
                this.server.OnConnectErrorEvent -= Server_OnConnectErrorEvent;
                this.server.OnDataErrorEvent -= Server_OnDataErrorEvent;
                this.server.OnDisconnectErrorEvent -= Server_OnDisconnectErrorEvent;
                this.server.OnDisconnectedEvent -= Server_OnDisconnectedEvent;

                this.server.ClearHandlers();

                // shutting down server
                this.server.DisconnectAllConnections();
                this.server.Stop();
                this.server.ClearHandlers();
                this.server = null;

                MatchMakingHelper.DestroyMatch(this.serverMatchInfo);
            }

            this.roomInfo = null;
            this.joinMatchInfoSnapshot = null;
            this.serverMatchInfo = null;

            if (this.state != State.Waiting)
            {
                this.SetState(State.Waiting);
            }
        }

        protected abstract void RegisterCustomMessages();

        protected abstract void OnRoomMessageReceived(RoomMessage roomMessage);

        protected abstract void DisconnectedAsServer();

        protected abstract void DisconnectedAsClient();

        private void Awake()
        {
            this.RegisterMessage<ClientConnected>();
            this.RegisterMessage<ClientsConnected>();
            this.RegisterMessage<ClientDisconnected>();
            this.RegisterMessage<ClientIdentity>();

            this.RegisterCustomMessages();
        }

        private void OnDestroy()
        {
            this.Shutdown();
        }

        private void SetState(State newState)
        {
            if (this.state == newState)
            {
                Debug.LogErrorFormat("MessageRoom tried to go into state {0} while already in that state.", newState);
                return;
            }

            this.isDebugInfoDirty = true;
            this.state = newState;

            switch (this.state)
            {
                case State.Searching:
                    {
                        this.listMatchesTask = MatchMakingHelper.ListMatches(this.roomInfo.RoomName, this.roomInfo.RoomPassword, this.roomInfo.Elo, false);
                        break;
                    }

                case State.Joining:
                    {
                        this.joinMatchTask = MatchMakingHelper.JoinMatch(this.joinMatchInfoSnapshot.networkId, this.roomInfo.RoomPassword, this.roomInfo.Elo);
                        break;
                    }

                case State.Creating:
                    {
                        this.createMatchTask = MatchMakingHelper.CreateMatch(this.roomInfo.RoomName, this.roomInfo.MaxConnections, true, this.roomInfo.RoomPassword, this.roomInfo.Elo);
                        break;
                    }

                case State.SendingClientInfo:
                    {
                        // TODO [bgish]: Send the Client info
                        this.SetState(State.ConnectedAsClient);
                        break;
                    }

                case State.DisconnectedAsClient:
                    {
                        this.DisconnectedAsClient();
                        this.SetState(State.ShuttingDown);
                        break;
                    }

                case State.DisconnectedAsServer:
                    {
                        this.DisconnectedAsServer();
                        this.SetState(State.ShuttingDown);
                        break;
                    }

                case State.ShuttingDown:
                    {
                        this.Shutdown();
                        break;
                    }

                default:
                    break;
            }
        }

        private void Update()
        {
            switch (this.state)
            {
                case State.Searching:
                    {
                        if (this.listMatchesTask.IsDone)
                        {
                            if (this.listMatchesTask.HasError == false)
                            {
                                if (this.listMatchesTask.Value.Count == 0)
                                {
                                    this.SetState(State.Creating);
                                }
                                else
                                {
                                    this.joinMatchInfoSnapshot = this.listMatchesTask.Value[0];
                                    this.SetState(State.Joining);
                                }
                            }
                            else
                            {
                                Debug.LogErrorFormat("Restarting: Error Trying To List Matches: {0}", this.listMatchesTask.Exception.Message);
                                this.CreateOrJoinRoom(this.roomInfo);
                            }
                        }

                        break;
                    }

                case State.Joining:
                    {
                        if (this.joinMatchTask.IsDone)
                        {
                            if (joinMatchTask.HasError == false)
                            {
                                Debug.Assert(this.client == null, "Creating client, when old client was never cleanded up!");

                                this.matchInfo = joinMatchTask.Value;

                                this.client = new NetworkClient();
                                this.client.Configure(this.ConnectionConfig, this.joinMatchInfoSnapshot.maxSize);

                                this.client.RegisterHandler(MsgType.Connect, this.Client_OnConnectInternal);
                                this.client.RegisterHandler(MsgType.Disconnect, this.Client_OnDisconnectInternal);
                                this.client.RegisterHandler(MsgType.NotReady, this.Client_OnNotReadyMessageInternal);
                                this.client.RegisterHandler(MsgType.Error, this.Client_OnErrorInternal);

                                // registering room messages
                                foreach (var roomMessageId in this.roomMessageTypes.Keys)
                                {
                                    this.client.RegisterHandler(roomMessageId, this.HandleRoomMessage);
                                }

                                this.client.Connect(this.matchInfo);
                                this.SetState(State.SendingClientInfo);
                            }
                            else
                            {
                                Debug.LogErrorFormat("Restarting: Error Trying To Join Match: {0}", this.joinMatchTask.Exception.Message);
                                this.CreateOrJoinRoom(this.roomInfo);
                            }
                        }

                        break;
                    }

                case State.Creating:
                    {
                        if (this.createMatchTask.IsDone)
                        {
                            if (this.createMatchTask.HasError == false)
                            {
                                Debug.Assert(this.client == null, "Creating client, when old client was never cleanded up!");

                                this.server = new SimpleServer();
                                this.server.OnConnectedEvent += Server_OnConnectedEvent;
                                this.server.OnConnectErrorEvent += Server_OnConnectErrorEvent;
                                this.server.OnDataErrorEvent += Server_OnDataErrorEvent;
                                this.server.OnDisconnectErrorEvent += Server_OnDisconnectErrorEvent;
                                this.server.OnDisconnectedEvent += Server_OnDisconnectedEvent;

                                // registering room messages
                                foreach (var roomMessageId in this.roomMessageTypes.Keys)
                                {
                                    this.server.RegisterHandler(roomMessageId, this.HandleRoomMessage);
                                }

                                this.serverMatchInfo = this.createMatchTask.Value;
                                this.server.Configure(this.ConnectionConfig, (int)this.roomInfo.MaxConnections);
                                this.server.ListenMatchInfo(this.serverMatchInfo);
                                this.SetState(State.ConnectedAsServer);
                            }
                            else
                            {
                                Debug.LogErrorFormat("Restarting: Error Trying To Create Match: {0}", this.createMatchTask.Exception.Message);
                                this.CreateOrJoinRoom(this.roomInfo);
                            }
                        }

                        break;
                    }

                case State.ConnectedAsServer:
                    this.server.Update();
                    break;

                default:
                    break;
            }
        }

        protected void RecycleRoomMessage(RoomMessage roomMessage)
        {
            short roomMessageId = roomMessage.GetMessageId();

            if (this.roomMessagePools.ContainsKey(roomMessageId) == false)
            {
                Debug.LogErrorFormat("Tried to recycle RoomMessage {0} with Id {1} without registering it!", roomMessage.GetType().Name, roomMessageId);
                return;
            }

            this.roomMessagePools[roomMessageId].Add(roomMessage);
        }

        private RoomMessage GetRoomMessage(short msgType)
        {
            List<RoomMessage> pool = null;

            if (this.roomMessagePools.TryGetValue(msgType, out pool) == false)
            {
                Debug.LogErrorFormat("Tried to get RoomMessage Id {0} without registering it!", msgType);
                return null;
            }

            if (pool.Count > 0)
            {
                int lastIndex = pool.Count - 1;
                RoomMessage roomMessage = pool[lastIndex];
                pool.RemoveAt(lastIndex);
                return roomMessage;
            }
            else
            {
                Type roomMessageType = null;
                if (this.roomMessageTypes.TryGetValue(msgType, out roomMessageType))
                {
                    return (RoomMessage)Activator.CreateInstance(roomMessageType);
                }
                else
                {
                    Debug.LogErrorFormat("Tried to get RoomMessage Id {0} without registering it!", msgType);
                    return null;
                }
            }
        }

        private void HandleRoomMessage(NetworkMessage networkMessage)
        {
            var roomMessageInstance = this.GetRoomMessage(networkMessage.msgType);
            roomMessageInstance.Deserialize(networkMessage.reader);

            // if we're the server, then lets forward these messages back to all the other clients (except the one that sent it)
            if (this.server != null && roomMessageInstance.RelayToAllClients)
            {
                for (int i = 0; i < this.server.connections.Count; i++)
                {
                    var connection = this.server.connections[i];
                    if (connection != null && connection.isConnected && connection != networkMessage.conn)
                    {
                        if (roomMessageInstance.IsReliable)
                        {
                            connection.Send(roomMessageInstance.GetMessageId(), roomMessageInstance);
                        }
                        else
                        {
                            connection.SendUnreliable(roomMessageInstance.GetMessageId(), roomMessageInstance);
                        }
                    }
                }
            }

            // TODO [bgish]: actually handle these cases
            switch (networkMessage.msgType)
            {
                case ClientConnected.MessageId:
                case ClientsConnected.MessageId:
                case ClientDisconnected.MessageId:
                case ClientIdentity.MessageId:
                    break;
            }
            
            this.OnRoomMessageReceived(roomMessageInstance);
        }
                
        protected virtual ConnectionConfig CreateConnectionConfig()
        {
            var config = new ConnectionConfig();
            config.AddChannel(QosType.Reliable);
            config.AddChannel(QosType.Unreliable);

            return config;
        }

        #region Client Callbacks

        // this is called when the client has successfully connected to the server
        private void Client_OnConnectInternal(NetworkMessage message)
        {
            Debug.Log("Client_OnConnectInternal");
            this.isDebugInfoDirty = true;
        }

        // this is called when the client is disconnected from the server, or was never able to connect to the server (timeout is pretty long if server quit while joining room, may want to shorten it a bit)
        private void Client_OnDisconnectInternal(NetworkMessage message)
        {
            Debug.LogError("Client_OnDisconnectInternal");
            this.SetState(State.DisconnectedAsClient);
            this.isDebugInfoDirty = true;
        }

        private void Client_OnNotReadyMessageInternal(NetworkMessage message)
        {
            Debug.LogError("Client_OnNotReadyMessageInternal");
            this.isDebugInfoDirty = true;
        }

        private void Client_OnErrorInternal(NetworkMessage message)
        {
            Debug.LogError("Client_OnErrorInternal");
            this.isDebugInfoDirty = true;
        }

        #endregion

        #region Simple Server Callbacks

        private void Server_OnDisconnectErrorEvent(UnityEngine.Networking.NetworkConnection conn, byte error)
        {
            // TODO [bgish]: Need to send client disconnected event to everyone
            Debug.LogErrorFormat("Server_OnDisconnectErrorEvent: {0}", error);
            this.isDebugInfoDirty = true;
        }

        private void Server_OnDataErrorEvent(UnityEngine.Networking.NetworkConnection conn, byte error)
        {
            Debug.LogErrorFormat("Server_OnDataErrorEvent: {0}", error);
            this.isDebugInfoDirty = true;
        }

        private void Server_OnConnectErrorEvent(int connectionId, byte error)
        {
            Debug.LogErrorFormat("Server_OnConnectErrorEvent: {0}", error);
            this.isDebugInfoDirty = true;
        }

        private void Server_OnConnectedEvent(UnityEngine.Networking.NetworkConnection conn)
        {
            // conn.connectionId

            // TODO [bgish]: Need to send client connected event to everyone
            Debug.Log("Server_OnConnectedEvent");
            this.isDebugInfoDirty = true;
        }

        private void Server_OnDisconnectedEvent(NetworkConnection conn)
        {
            Debug.Log("Server_OnDisconnectedEvent");
            this.SetState(State.DisconnectedAsServer);
            this.isDebugInfoDirty = true;
        }

        #endregion
    }
}
