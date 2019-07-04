//-----------------------------------------------------------------------
// <copyright file="NetworkIdentityCreated.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_2018_3_OR_NEWER

using UnityEngine;

namespace Lost.Networking
{
    public class NetworkIdentityCreated : Message
    {
        public const short Id = 205;

        public long NetworkId { get; set; }
        public long OwnerId { get; set; }
        public string ResourceName { get; set; }
        public Vector3 Position { get; set; }

        public override short GetId()
        {
            return Id;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.NetworkId = reader.ReadInt64();
            this.OwnerId = reader.ReadInt64();
            this.ResourceName = reader.ReadString();
            this.Position = reader.ReadVector3();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.Write(this.NetworkId);
            writer.Write(this.OwnerId);
            writer.Write(this.ResourceName);
            writer.Write(this.Position);
        }
    }
}

#endif
