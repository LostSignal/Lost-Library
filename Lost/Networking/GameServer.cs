//-----------------------------------------------------------------------
// <copyright file="GameServer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;

    public abstract class GameServer
    {
        private IServerTransportLayer transportLayer;
        private MessageCollection messageCollection;

        // used for serializing messages to a byte buffer
        private NetworkWriter messageWriter = new NetworkWriter();

        // user management
        private Dictionary<long, UserInfo> connectionIdToUserInfoMap = new Dictionary<long, UserInfo>();
        private List<UserInfo> users = new List<UserInfo>();
        private HashSet<long> knownUserIds = new HashSet<long>();
        private ReadOnlyCollection<UserInfo> readonlyUsersList;

        // tracking data
        private ServerStats stats = new ServerStats();

        public IServerStats Stats
        {
            get { return this.stats; }
        }

        public bool IsStarting
        {
            get { return this.transportLayer.IsStarting; }
        }

        public bool IsRunning
        {
            get { return this.transportLayer.IsRunning; }
        }

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

        public GameServer(IServerTransportLayer transportLayer)
        {
            this.transportLayer = transportLayer;

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

        public bool Start(int port)
        {
            if (this.transportLayer.IsRunning == false)
            {
                this.stats.StartTime = DateTime.UtcNow;
                this.transportLayer.Start(port);

                if (this.transportLayer.IsRunning)
                {
                    if (this.OnServerStart())
                    {
                        return true;
                    }
                    else
                    {
                        this.transportLayer.Shutdown();
                        return false;
                    }
                }
            }

            return true;
        }

        public virtual void Shutdown()
        {
            if (this.transportLayer.IsRunning)
            {
                this.stats.ShutdownTime = DateTime.UtcNow;
                this.transportLayer.Shutdown();

                this.OnServerShutdown();
            }
        }

        public void SendMessageToUser(UserInfo userInfo, Message message)
        {
            this.SendMessageToConnection(userInfo.ConnectionId, message);
        }

        public void SendMessageToConnection(long connectionId, Message message)
        {
            byte[] data;
            uint length;
            this.GetDataFromMessage(message, out data, out length);

            this.SendData(connectionId, data, length);
        }

        public void SendMessageToAll(Message message)
        {
            byte[] data;
            uint length;
            this.GetDataFromMessage(message, out data, out length);

            for (int i = 0; i < this.users.Count; i++)
            {
                this.SendData(this.users[i].ConnectionId, data, length);
            }
        }

        public void SendMessageToAllExcept(UserInfo userInfo, Message message)
        {
            byte[] data;
            uint length;
            this.GetDataFromMessage(message, out data, out length);

            for (int i = 0; i < this.users.Count; i++)
            {
                if (this.users[i].UserId == userInfo.UserId)
                {
                    continue;
                }

                this.SendData(this.users[i].ConnectionId, data, length);
            }
        }

        public abstract bool CanUserJoinServer(JoinServerRequestMessage joinServerRequest);

        public abstract void RegisterCustomMessages(MessageCollection messageCollection);

        public abstract void UserConneceted(UserInfo userInfo, bool isReconnect);

        public abstract void UserInfoUpdated(UserInfo userInfo);

        public abstract void UserDisconnected(UserInfo userInfo, bool wasConnectionLost);

        public abstract void ReceivedMessage(UserInfo userInfo, Message message);

        public abstract bool OnServerStart();

        public abstract void OnUserClosedConnection(UserInfo userInfo);

        public abstract void OnServerShutdown();

        public virtual void Update()
        {
            ServerEvent serverEvent;

            while (this.transportLayer.TryDequeueServerEvent(out serverEvent))
            {
                switch (serverEvent.EventType)
                {
                    case ServerEventType.ConnectionOpened:
                        this.ConnectionOpened(serverEvent.ConnectionId);
                        break;

                    case ServerEventType.ConnectionClosed:
                        this.ConnectionClosed(serverEvent.ConnectionId);
                        break;

                    case ServerEventType.ConnectionLost:
                        this.ConnectionLost(serverEvent.ConnectionId);
                        break;

                    case ServerEventType.ReceivedData:
                        this.ReceivedData(serverEvent.ConnectionId, serverEvent.Data);
                        break;

                    default:
                        Debug.LogErrorFormat("GameServer: Found unknown ServerEvent type {0}", serverEvent.EventType);
                        break;
                }
            }
        }

        private void ConnectionOpened(long connectionId)
        {
            // Do nothing about this connection till we get a RequestJoinServerMessage
        }

        private void ConnectionClosed(long connectionId)
        {
            this.CloseConnection(connectionId, false);
        }

        private void ConnectionLost(long connectionId)
        {
            this.stats.NumberOfConnectionDrops++;
            this.CloseConnection(connectionId, true);
        }

        private void ReceivedData(long connectionId, byte[] data)
        {
            Message message = this.messageCollection.GetMessage(data);

            UserInfo userInfo = null;
            this.connectionIdToUserInfoMap.TryGetValue(connectionId, out userInfo);

            switch (message.GetId())
            {
                case JoinServerRequestMessage.Id:
                    {
                        var requestJoinServer = (JoinServerRequestMessage)message;

                        // checking if this connection has already registered a user info, if so they've already joined
                        if (userInfo != null)
                        {
                            Debug.LogErrorFormat("GameServer: Got a JoinServerRequestMessage when the ConnectionId {0} has already joined.", userInfo.ConnectionId);
                            break;
                        }

                        Debug.LogFormat("GameServer: Received JoinServerRequestMessage from UserId {0}", requestJoinServer.UserInfo.UserId);

                        if (requestJoinServer.UserInfo != null)
                        {
                            requestJoinServer.UserInfo.ConnectionId = connectionId;
                        }
                        else
                        {
                            Debug.LogError("GameServer: JoinServerRequestMessage had a null UserInfo object.");
                        }

                        bool canUserJoinServer = requestJoinServer.UserInfo != null && this.CanUserJoinServer(requestJoinServer);

                        // send the response
                        var joinServerResponse = (JoinServerResponseMessage)this.messageCollection.GetMessage(JoinServerResponseMessage.Id);
                        joinServerResponse.Accepted = canUserJoinServer;

                        Debug.LogFormat("GameServer: Sending JoinServerResponseMessage to UserId {0} With Accepted = {1}", requestJoinServer.UserInfo.UserId, canUserJoinServer);

                        this.SendMessageToConnection(connectionId, joinServerResponse);

                        this.messageCollection.RecycleMessage(joinServerResponse);

                        // add this user to the list if they were able to join
                        if (canUserJoinServer)
                        {
                            this.AddOrUpdateUserInfo(requestJoinServer.UserInfo);
                        }

                        break;
                    }

                case UpdateUserInfoMessage.Id:
                    {
                        if (userInfo == null)
                        {
                            Debug.LogError("GameServer: Unregistered user tried to send a messages.");
                            break;
                        }

                        var updateUserInfoMessage = (UpdateUserInfoMessage)message;

                        if (updateUserInfoMessage.UserInfo != null)
                        {
                            updateUserInfoMessage.UserInfo.ConnectionId = connectionId;
                        }
                        else
                        {
                            Debug.LogError("GameServer: UpdateUserInfoMessage had a null UserInfo object.");
                            break;
                        }

                        Debug.LogFormat("GameServer: Received UpdateUserInfoMessage from UserId {0}", updateUserInfoMessage.UserInfo.UserId);

                        // checking is someone is trying to hack by changing their userId to something else
                        if (userInfo.UserId != updateUserInfoMessage.UserInfo.UserId)
                        {
                            Debug.LogErrorFormat("GameServer: User {0} is trying to change thier Id to {1}", userInfo.UserId, updateUserInfoMessage.UserInfo.UserId);
                        }
                        else
                        {
                            this.AddOrUpdateUserInfo(updateUserInfoMessage.UserInfo);
                        }

                        break;
                    }

                default:
                    {
                        if (userInfo == null)
                        {
                            Debug.LogError("GameServer: Unregistered user tried to send a messages.");
                        }
                        else
                        {
                            this.ReceivedMessage(userInfo, message);
                        }

                        break;
                    }
            }

            this.messageCollection.RecycleMessage(message);
        }

        private void AddOrUpdateUserInfo(UserInfo userInfoUpdate)
        {
            UserInfoMessage userInfoMessage = null;
            UserInfo existingUserInfo = null;
            UserInfo newUserInfo = null;

            for (int i = 0; i < this.users.Count; i++)
            {
                if (this.users[i].UserId == userInfoUpdate.UserId)
                {
                    existingUserInfo = this.users[i];
                }
            }

            // checking if this is just a user info updated
            if (existingUserInfo != null)
            {
                existingUserInfo.CopyFrom(userInfoUpdate);
                this.UserInfoUpdated(existingUserInfo);
            }
            else
            {
                // if we got here, then this is a newly connected user
                newUserInfo = new UserInfo();
                newUserInfo.CopyFrom(userInfoUpdate);

                bool isReconnect = this.knownUserIds.Contains(newUserInfo.UserId);
                this.UserConneceted(newUserInfo, isReconnect);

                this.knownUserIds.Add(newUserInfo.UserId);
                this.stats.NumberOfReconnects += (uint)(isReconnect ? 1 : 0);

                // Since this user is new, send them all the known user infos to them
                userInfoMessage = (UserInfoMessage)this.messageCollection.GetMessage(UserInfoMessage.Id);

                for (int i = 0; i < this.users.Count; i++)
                {
                    userInfoMessage.UserInfo = this.users[i];
                    this.SendMessageToUser(newUserInfo, userInfoMessage);
                }

                this.messageCollection.RecycleMessage(userInfoMessage);

                // making sure this user is now in the list and map
                this.users.Add(newUserInfo);
            }

            UserInfo user = newUserInfo ?? existingUserInfo;

            // telling all other users that someone's info has been updated
            userInfoMessage = (UserInfoMessage)this.messageCollection.GetMessage(UserInfoMessage.Id);
            userInfoMessage.UserInfo = user;

            this.SendMessageToAllExcept(user, userInfoMessage);

            this.messageCollection.RecycleMessage(userInfoMessage);

            // making sure the user's connectionId is in the map
            if (this.connectionIdToUserInfoMap.ContainsKey(user.ConnectionId) == false)
            {
                this.connectionIdToUserInfoMap.Add(user.ConnectionId, user);
            }

            // updating max connected stat
            if (this.users.Count > this.stats.MaxConnectedUsers)
            {
                this.stats.MaxConnectedUsers = (uint)this.users.Count;
            }
        }

        private void CloseConnection(long connectionId, bool connectionLost)
        {
            UserInfo userInfo;
            if (this.connectionIdToUserInfoMap.TryGetValue(connectionId, out userInfo) == false)
            {
                return;
            }

            if (userInfo.ConnectionId != connectionId)
            {
                Debug.LogErrorFormat("Trying to close old connection {0}.  User {1} already has connectionId {2}", connectionId, userInfo.UserId, userInfo.ConnectionId);
                return;
            }

            // removing this connection from all our maps and lists
            this.connectionIdToUserInfoMap.Remove(userInfo.ConnectionId);

            int usersRemovedCount = 0;
            for (int i = this.users.Count - 1; i >= 0; i--)
            {
                if (this.users[i].UserId == userInfo.UserId)
                {
                    this.users.RemoveAt(i);
                    usersRemovedCount++;
                }
            }

            Debug.AssertFormat(usersRemovedCount == 1, "Couldn't properly remove users {0}.  Found {1} when should have found 1", userInfo.UserId, usersRemovedCount);

            // send the disconnected message to all remaining users
            var userDisconnected = (UserDisconnectedMessage)this.messageCollection.GetMessage(UserDisconnectedMessage.Id);
            userDisconnected.UserId = userInfo.UserId;
            userDisconnected.WasConnectionLost = connectionLost;
            this.SendMessageToAll(userDisconnected);
            this.messageCollection.RecycleMessage(userDisconnected);

            if (connectionLost == false)
            {
                this.OnUserClosedConnection(userInfo);
            }

            // telling child classes of the disconnect
            this.UserDisconnected(userInfo, connectionLost);
        }

        private void SendData(long connectionId, byte[] data, uint length)
        {
            if (this.transportLayer.IsRunning)
            {
                this.stats.MessagesSent += 1;
                this.stats.BytesSent += length;
                this.transportLayer.SendData(connectionId, data, 0, length);
            }
        }

        private void GetDataFromMessage(Message message, out byte[] data, out uint length)
        {
            this.messageWriter.SeekZero();
            message.Serialize(this.messageWriter);

            data = messageWriter.RawBuffer;
            length = (uint)messageWriter.Position;
        }
    }

    public interface IServerStats
    {
        DateTime StartTime { get; }
        DateTime ShutdownTime { get; }
        uint MessagesSent { get; }
        uint BytesSent { get; }
        uint MaxConnectedUsers { get; }
        uint NumberOfReconnects { get; }
        uint NumberOfConnectionDrops { get; }
    }

    public class ServerStats : IServerStats
    {
        public DateTime StartTime { get; set; }
        public DateTime ShutdownTime { get; set; }
        public uint MessagesSent { get; set; }
        public uint BytesSent { get; set; }
        public uint MaxConnectedUsers { get; set; }
        public uint NumberOfReconnects { get; set; }
        public uint NumberOfConnectionDrops { get; set; }
    }
}
