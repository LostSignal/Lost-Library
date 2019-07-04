//-----------------------------------------------------------------------
// <copyright file="NetworkIdentity.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_2018_3_OR_NEWER

namespace Lost.Networking
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class NetworkIdentity : MonoBehaviour
    {
        public static long NewId()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
        }

        private static readonly NetworkIdentityUpdate updateNetworkIdentityMessageCache = new NetworkIdentityUpdate();

        public delegate void NetworkIdentityDestroyedDelegate(long networkId);
        public NetworkIdentityDestroyedDelegate Destroyed;

        #pragma warning disable 0649
        [SerializeField] private long networkId = -1L;
        [SerializeField] private NetworkBehaviour[] behaviours = new NetworkBehaviour[0];
        #pragma warning restore 0649

        private long ownerId;

        public long NetworkId => this.networkId;
        public long OwnerId => this.ownerId;
        public string ResourceName { get; set; }

        public bool IsOwner
        {
            get
            {
                return
                    NetworkManager.IsInitialized &&
                    (NetworkManager.Instance.IsOffline ||
                    (this.ownerId == 0 && NetworkManager.Instance.IsServer) ||
                    (this.ownerId == NetworkManager.Instance.UserId));
            }
        }

        public NetworkBehaviour[] Behaviours => this.behaviours;

        public void SetNetworkId(long networkId)
        {
            this.networkId = networkId;
        }

        public void GenerateNewNetworkId()
        {
            this.networkId = NewId();
        }

        public void SetOwner(long ownerId)
        {
            this.ownerId = ownerId;

            if (NetworkManager.Instance.IsOffline || NetworkManager.Instance.IsServer)
            {
                this.SendUpdateNetworkIdentityMessage();
            }
        }

        public int GetBehaviourIndex(NetworkBehaviour behaviour)
        {
            if (this.behaviours != null)
            {
                for (int i = 0; i < this.behaviours.Length; i++)
                {
                    if (this.behaviours[i] == behaviour)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private void OnValidate()
        {
            if (Application.isPlaying || Platform.IsApplicationQuitting)
            {
                return;
            }

            if (this.networkId == -1L)
            {
                this.networkId = NewId();
            }
            else
            {
                List<NetworkIdentity> networkIdentities = new List<NetworkIdentity>();
                NetworkManager.GetAllNetworkIdentities(networkIdentities);

                foreach (var networkIdentity in networkIdentities)
                {
                    if (networkIdentity != this && networkIdentity.networkId == this.networkId)
                    {
                        this.networkId = NewId();
                        break;
                    }
                }
            }
        }

        private void OnEnable()
        {
            this.SendUpdateNetworkIdentityMessage();
        }

        private void OnDisable()
        {
            this.SendUpdateNetworkIdentityMessage();
        }

        private void OnDestroy()
        {
            this.Destroyed?.Invoke(this.networkId);
        }

        private void SendUpdateNetworkIdentityMessage()
        {
            if (NetworkManager.IsInitialized && this.IsOwner)
            {
                updateNetworkIdentityMessageCache.PopulateMessage(this);
                NetworkManager.Instance.SendMessage(updateNetworkIdentityMessageCache);
            }
        }
    }
}

#endif
