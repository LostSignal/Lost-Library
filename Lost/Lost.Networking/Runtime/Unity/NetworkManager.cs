//-----------------------------------------------------------------------
// <copyright file="Network.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class NetworkManager : SingletonGameObject<NetworkManager>
    {
        public delegate void OnNetworkUpdateDelegate(float deltaTime);
        public event OnNetworkUpdateDelegate OnNetworkUpdate;

        private static readonly List<NetworkIdentity> networkIdentitiesCache = new List<NetworkIdentity>(0);

        public long UserId { get; private set; } = -1L;
        public bool IsServer => this.gameServer != null;
        public bool IsClientAndServer => this.gameServer != null && this.gameClient != null;
        public bool ShouldRunServerLogic => this.IsServer || this.IsOffline;

        // TODO [bgish]: Once SendBehaviourMessage works, this should go away
        public bool IsOffline => this.gameServer == null && this.gameClient == null;

        // TODO [bgish]: This should really be synced between all clients and the server
        public double Time => UnityEngine.Time.realtimeSinceStartup;

        public ReadOnlyCollection<UserInfo> ConnectedUsers => this.gameServer?.ConnectedUsers ?? this.gameClient?.ConnectedUsers;

        private UnityGameServer gameServer;
        private UnityGameClient gameClient;

        public void Shutdown()
        {
            this.gameClient?.Shutdown();
            this.gameClient = null;

            this.gameServer?.Shutdown();
            this.gameServer = null;
        }

        public void Initialize(GameServer server)
        {
            this.gameServer = new UnityGameServer(server);
        }

        public void Initialize(GameClient client)
        {
            this.UserId = client.UserId;
            this.gameClient = new UnityGameClient(client);
            Debug.Log("UserId = " + gameClient.UserId);
        }

        public NetworkIdentity CreateNetworkIdentity(string resourceName, long ownerId = 0L, Vector3 position = new Vector3())
        {
            return this.gameServer?.CreateNetworkIdentity(resourceName, ownerId, position);
        }

        public void SendMessage(Message message)
        {
            this.gameServer?.SendNetworkMessage(message);
            this.gameClient?.SendNetworkMessage(message);
        }

        public static void GetAllNetworkIdentities(List<NetworkIdentity> networkIdentities)
        {
            networkIdentities.Clear();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).isLoaded)
                {
                    foreach (var rootObject in SceneManager.GetSceneAt(i).GetRootGameObjects())
                    {
                        networkIdentitiesCache.Clear();
                        rootObject.GetComponentsInChildren<NetworkIdentity>(true, networkIdentitiesCache);
                        networkIdentities.AddRange(networkIdentitiesCache);
                    }
                }
            }
        }

        private void Update()
        {
            this.gameClient?.Update();
            this.gameServer?.Update();
            this.OnNetworkUpdate?.Invoke(UnityEngine.Time.deltaTime);
        }

        private void OnApplicationQuit()
        {
            this.Shutdown();
        }
    }
}
