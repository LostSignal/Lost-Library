//-----------------------------------------------------------------------
// <copyright file="ClientIdentity.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class ClientIdentity : RoomMessage
    {
        public const short MessageId = 104;

        public override bool IsReliable
        {
            get { return true; }
        }

        public override bool RelayToAllClients
        {
            get { return false; }
        }

        public override short GetMessageId()
        {
            return MessageId;
        }
    }
}
