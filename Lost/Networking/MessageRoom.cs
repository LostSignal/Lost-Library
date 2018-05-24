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

    public enum NetworkingState
    {
        NotConnected,
        Client,
        Server,
    }

    public abstract class MessageRoom<UserInfo> : MonoBehaviour where UserInfo : UserInfoMessage, new()
    {
        public enum State
        {
            Waiting,
            Searching,

            Creating,
            ConnectedAsServer,
            DisconnectedAsServer,

            Joining,
            ConnectedAsClient,
            DisconnectedAsClient,

            ShuttingDown,
        }

        public NetworkingState NetworkingState
        {
            get
            {
                if (this.server != null)
                {
                    return NetworkingState.Server;
                }
                else if (this.client != null)
                {
                    return NetworkingState.Client;
                }
                else
                {
                    return NetworkingState.NotConnected;
                }
            }
        }

        // connection
        private ConnectionConfig connectionConfig;
        private RoomInfo roomInfo;
        private State state;

        // tasks
        private UnityTask<List<MatchInfoSnapshot>> listMatchesTask;
        private UnityTask<MatchInfo> createMatchTask;
        private UnityTask<MatchInfo> joinMatchTask;

        // client state
        private MatchInfoSnapshot joinMatchInfoSnapshot;
        private MatchInfo clientMatchInfo;
        private NetworkClient client;

        // server state
        private MatchInfo serverMatchInfo;
        private SimpleServer server;

        // messages
        private Dictionary<short, Type> roomMessageTypes = new Dictionary<short, Type>();
        private Dictionary<short, List<RoomMessage>> roomMessagePools = new Dictionary<short, List<RoomMessage>>();

        // used to invalidate debug info and regenerate (so it wont happen every tick)
        private string debugInfoCache = null;
        private bool isDebugInfoDirty = true;

        // tracking user info
        private UserDisconnected userDisconnectedMessage = new UserDisconnected();
        private Dictionary<long, UserInfo> otherUsers = new Dictionary<long, UserInfo>();
        private Dictionary<int, UserInfo> connectionIdToUserInfoMap = new Dictionary<int, UserInfo>();
        private UserInfo myUserInfo;

        // used for copying messages
        private byte[] copyBytes;
        private NetworkWriter copyNetworkWriter;
        private NetworkReader copyNetworkReader;

        public UserInfo MyUserInfo
        {
            get { return this.myUserInfo; }
        }

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

            if (this.state == State.Searching)
            {
                if (this.roomInfo.RoomSearchName.IsNullOrWhitespace() == false)
                {
                    summary.Append("Room Search Name: ");
                    summary.AppendLine(this.roomInfo.RoomSearchName);
                }

                if (this.roomInfo.RoomSearchPassword.IsNullOrWhitespace() == false)
                {
                    summary.Append("Room Search Password: ");
                    summary.AppendLine(this.roomInfo.RoomSearchPassword);
                }
            }
            else if (this.state == State.Creating || this.state == State.ConnectedAsServer)
            {
                if (this.roomInfo.NewRoomName.IsNullOrWhitespace() == false)
                {
                    summary.Append("New Room Name: ");
                    summary.AppendLine(this.roomInfo.NewRoomName);
                }

                if (this.roomInfo.NewRoomPassword.IsNullOrWhitespace() == false)
                {
                    summary.Append("New Room Password: ");
                    summary.AppendLine(this.roomInfo.NewRoomPassword);
                }
            }
            else if (this.state == State.Joining || this.state == State.ConnectedAsClient)
            {
                if (this.roomInfo.RoomSearchName.IsNullOrWhitespace() == false)
                {
                    summary.Append("Room Search Name: ");
                    summary.AppendLine(this.roomInfo.RoomSearchName);
                }

                if (this.roomInfo.RoomSearchPassword.IsNullOrWhitespace() == false)
                {
                    summary.Append("Room Search Password: ");
                    summary.AppendLine(this.roomInfo.RoomSearchPassword);
                }

                if (this.joinMatchInfoSnapshot != null)
                {
                    summary.Append("Room Name: ");
                    summary.AppendLine(this.joinMatchInfoSnapshot.name);
                }
            }

            int userCount = (this.server != null || this.client != null) ? 1 : 0;
            userCount += this.otherUsers.Count;

            summary.Append("Room Count: ");
            summary.AppendLine(userCount.ToString());

            if (this.client != null)
            {
                summary.Append("Client IsConnected: ");
                summary.AppendLine(this.client.isConnected.ToString());
            }

            this.debugInfoCache = summary.ToString();
            this.isDebugInfoDirty = false;

            return this.debugInfoCache;
        }

        public void CreateOrJoinRoom(RoomInfo roomInfo, UserInfo userInfo)
        {
            this.myUserInfo = userInfo;

            if (this.state != State.Waiting)
            {
                this.SetState(State.ShuttingDown);
            }

            this.roomInfo = roomInfo;
            this.SetState(State.Searching);
        }

        public void UpdateUserInfo(UserInfo userInfo)
        {
            this.myUserInfo = userInfo;
            this.SendRoomMessage(this.myUserInfo);
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
            roomMessage.UserId = this.myUserInfo.UserId;

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

        public virtual void Shutdown()
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

                // tell the match maker we're shutting down
                MatchMakingHelper.DropConnection(this.clientMatchInfo);
                this.clientMatchInfo = null;
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

                // tell the match maker we're shutting down
                MatchMakingHelper.DestroyMatch(this.serverMatchInfo);
                this.serverMatchInfo = null;
            }

            this.roomInfo = null;
            this.joinMatchInfoSnapshot = null;
            this.otherUsers.Clear();
            this.connectionIdToUserInfoMap.Clear();

            if (this.state != State.Waiting)
            {
                this.SetState(State.Waiting);
            }
        }

        protected virtual void SetCurrentRoomInvisible()
        {
            if (this.NetworkingState == NetworkingState.Server)
            {
                MatchMakingHelper.SetMatchAttributes(this.serverMatchInfo, false);
            }
        }

        protected virtual void SetCurrentRoomVisible()
        {
            if (this.NetworkingState == NetworkingState.Server)
            {
                MatchMakingHelper.SetMatchAttributes(this.serverMatchInfo, true);
            }
        }

        protected virtual ConnectionConfig CreateConnectionConfig()
        {
            var config = new ConnectionConfig();
            config.AddChannel(QosType.Reliable);
            config.AddChannel(QosType.Unreliable);

            return config;
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

        protected abstract void RegisterCustomMessages();

        protected abstract void OnRoomMessageReceived(RoomMessage roomMessage);

        protected abstract MatchInfoSnapshot ChoseMatchInfoSnapshot(List<MatchInfoSnapshot> matches);

        protected abstract void ConnectedAsServer();

        protected abstract void ConnectedAsClient();

        protected abstract void DisconnectedAsServer();

        protected abstract void DisconnectedAsClient();

        protected abstract void UserJoined(UserInfo userInfo);

        protected abstract void UserInfoUpdated(UserInfo userInfo);

        protected abstract void UserLeft(UserInfo userInfo);

        protected virtual void Awake()
        {
            this.RegisterMessage<UserInfo>();
            this.RegisterMessage<UserDisconnected>();

            this.RegisterCustomMessages();
        }

        protected virtual void OnDestroy()
        {
            this.Shutdown();
        }

        protected virtual void Update()
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
                                    MatchInfoSnapshot matchInfoSnapShot = this.ChoseMatchInfoSnapshot(this.listMatchesTask.Value);

                                    if (matchInfoSnapShot != null)
                                    {
                                        this.joinMatchInfoSnapshot = matchInfoSnapShot;
                                        this.SetState(State.Joining);
                                    }
                                    else
                                    {
                                        this.SetState(State.Creating);
                                    }
                                }
                            }
                            else
                            {
                                Debug.LogErrorFormat("Restarting: Error Trying To List Matches: {0}", this.listMatchesTask.Exception.Message);
                                this.CreateOrJoinRoom(this.roomInfo, this.myUserInfo);
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

                                this.clientMatchInfo = joinMatchTask.Value;

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

                                this.client.Connect(this.clientMatchInfo);
                                this.SetState(State.ConnectedAsClient);
                            }
                            else
                            {
                                Debug.LogErrorFormat("Restarting: Error Trying To Join Match: {0}", this.joinMatchTask.Exception.Message);
                                this.CreateOrJoinRoom(this.roomInfo, this.myUserInfo);
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
                                this.CreateOrJoinRoom(this.roomInfo, this.myUserInfo);
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
                        this.listMatchesTask = MatchMakingHelper.ListMatches(this.roomInfo.RoomSearchName, this.roomInfo.RoomSearchPassword, this.roomInfo.Elo, false);
                        break;
                    }

                case State.Joining:
                    {
                        this.joinMatchTask = MatchMakingHelper.JoinMatch(this.joinMatchInfoSnapshot.networkId, this.roomInfo.RoomSearchPassword, this.roomInfo.Elo);
                        break;
                    }

                case State.Creating:
                    {
                        this.createMatchTask = MatchMakingHelper.CreateMatch(this.roomInfo.NewRoomName, this.roomInfo.MaxConnections, true, this.roomInfo.NewRoomPassword, this.roomInfo.Elo);
                        break;
                    }

                case State.ConnectedAsServer:
                    {
                        this.ConnectedAsServer();
                        break;
                    }

                case State.ConnectedAsClient:
                    {
                        this.ConnectedAsClient();
                        break;
                    }

                case State.DisconnectedAsClient:
                    {
                        this.SetState(State.ShuttingDown);
                        this.DisconnectedAsClient();
                        break;
                    }

                case State.DisconnectedAsServer:
                    {
                        this.SetState(State.ShuttingDown);
                        this.DisconnectedAsServer();
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

        private void Copy(RoomMessage to, RoomMessage from)
        {
            if (this.copyBytes == null)
            {
                this.copyBytes = new byte[4096];
                this.copyNetworkReader = new NetworkReader(this.copyBytes);
                this.copyNetworkWriter = new NetworkWriter(this.copyBytes);
            }

            this.copyNetworkWriter.SeekZero();
            this.copyNetworkReader.SeekZero();

            from.Serialize(this.copyNetworkWriter);

            if (copyNetworkWriter.Position > copyBytes.Length)
            {
                // NOTE [bgish]: Because the buffer grew, all 3 of these objects are now hosed (so recreate them)
                this.copyBytes = null;
                this.copyNetworkReader = null;
                this.copyNetworkWriter = null;

                // NOTE [bgish]: If the buffer grows, then we're hosed!  Tell the user that we shouldn't be sending more then 4k!
                Debug.LogError("UserInfo Message exceeded 4k!  Sending UserInfo will now leak memory.  Please limit size to under 4k");

                NetworkWriter newNetworkWriter = new NetworkWriter();
                from.Serialize(newNetworkWriter);

                NetworkReader newNetworkReader = new NetworkReader(newNetworkWriter);
                to.Deserialize(newNetworkReader);
            }
            else
            {
                to.Deserialize(copyNetworkReader);
            }
        }

        private void HandleRoomMessage(NetworkMessage networkMessage)
        {
            var roomMessageInstance = this.GetRoomMessage(networkMessage.msgType);
            roomMessageInstance.Deserialize(networkMessage.reader);

            short roomMessageId = roomMessageInstance.GetMessageId();

            // handling the UserInfo message
            if (roomMessageId == UserInfoMessage.MessageId)
            {
                var userInfo = (UserInfo)roomMessageInstance;

                // in a very rare case, the server could send back the clients info (this will make sure we don't do anything with it)
                if (userInfo.UserId == this.myUserInfo.UserId)
                {
                    return;
                }

                UserInfo currentUserInfo;
                if (this.otherUsers.TryGetValue(userInfo.UserId, out currentUserInfo))
                {
                    this.Copy(currentUserInfo, userInfo);
                    this.UserInfoUpdated(currentUserInfo);
                }
                else
                {
                    UserInfo newUser = new UserInfo();
                    this.Copy(newUser, userInfo);
                    this.otherUsers.Add(newUser.UserId, newUser);

                    if (this.server != null)
                    {
                        this.connectionIdToUserInfoMap.Add(networkMessage.conn.connectionId, newUser);
                    }

                    this.UserJoined(newUser);
                    this.isDebugInfoDirty = true;
                }
            }

            // handling user disconnected message (only happens on clients)
            if (this.client != null && roomMessageId == UserDisconnected.MessageId)
            {
                var userDisconnectedInfo = (UserDisconnected)roomMessageInstance;

                UserInfo userInfo;
                if (this.otherUsers.TryGetValue(userDisconnectedInfo.DisconnectedUserId, out userInfo))
                {
                    this.otherUsers.Remove(userDisconnectedInfo.DisconnectedUserId);

                    this.UserLeft(userInfo);
                    this.isDebugInfoDirty = true;
                }
            }

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
                            connection.Send(roomMessageId, roomMessageInstance);
                        }
                        else
                        {
                            connection.SendUnreliable(roomMessageId, roomMessageInstance);
                        }
                    }
                }
            }

            if (roomMessageId != UserInfoMessage.MessageId && roomMessageId != UserDisconnected.MessageId)
            {
                this.OnRoomMessageReceived(roomMessageInstance);
            }
        }

        #region Client Callbacks

        // this is called when the client has successfully connected to the server
        private void Client_OnConnectInternal(NetworkMessage message)
        {
            Debug.Log("Client_OnConnectInternal");
            this.SendRoomMessage(this.myUserInfo);
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
            // TODO [bgish]: Actually have no clue how we should handle this
            Debug.LogError("Client_OnNotReadyMessageInternal");
            this.isDebugInfoDirty = true;
        }

        private void Client_OnErrorInternal(NetworkMessage message)
        {
            // TODO [bgish]: Actually have no clue how we should handle this
            Debug.LogError("Client_OnErrorInternal");
            this.isDebugInfoDirty = true;
        }

        #endregion

        #region Simple Server Callbacks

        private void Server_OnDisconnectErrorEvent(UnityEngine.Networking.NetworkConnection conn, byte error)
        {
            // TODO [bgish]: Need to send client disconnected event to everyone
            Debug.LogErrorFormat("Server_OnDisconnectErrorEvent: {0}", error);
            this.SetState(State.DisconnectedAsServer);
            this.isDebugInfoDirty = true;
        }

        private void Server_OnDataErrorEvent(UnityEngine.Networking.NetworkConnection conn, byte error)
        {
            // TODO [bgish]: Actually have no clue how we should handle this
            Debug.LogErrorFormat("Server_OnDataErrorEvent: {0}", error);
            this.isDebugInfoDirty = true;
        }

        private void Server_OnConnectErrorEvent(int connectionId, byte error)
        {
            // TODO [bgish]: Actually have no clue how we should handle this
            Debug.LogErrorFormat("Server_OnConnectErrorEvent: {0}", error);
            this.isDebugInfoDirty = true;
        }

        private void Server_OnConnectedEvent(UnityEngine.Networking.NetworkConnection conn)
        {
            // sending this new user my info
            conn.Send(this.myUserInfo.GetMessageId(), this.myUserInfo);

            // sending this new user all the other infos I currently know about
            foreach (var user in this.otherUsers.Values)
            {
                conn.Send(user.GetMessageId(), user);
            }

            this.isDebugInfoDirty = true;
        }

        private void Server_OnDisconnectedEvent(NetworkConnection conn)
        {
            Debug.Log("Server_OnDisconnectedEvent");

            UserInfo userInfo;
            if (this.connectionIdToUserInfoMap.TryGetValue(conn.connectionId, out userInfo))
            {
                this.connectionIdToUserInfoMap.Remove(conn.connectionId);
                this.otherUsers.Remove(userInfo.UserId);

                // sending the disconnect event to all client users
                this.userDisconnectedMessage.DisconnectedUserId = userInfo.UserId;
                this.SendRoomMessage(this.userDisconnectedMessage);

                this.UserLeft(userInfo);
            }
            else
            {
                // Do Nothing, if the client disconnected before they had a chance to send their
                // UserInfo object, then no need to tell everyone that they've disconnected.
            }

            // TODO [bgish]: Investigate if the following could happen.  These events would mean that all users
            //               would think the person exists, even though they've been disconnected already.
            //
            // Server get Connected
            // Client sends UserInfo
            // Server gets Disconnected
            // Server then processes UserInfo
            //

            this.isDebugInfoDirty = true;
        }

        #endregion
    }
}
