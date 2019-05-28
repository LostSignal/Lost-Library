//-----------------------------------------------------------------------
// <copyright file="NetworkBehaviourMessage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public class NetworkBehaviourMessage : Message
    {
        private static readonly NetworkReader reader = new NetworkReader(new byte[0]);

        public const short Id = 201;

        public long NetworkId { get; set; }
        public int BehaviourIndex { get; set; }
        public int DataLength { get; set; }
        public byte[] Data { get; set; } = new byte[1024];

        public override short GetId()
        {
            return Id;
        }

        // public void PopulateMessage(NetworkBehaviour behaviour, NetworkWriter writer)
        // {
        //     this.NetworkId = behaviour.NetworkId;
        //     this.BehaviourIndex = behaviour.BehaviourIndex;
        //     this.DataLength = writer.Position;
        // 
        //     reader.Replace(writer.RawBuffer);
        //     reader.ReadBytes(this.Data, this.DataLength);
        // }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.NetworkId = (long)reader.ReadPackedUInt64();
            this.BehaviourIndex = (int)reader.ReadPackedUInt32();
            this.DataLength = (int)reader.ReadPackedUInt32();

            for (int i = 0; i < this.DataLength; i++)
            {
                this.Data[i] = reader.ReadByte();
            }
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.WritePackedUInt64((ulong)this.NetworkId);
            writer.WritePackedUInt32((uint)this.BehaviourIndex);
            writer.WritePackedUInt32((uint)this.DataLength);

            for (int i = 0; i < this.DataLength; i++)
            {
                writer.Write(this.Data[i]);
            }
        }
    }
}
