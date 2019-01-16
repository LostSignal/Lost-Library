//-----------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using System.Collections.Generic;

    public class UserInfo
    {
        /// <summary>
        /// Only used/set by the server.
        /// </summary>
        public long ConnectionId { get; set; }

        public long UserId { get; set; }

        public Dictionary<string, string> CustomData { get; set; }

        public UserInfo()
        {
            this.CustomData = new Dictionary<string, string>();
        }

        public void Deserialize(NetworkReader reader)
        {
            this.ConnectionId = reader.ReadInt64();
            this.UserId = reader.ReadInt64();

            // CustomData
            this.CustomData.Clear();

            byte count = reader.ReadByte();

            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();

                this.CustomData.Add(key, value);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.ConnectionId);
            writer.Write(this.UserId);

            // CustomData
            writer.Write((byte)this.CustomData.Count);

            foreach (var pair in this.CustomData)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
        }

        public void CopyFrom(UserInfo source)
        {
            this.ConnectionId = source.ConnectionId;
            this.UserId = source.UserId;

            // CustomData
            this.CustomData.Clear();

            foreach (var pair in source.CustomData)
            {
                this.CustomData.Add(pair.Key, pair.Value);
            }
        }
    }
}
