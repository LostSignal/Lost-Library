//-----------------------------------------------------------------------
// <copyright file="Message.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    public abstract class Message
    {
        public abstract short GetId();

        public virtual void Serialize(NetworkWriter writer)
        {
            writer.Write(this.GetId());
        }

        public virtual void Deserialize(NetworkReader reader)
        {
            reader.ReadInt16();
        }
    }
}
