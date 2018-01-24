//-----------------------------------------------------------------------
// <copyright file="UserDisconnected.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class UserDisconnected : RoomMessage
    {
        public const short MessageId = 102;
        
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
    }
}
