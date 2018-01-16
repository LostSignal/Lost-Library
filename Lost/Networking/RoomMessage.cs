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
        public abstract bool IsReliable { get; }

        public abstract bool RelayToAllClients { get; }

        public abstract short GetMessageId();
    }
}
