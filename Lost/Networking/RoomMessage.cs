//-----------------------------------------------------------------------
// <copyright file="RoomMessage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine.Networking;

    public abstract class RoomMessage : MessageBase
    {
        public long UserId { get; set; }

        public abstract bool IsReliable { get; }

        public abstract bool RelayToAllClients { get; }

        public abstract short GetMessageId();

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.Write(this.UserId);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.UserId = reader.ReadInt64();
        }
    }
}
