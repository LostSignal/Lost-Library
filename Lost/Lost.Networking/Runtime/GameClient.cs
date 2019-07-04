//-----------------------------------------------------------------------
// <copyright file="GameClient.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;

    //// TODO [bgish]: Keep a bool called isConnectedToServer and turn it on/off where appropiate
    ////               If the user tries to send a message while not connected, then throw an error.

    public abstract class GameClient
    {
        private string lastKnownConnectionString = null;
        private IClientTransportLayer transportLayer;
        private MessageCollection messageCollection;
        private string joinServerCustomData;
        private UserInfo myUserInfo;

        // used for serializing messages to a byte buffer
        private NetworkWriter messageWriter = new NetworkWriter();

        // user/connection management
        private Dictionary<long, UserInfo> userIdToUserInfoMap = new Dictionary<long, UserInfo>();
        private HashSet<long> knownUserIds = new HashSet<long>();
        private List<UserInfo> users = new List<UserInfo>();

        private ReadOnlyCollection<UserInfo> readonlyUsersList;

        public ReadOnlyCollection<UserInfo> ConnectedUsers
        {
            get
            {
                if (this.readonlyUsersList == null)
                {
                    this.readonlyUsersList = new ReadOnlyCollection<UserInfo>(this.users);
                }

                return this.readonlyUsersList;
            }
        }

        public long UserId { get; private set; }

        public bool HasJoinedServer { get; private set; }

        public bool IsConnecting
        {
            get { return this.transportLayer.IsConnecting; }
        }

        public bool IsConnected
        {
            get { return this.transportLayer.IsConnected; }
        }

        public GameClient(IClientTransportLayer transportLayer, UserInfo myUserInfo, string joinServerCustomData)
        {
            this.transportLayer = transportLayer;

            this.UserId = myUserInfo.UserId;

            this.myUserInfo = new UserInfo();
            this.myUserInfo.CopyFrom(myUserInfo);

            this.joinServerCustomData = joinServerCustomData;

            this.messageCollection = new MessageCollection();

            // client to server
            this.messageCollection.RegisterMessage<JoinServerRequestMessage>();
            this.messageCollection.RegisterMessage<UpdateUserInfoMessage>();

            // server to client
            this.messageCollection.RegisterMessage<JoinServerResponseMessage>();
            this.messageCollection.RegisterMessage<UserDisconnectedMessage>();
            this.messageCollection.RegisterMessage<UserInfoMessage>();

            this.RegisterCustomMessages(this.messageCollection);
        }

        public void RegisterMessage<T>() where T : Message, new()
        {
            this.messageCollection.RegisterMessage<T>();
        }

        public virtual void Connect(string connectionString)
        {
            Debug.Assert(string.IsNullOrEmpty(connectionString) == false, "Can't connect to null or empty connection string!");

            if (this.lastKnownConnectionString != null && this.lastKnownConnectionString != connectionString && this.IsConnected)
            {
                this.Shutdown();
            }

            else if (this.IsConnected && this.lastKnownConnectionString == connectionString)
            {
                return;
            }

            // if we're trying to connect to a new server, then reset everything
            if (connectionString != this.lastKnownConnectionString)
            {
                this.Reset();
                this.lastKnownConnectionString = connectionString;
            }

            if (this.transportLayer.IsConnected == false)
            {
                this.transportLayer.Connect(connectionString);
            }
        }

        public virtual void SendMessage(Message message)
        {
            if (this.HasJoinedServer == false && message.GetId() != JoinServerRequestMessage.Id)
            {
                // TODO [bgish]: throw some sort of error?
                return;
            }

            this.messageWriter.SeekZero();
            message.Serialize(this.messageWriter);
            this.transportLayer.SendData(messageWriter.RawBuffer, 0, (uint)messageWriter.Position);
        }

        public virtual void Shutdown()
        {
            this.Reset();
        }

        public virtual void Update()
        {
            this.transportLayer?.Update();

            ClientEvent clientEvent;

            while (this.transportLayer.TryDequeueClientEvent(out clientEvent))
            {
                switch (clientEvent.EventType)
                {
                    case ClientEventType.ConnectionOpened:
                        this.ConnectionOpened();
                        break;

                    case ClientEventType.ConnectionClosed:
                        this.ConnectionClosed();
                        break;

                    case ClientEventType.ConnectionLost:
                        this.ConnectionLost();
                        break;

                    case ClientEventType.ReceivedData:
                        this.ReceivedData(clientEvent.Data);
                        break;

                    default:
                        Debug.LogErrorFormat("GameClient: Found unknown ClientEvent type {0}", clientEvent.EventType);
                        break;
                }
            }
        }

        public void UpdateUserInfo(UserInfo myUserInfo)
        {
            this.myUserInfo.CopyFrom(myUserInfo);

            var requestUpdateUserInfo = (UpdateUserInfoMessage)this.messageCollection.GetMessage(UpdateUserInfoMessage.Id);
            requestUpdateUserInfo.UserInfo = this.myUserInfo;

            this.SendMessage(requestUpdateUserInfo);

            this.messageCollection.RecycleMessage(requestUpdateUserInfo);
        }

        public delegate void ClientUserConnectedDelegate(UserInfo userInfo, bool wasReconnect);
        public delegate void ClientUserDisconnectedDelegate(UserInfo userInfo, bool wasConnectionLost);
        public delegate void ClientUserInfoUpdatedDelegate(UserInfo userInfo);
        public delegate void ClientReceivedMessageDelegate(Message message);
        public delegate void ClientConnectedToServerDelegate();
        public delegate void ClientFailedToConnectToServerDelegate();
        public delegate void ClientDisconnectedFromServerDelegate();
        public delegate void ClientLostConnectionToServerDelegate();

        public ClientUserConnectedDelegate ClientUserConnected;
        public ClientUserDisconnectedDelegate ClientUserDisconnected;
        public ClientUserInfoUpdatedDelegate ClientUserInfoUpdated;
        public ClientReceivedMessageDelegate ClientReceivedMessage;
        public ClientConnectedToServerDelegate ClientConnectedToServer;
        public ClientFailedToConnectToServerDelegate ClientFailedToConnectToServer;
        public ClientDisconnectedFromServerDelegate ClientDisconnectedFromServer;
        public ClientLostConnectionToServerDelegate ClientLostConnectionToServer;

        public abstract void RegisterCustomMessages(MessageCollection messageCollection);

        public abstract void UserConneceted(UserInfo userInfo, bool wasReconnect);

        public abstract void UserDisconnected(UserInfo userInfo, bool wasConnectionLost);

        public abstract void UserInfoUpdated(UserInfo userInfo);

        public abstract void ReceivedMessage(Message message);

        public abstract void ConnectedToServer();

        public abstract void FailedToConnectToServer();

        public abstract void DisconnectedFromServer();

        public abstract void LostConnectionToServer();

        private void ConnectionOpened()
        {
            var requestJoinServer = (JoinServerRequestMessage)this.messageCollection.GetMessage(JoinServerRequestMessage.Id);
            requestJoinServer.UserInfo = this.myUserInfo;
            requestJoinServer.CustomData = this.joinServerCustomData;

            this.SendMessage(requestJoinServer);

            this.messageCollection.RecycleMessage(requestJoinServer);
        }

        private void ConnectionClosed()
        {
            this.Reset();
            this.DisconnectedFromServer();
            this.ClientDisconnectedFromServer?.Invoke();
        }

        private void ConnectionLost()
        {
            // NOTE [bgish]: If we lost connectoin, we may want to reconnect, so lets say
            //               everyone's connection was lost and not reset everything.
            for (int i = 0; i < this.users.Count; i++)
            {
                this.UserDisconnected(this.users[i], true);
                this.ClientUserDisconnected?.Invoke(this.users[i], true);
            }

            this.users.Clear();
            this.userIdToUserInfoMap.Clear();
            this.HasJoinedServer = false;

            this.LostConnectionToServer();
            this.ClientLostConnectionToServer?.Invoke();
        }

        private void ReceivedData(byte[] data)
        {
            Message message = this.messageCollection.GetMessage(data);

            switch (message.GetId())
            {
                case JoinServerResponseMessage.Id:
                    var joinServerResponse = (JoinServerResponseMessage)message;
                    Debug.LogFormat("GameClient: JoinServerResponseMessage.Accepted = {0}", joinServerResponse.Accepted);

                    if (joinServerResponse.Accepted)
                    {
                        this.HasJoinedServer = true;
                        this.ConnectedToServer();
                        this.ClientConnectedToServer?.Invoke();
                    }
                    else
                    {
                        this.FailedToConnectToServer();
                        this.ClientFailedToConnectToServer?.Invoke();
                    }

                    break;

                case UserInfoMessage.Id:
                    {
                        var userConnectedMessage = (UserInfoMessage)message;

                        if (userConnectedMessage.UserInfo.UserId != this.UserId)
                        {
                            Debug.LogFormat("GameClient: UserInfoMessage For UserId {0}", userConnectedMessage.UserInfo.UserId);
                            this.AddOrUpdateUserInfo(userConnectedMessage.UserInfo);
                        }

                        break;
                    }

                case UserDisconnectedMessage.Id:
                    {
                        var userDisconnectedMessage = (UserDisconnectedMessage)message;
                        long userId = userDisconnectedMessage.UserId;

                        Debug.LogFormat("GameClient: UserDisconnectedMessage For UserId {0}", userDisconnectedMessage.UserId);

                        UserInfo removedUserInfo = this.RemoveUserInfo(userId);

                        if (removedUserInfo != null)
                        {
                            this.UserDisconnected(removedUserInfo, userDisconnectedMessage.WasConnectionLost);
                            this.ClientUserDisconnected?.Invoke(removedUserInfo, userDisconnectedMessage.WasConnectionLost);
                        }

                        break;
                    }

                default:
                    break;
            }

            this.ReceivedMessage(message);
            this.ClientReceivedMessage?.Invoke(message);

            this.messageCollection.RecycleMessage(message);
        }

        private UserInfo AddOrUpdateUserInfo(UserInfo messageUserInfo)
        {
            UserInfo userInfo;
            if (this.userIdToUserInfoMap.TryGetValue(messageUserInfo.UserId, out userInfo))
            {
                userInfo.CopyFrom(messageUserInfo);

                this.UserInfoUpdated(userInfo);
                this.ClientUserInfoUpdated?.Invoke(userInfo);
            }
            else
            {
                userInfo = new UserInfo();
                userInfo.CopyFrom(messageUserInfo);

                this.userIdToUserInfoMap.Add(userInfo.UserId, userInfo);
                this.users.Add(userInfo);

                bool wasReconnect = this.knownUserIds.Contains(userInfo.UserId);
                this.knownUserIds.Add(userInfo.UserId);

                this.UserConneceted(userInfo, wasReconnect);
                this.ClientUserConnected?.Invoke(userInfo, wasReconnect);
            }

            return userInfo;
        }

        private UserInfo RemoveUserInfo(long userId)
        {
            UserInfo userInfo;
            if (this.userIdToUserInfoMap.TryGetValue(userId, out userInfo))
            {
                this.userIdToUserInfoMap.Remove(userId);
                this.users.Remove(userInfo);

                return userInfo;
            }

            return null;
        }

        private void Reset()
        {
            this.lastKnownConnectionString = null;
            this.HasJoinedServer = false;
            this.users.Clear();
            this.userIdToUserInfoMap.Clear();
            this.knownUserIds.Clear();

            if (this.transportLayer.IsConnected)
            {
                this.transportLayer.Shutdown();
            }
        }
    }
}
