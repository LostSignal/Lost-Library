//-----------------------------------------------------------------------
// <copyright file="UserDisconnected.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine.Networking;

    public class UserDisconnected : RoomMessage
    {
        public const short MessageId = 102;

        public long DisconnectedUserId { get; set; }

        public override bool IsReliable
        {
            get { return true; }
        }

        public override bool RelayToAllClients
        {
            get { return true; }
        }

        public override short GetMessageId()
        {
            return MessageId;
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.Write(this.DisconnectedUserId);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.DisconnectedUserId = reader.ReadInt64();
        }
    }
}
