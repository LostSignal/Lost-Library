//-----------------------------------------------------------------------
// <copyright file="JoinServerResponseMessage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public class JoinServerResponseMessage : Message
    {
        public const short Id = 3;

        public bool Accepted { get; set; }

        public override short GetId()
        {
            return Id;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.Accepted = reader.ReadBoolean();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            writer.Write(this.Accepted);
        }
    }
}
