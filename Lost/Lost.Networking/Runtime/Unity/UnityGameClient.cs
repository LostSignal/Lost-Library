//-----------------------------------------------------------------------
// <copyright file="UnityGameClient.cs" company="Lost Signal LLC">
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

    public class UnityGameClient
    {
        private static readonly NetworkReader reader = new NetworkReader(new byte[0]);

        // private static readonly NetworkIdentityUpdate updateNetworkIdentityCache = new NetworkIdentityUpdate();
        private static Dictionary<string, NetworkIdentity> resourcePrefabCache = new Dictionary<string, NetworkIdentity>();

        // Dynamic Network Object Tracking
        private Dictionary<long, NetworkIdentity> dynamicNetworkObjectsHash = new Dictionary<long, NetworkIdentity>();
        private List<NetworkIdentity> dynamicNetworkObjectsList = new List<NetworkIdentity>();

        // Static Network Object Tracking
        private Dictionary<long, NetworkIdentity> staticNetworkObjectsHash = new Dictionary<long, NetworkIdentity>();
        private List<NetworkIdentity> staticNetworkObjectsList = new List<NetworkIdentity>();


        public UnityGameClient(GameClient gameClient)
        {
            this.StartGameClient(gameClient);
        }

        public ReadOnlyCollection<UserInfo> ConnectedUsers => this.gameClient.ConnectedUsers;

        public long UserId
        {
            get { return this.gameClient != null ? this.gameClient.UserId : -1L; }
        }

        private GameClient gameClient;

        public void Update()
        {
            this.gameClient?.Update();
        }

        public void Shutdown()
        {
            this.gameClient?.Shutdown();

            if (Platform.IsApplicationQuitting == false)
            {
                // Destroy all dynamic objects
                foreach (var dynamic in this.dynamicNetworkObjectsList)
                {
                    if (dynamic?.gameObject)
                    {
                        Pooler.Destroy(dynamic?.gameObject);
                    }
                }

                this.dynamicNetworkObjectsList.Clear();
                this.dynamicNetworkObjectsHash.Clear();
            }
        }

        public void SendNetworkMessage(Message message)
        {
            this.gameClient?.SendMessage(message);
        }

        private void StartGameClient(GameClient gameClient)
        {
            this.gameClient = gameClient;

            // Registering Custom Messages
            this.gameClient.RegisterMessage<NetworkIdentityUpdate>();
            this.gameClient.RegisterMessage<NetworkIdentitiesDestroyed>();
            this.gameClient.RegisterMessage<NetworkBehaviourMessage>();
            this.gameClient.RegisterMessage<NetworkIdentityCreated>();

            // Receiving Messages
            this.gameClient.ClientReceivedMessage += this.ClientReceivedMessage;

            // Server Connection Events
            this.gameClient.ClientConnectedToServer += this.ClientConnectedToServer;
            this.gameClient.ClientFailedToConnectToServer += this.ClientFailedToConnectToServer;
            this.gameClient.ClientDisconnectedFromServer += this.ClientDisconnectedFromServer;
            this.gameClient.ClientLostConnectionToServer += this.ClientLostConnectionToServer;

            // User Events
            this.gameClient.ClientUserConnected += this.ClientUserConnected;
            this.gameClient.ClientUserInfoUpdated += this.ClientUserInfoUpdated;
            this.gameClient.ClientUserDisconnected += this.ClientUserDisconnected;

            // Collecting all the static network objects
            NetworkManager.GetAllNetworkIdentities(this.staticNetworkObjectsList);

            // Hashing all the static network objects for quicker lookup
            foreach (var identity in this.staticNetworkObjectsList)
            {
                this.staticNetworkObjectsHash.Add(identity.NetworkId, identity);
            }
        }

        private void ClientReceivedMessage(Message message)
        {
            switch (message.GetId())
            {
                case NetworkIdentityCreated.Id:
                    {
                        if (NetworkManager.Instance.IsClientAndServer)
                        {
                            return;
                        }

                        var networkIdentityCreated = (NetworkIdentityCreated)message;

                        this.CreateDynamicNetworkIdentity(
                            networkIdentityCreated.ResourceName,
                            networkIdentityCreated.NetworkId,
                            networkIdentityCreated.OwnerId,
                            networkIdentityCreated.Position);

                        break;
                    }

                case NetworkIdentityUpdate.Id:
                    {
                        if (NetworkManager.Instance.IsClientAndServer)
                        {
                            return;
                        }

                        var networkIdentityUpdatedMessage = (NetworkIdentityUpdate)message;

                        if (networkIdentityUpdatedMessage.OwnerId != this.gameClient.UserId)
                        {
                            if (this.staticNetworkObjectsHash.TryGetValue(networkIdentityUpdatedMessage.NetworkId, out NetworkIdentity staticIdentity))
                            {
                                networkIdentityUpdatedMessage.PopulateNetworkIdentity(staticIdentity);
                            }
                            else if (this.dynamicNetworkObjectsHash.TryGetValue(networkIdentityUpdatedMessage.NetworkId, out NetworkIdentity dynamicIdentity))
                            {
                                networkIdentityUpdatedMessage.PopulateNetworkIdentity(dynamicIdentity);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(networkIdentityUpdatedMessage.ResourceName) == false)
                                {
                                    var newIdentity = this.CreateDynamicNetworkIdentity(
                                        networkIdentityUpdatedMessage.ResourceName,
                                        networkIdentityUpdatedMessage.NetworkId,
                                        networkIdentityUpdatedMessage.OwnerId,
                                        networkIdentityUpdatedMessage.Position);

                                    networkIdentityUpdatedMessage.PopulateNetworkIdentity(newIdentity);
                                }
                            }
                        }

                        break;
                    }

                case NetworkBehaviourMessage.Id:
                    {
                        if (NetworkManager.Instance.IsClientAndServer)
                        {
                            return;
                        }

                        var networkBehaviourMessage = (NetworkBehaviourMessage)message;

                        this.staticNetworkObjectsHash.TryGetValue(networkBehaviourMessage.NetworkId, out NetworkIdentity staticIdentity);
                        this.dynamicNetworkObjectsHash.TryGetValue(networkBehaviourMessage.NetworkId, out NetworkIdentity dynamicIdentity);

                        var identity = staticIdentity ?? dynamicIdentity;

                        if (identity != null && identity.IsOwner == false)
                        {
                            reader.Replace(networkBehaviourMessage.Data);
                            identity.Behaviours[networkBehaviourMessage.BehaviourIndex].Deserialize(reader);
                        }

                        break;
                    }

                case NetworkIdentitiesDestroyed.Id:
                    {
                        var networkIdentitiesDestoryed = (NetworkIdentitiesDestroyed)message;

                        foreach (long networkId in networkIdentitiesDestoryed.DestroyedNetworkIds)
                        {
                            this.staticNetworkObjectsHash.TryGetValue(networkId, out NetworkIdentity staticIdentity);
                            this.dynamicNetworkObjectsHash.TryGetValue(networkId, out NetworkIdentity dynamicIdentity);

                            var identity = staticIdentity ?? dynamicIdentity;

                            if (identity != null)
                            {
                                Pooler.Destroy(identity.gameObject);
                            }
                        }

                        break;
                    }
            }
        }

        private NetworkIdentity CreateDynamicNetworkIdentity(string resourceName, long networkId, long ownerId, Vector3 position)
        {
            NetworkIdentity networkIdentityPrefab = null;

            if (resourcePrefabCache.TryGetValue(resourceName, out networkIdentityPrefab) == false)
            {
                networkIdentityPrefab = Resources.Load<NetworkIdentity>(resourceName);
                resourcePrefabCache.Add(resourceName, networkIdentityPrefab);
            }

            var newNetworkIdentity = Pooler.Instantiate(networkIdentityPrefab);
            newNetworkIdentity.SetNetworkId(networkId);
            newNetworkIdentity.SetOwner(ownerId);
            newNetworkIdentity.ResourceName = resourceName;
            newNetworkIdentity.transform.position = position;

            // Registering the new dynamic object
            this.dynamicNetworkObjectsHash.Add(newNetworkIdentity.NetworkId, newNetworkIdentity);
            this.dynamicNetworkObjectsList.Add(newNetworkIdentity);

            return newNetworkIdentity;
        }

        private void ClientConnectedToServer()
        {
        }

        private void ClientFailedToConnectToServer()
        {
        }

        private void ClientDisconnectedFromServer()
        {
        }

        private void ClientLostConnectionToServer()
        {
        }

        private void ClientUserConnected(UserInfo userInfo, bool wasReconnect)
        {
        }

        private void ClientUserInfoUpdated(UserInfo userInfo)
        {
        }

        private void ClientUserDisconnected(UserInfo userInfo, bool wasConnectionLost)
        {
        }
    }
}

#endif
