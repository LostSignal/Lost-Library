//-----------------------------------------------------------------------
// <copyright file="UpdateNetworkIdentity.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;

namespace Lost.Networking
{
    public class NetworkIdentityUpdate : Message
    {
        public const short Id = 203;

        public long NetworkId { get; set; }
        public long OwnerId { get; set; }
        public bool IsEnabled { get; set; }
        public string ResourceName { get; set; }
        public Vector3 Position { get; set; }

        public override short GetId()
        {
            return Id;
        }

        public void PopulateMessage(NetworkIdentity identity)
        {
            this.NetworkId = identity.NetworkId;
            this.OwnerId = identity.OwnerId;
            this.IsEnabled = identity.gameObject.activeSelf;
            this.ResourceName = identity.ResourceName;
            this.Position = identity.transform.position;
        }

        public void PopulateNetworkIdentity(NetworkIdentity identity)
        {
            if (identity.OwnerId != this.OwnerId)
            {
                identity.SetOwner(this.OwnerId);
            }

            identity.gameObject.SafeSetActive(this.IsEnabled);
            identity.ResourceName = identity.ResourceName;
            identity.transform.position = this.Position;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.NetworkId = (long)reader.ReadPackedUInt64();
            this.OwnerId = (long)reader.ReadPackedUInt64();
            this.IsEnabled = reader.ReadBoolean();
            this.ResourceName = reader.ReadString();
            this.Position = reader.ReadVector3();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.WritePackedUInt64((ulong)this.NetworkId);
            writer.WritePackedUInt64((ulong)this.OwnerId);
            writer.Write(this.IsEnabled);
            writer.Write(this.ResourceName);
            writer.Write(this.Position);
        }
    }
}
