//-----------------------------------------------------------------------
// <copyright file="UserInfoMessage.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public class UserInfoMessage : Message
    {
        public const short Id = 5;

        public UserInfo UserInfo { get; set; }

        public UserInfoMessage()
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
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);

            this.UserInfo.Serialize(writer);
        }
    }
}
