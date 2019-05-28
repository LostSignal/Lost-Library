//-----------------------------------------------------------------------
// <copyright file="NetworkIdentitiesDestroyed.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using System.Collections.Generic;

    public class NetworkIdentitiesDestroyed : Message
    {
        public const short Id = 202;

        public List<long> DestroyedNetworkIds { get; } = new List<long>(50);

        public override short GetId()
        {
            return Id;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.DestroyedNetworkIds.Clear();

            int count = (int)reader.ReadPackedUInt32();

            for (int i = 0; i < count; i++)
            {
                this.DestroyedNetworkIds.Add(reader.ReadInt64());
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.WritePackedUInt32((uint)this.DestroyedNetworkIds.Count);

            for (int i = 0; i < this.DestroyedNetworkIds.Count; i++)
            {
                writer.Write(this.DestroyedNetworkIds[i]);
            }
        }
    }
}
