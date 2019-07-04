//-----------------------------------------------------------------------
// <copyright file="UnityGameServer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_2018_3_OR_NEWER

namespace Lost
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Lost.Networking;
    using UnityEngine;

    public class UnityGameServer
    {
        private static readonly NetworkReader reader = new NetworkReader(new byte[0]);

        // Caching Members
        private static readonly NetworkIdentityUpdate networkIdentityUpdateCache = new NetworkIdentityUpdate();
        private static readonly NetworkIdentitiesDestroyed networkIdentitiesDestroyedCache = new NetworkIdentitiesDestroyed();
        private static Dictionary<string, NetworkIdentity> resourcePrefabCache = new Dictionary<string, NetworkIdentity>();

        // Dynamic Network Object Tracking
        private Dictionary<long, NetworkIdentity> dynamicNetworkObjectsHash = new Dictionary<long, NetworkIdentity>();
        private List<NetworkIdentity> dynamicNetworkObjectsList = new List<NetworkIdentity>();

        // Static Network Object Tracking
        private Dictionary<long, NetworkIdentity> staticNetworkObjectsHash = new Dictionary<long, NetworkIdentity>();
        private List<NetworkIdentity> staticNetworkObjectsList = new List<NetworkIdentity>();
        private HashSet<long> destroyedStaticNetworkObjects = new HashSet<long>();

        NetworkIdentityCreated networkIdentityCreatedMessage = new NetworkIdentityCreated();
        private GameServer gameServer;

        public ReadOnlyCollection<UserInfo> ConnectedUsers => this.gameServer.ConnectedUsers;

        public UnityGameServer(GameServer gameServer)
        {
            this.StartGameServer(gameServer);
        }

        public void Update()
        {
            this.gameServer?.Update();
        }

        public void Shutdown()
        {
            this.gameServer?.Shutdown();

            if (Platform.IsApplicationQuitting == false)
            {
                // Destroy all dynamic objects
                foreach (var dynamic in this.dynamicNetworkObjectsList)
                {
                    Pooler.Destroy(dynamic.gameObject);
                }
            }
        }

        public NetworkIdentity CreateNetworkIdentity(string resourceName, long ownerId, Vector3 position)
        {
            NetworkIdentity networkIdentityPrefab = null;

            if (resourcePrefabCache.TryGetValue(resourceName, out networkIdentityPrefab) == false)
            {
                networkIdentityPrefab = Resources.Load<NetworkIdentity>(resourceName);
                resourcePrefabCache.Add(resourceName, networkIdentityPrefab);
            }

            var newNetworkIdentity = Pooler.Instantiate(networkIdentityPrefab);
            newNetworkIdentity.GenerateNewNetworkId();
            newNetworkIdentity.SetOwner(ownerId);
            newNetworkIdentity.ResourceName = resourceName;
            newNetworkIdentity.Destroyed += this.NetworkIdentityDestroyed;
            newNetworkIdentity.transform.position = position;

            // Registering it with the dynamic game objects
            this.dynamicNetworkObjectsHash.Add(newNetworkIdentity.NetworkId, newNetworkIdentity);
            this.dynamicNetworkObjectsList.Add(newNetworkIdentity);

            this.networkIdentityCreatedMessage.NetworkId = newNetworkIdentity.NetworkId;
            this.networkIdentityCreatedMessage.OwnerId = newNetworkIdentity.OwnerId;
            this.networkIdentityCreatedMessage.ResourceName = resourceName;
            this.networkIdentityCreatedMessage.Position = position;
            this.SendNetworkMessage(this.networkIdentityCreatedMessage);

            return newNetworkIdentity;
        }

        public void SendNetworkMessage(Message message)
        {
            this.gameServer?.SendMessageToAll(message);
        }

        private void StartGameServer(GameServer gameServer)
        {
            this.gameServer = gameServer;

            // Registering Custom Messages
            this.gameServer.RegisterMessage<NetworkIdentityUpdate>();
            this.gameServer.RegisterMessage<NetworkIdentitiesDestroyed>();
            this.gameServer.RegisterMessage<NetworkBehaviourMessage>();
            this.gameServer.RegisterMessage<NetworkIdentityCreated>();

            // Messages
            this.gameServer.ServerReceivedMessage += this.ServerReceivedMessage;

            // Startup/Shutdown
            this.gameServer.ServerStarted += this.ServerStarted;
            this.gameServer.ServerShutdown += this.ServerShutdown;

            // User Connected / Disconnected
            this.gameServer.ServerUserConnected += this.ServerUserConnected;
            this.gameServer.ServerUserInfoUpdated += this.ServerUserInfoUpdated;
            this.gameServer.ServerUserDisconnected += this.ServerUserDisconnected;

            // Collecting all the static network objects
            NetworkManager.GetAllNetworkIdentities(this.staticNetworkObjectsList);

            // Hashing all the static network objects for quicker lookup
            foreach (var identity in this.staticNetworkObjectsList)
            {
                this.staticNetworkObjectsHash.Add(identity.NetworkId, identity);
                identity.Destroyed += this.NetworkIdentityDestroyed;
            }
        }

        private void NetworkIdentityDestroyed(long networkId)
        {
            if (this.staticNetworkObjectsHash.TryGetValue(networkId, out NetworkIdentity staticNetworkIdentity))
            {
                staticNetworkIdentity.Destroyed = null;

                this.staticNetworkObjectsHash.Remove(networkId);
                this.staticNetworkObjectsList.Remove(staticNetworkIdentity);
                this.destroyedStaticNetworkObjects.Add(networkId);

                // Telling all the clients it was destroyed
                networkIdentitiesDestroyedCache.DestroyedNetworkIds.Clear();
                networkIdentitiesDestroyedCache.DestroyedNetworkIds.Add(networkId);
                this.gameServer.SendMessageToAll(networkIdentitiesDestroyedCache);
            }
            else if (this.dynamicNetworkObjectsHash.TryGetValue(networkId, out NetworkIdentity dynamicNetworkIdentity))
            {
                dynamicNetworkIdentity.Destroyed = null;

                this.dynamicNetworkObjectsHash.Remove(networkId);
                this.dynamicNetworkObjectsList.Remove(dynamicNetworkIdentity);

                // Telling all the clients it was destroyed
                networkIdentitiesDestroyedCache.DestroyedNetworkIds.Clear();
                networkIdentitiesDestroyedCache.DestroyedNetworkIds.Add(networkId);
                this.gameServer.SendMessageToAll(networkIdentitiesDestroyedCache);
            }
            else
            {
                Debug.LogError("WTF");
            }
        }

        private void ServerReceivedMessage(UserInfo userInfo, Message message)
        {
            switch (message.GetId())
            {
                case NetworkIdentityCreated.Id:
                    {
                        // Ignore for now, can't get this till ownership changing exists
                        break;
                    }

                case NetworkIdentitiesDestroyed.Id:
                    {
                        // Ignore for now, can't get this till ownership changing exists
                        break;
                    }

                case NetworkIdentityUpdate.Id:
                    {
                        // Ignore for now, can't get this till ownership changing exists
                        break;
                    }

                case NetworkBehaviourMessage.Id:
                    {
                        var networkBehaviourMessage = (NetworkBehaviourMessage)message;

                        this.staticNetworkObjectsHash.TryGetValue(networkBehaviourMessage.NetworkId, out NetworkIdentity staticIdentity);
                        this.dynamicNetworkObjectsHash.TryGetValue(networkBehaviourMessage.NetworkId, out NetworkIdentity dynamicIdentity);

                        var identity = staticIdentity ?? dynamicIdentity;

                        if (identity != null && identity.IsOwner == false)
                        {
                            reader.Replace(networkBehaviourMessage.Data);
                            identity.Behaviours[networkBehaviourMessage.BehaviourIndex].Deserialize(reader);
                        }

                        this.gameServer.SendMessageToAllExcept(userInfo, message);

                        break;
                    }
            }
        }

        private void ServerUserConnected(UserInfo userInfo, bool isReconnect)
        {
            Update(this.staticNetworkObjectsList);
            Update(this.dynamicNetworkObjectsList);

            // Telling this user what static network objects have been destroyed
            networkIdentitiesDestroyedCache.DestroyedNetworkIds.Clear();
            networkIdentitiesDestroyedCache.DestroyedNetworkIds.AddRange(this.destroyedStaticNetworkObjects);
            this.gameServer.SendMessageToAll(networkIdentitiesDestroyedCache);

            void Update(List<NetworkIdentity> identities)
            {
                for (int identityIndex = 0; identityIndex < identities.Count; identityIndex++)
                {
                    networkIdentityUpdateCache.PopulateMessage(identities[identityIndex]);
                    this.gameServer.SendMessageToUser(userInfo, networkIdentityUpdateCache);
                }
            }
        }

        private void ServerStarted()
        {
        }

        private void ServerShutdown()
        {
        }

        private void ServerUserInfoUpdated(UserInfo userInfo)
        {
        }

        private void ServerUserDisconnected(UserInfo userInfo, bool wasConnectionLost)
        {
        }
    }
}

#endif
