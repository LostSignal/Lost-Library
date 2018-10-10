//-----------------------------------------------------------------------
// <copyright file="JoinServerRequestMessage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public class JoinServerRequestMessage : Message
    {
        public const short Id = 1;

        public UserInfo UserInfo { get; set; }

        public string CustomData { get; set; }

        public JoinServerRequestMessage()
        {
            this.UserInfo = new UserInfo();
        }

        public override short GetId()
        {
            return Id;
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);

            this.UserInfo.Deserialize(reader);
            this.CustomData = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            this.UserInfo.Serialize(writer);
            writer.Write(this.CustomData);
        }
    }
}
