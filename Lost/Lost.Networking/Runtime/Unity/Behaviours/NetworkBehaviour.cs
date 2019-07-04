//-----------------------------------------------------------------------
// <copyright file="NetworkBehaviour.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_2018_3_OR_NEWER

namespace Lost.Networking
{
    using UnityEngine;

    [RequireComponent(typeof(NetworkIdentity))]
    public abstract class NetworkBehaviour : MonoBehaviour
    {
        private static NetworkBehaviourMessage behaviourMessage = new NetworkBehaviourMessage();
        private static NetworkWriter writer = new NetworkWriter();

        #pragma warning disable 0649
        [HideInInspector, SerializeField] private NetworkIdentity networkIdentity;
        [SerializeField] private float updateFrequency = 0.1f;
        #pragma warning restore 0649

        private float lastSent;

        public NetworkIdentity Identity => this.networkIdentity;

        public long NetworkId => this.networkIdentity.NetworkId;

        public bool IsOwner => this.networkIdentity.IsOwner;

        public int BehaviourIndex => this.networkIdentity.GetBehaviourIndex(this);

        public abstract void Serialize(NetworkWriter writer);

        public abstract void Deserialize(NetworkReader reader);

        protected virtual void Update()
        {
            if (this.IsOwner && (Time.realtimeSinceStartup - this.lastSent) > this.updateFrequency)
            {
                behaviourMessage.NetworkId = this.NetworkId;
                behaviourMessage.BehaviourIndex = this.BehaviourIndex;

                writer.SeekZero();
                this.Serialize(writer);

                behaviourMessage.DataLength = writer.Position;
                behaviourMessage.Data = writer.RawBuffer;

                NetworkManager.Instance.SendMessage(behaviourMessage);

                this.lastSent = Time.realtimeSinceStartup;
            }
        }

        protected virtual void Reset()
        {
            this.AssertGetComponent(ref this.networkIdentity);
        }

        protected virtual void Awake()
        {
            this.AssertGetComponent(ref this.networkIdentity);
            Debug.Assert(this.BehaviourIndex != -1);
        }
    }
}

#endif
