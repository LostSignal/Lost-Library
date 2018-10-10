//-----------------------------------------------------------------------
// <copyright file="UserDisconnectedMessage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public class UserDisconnectedMessage : Message
    {
        public const short Id = 4;

        public long UserId { get; set; }

        public bool WasConnectionLost { get; set; }

        public override short GetId()
        {
            return Id;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.UserId = reader.ReadInt64();
            this.WasConnectionLost = reader.ReadBoolean();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.Write(this.UserId);
            writer.Write(this.WasConnectionLost);
        }
    }
}
