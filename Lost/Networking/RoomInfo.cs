//-----------------------------------------------------------------------
// <copyright file="RoomInfo.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class RoomInfo
    {
        public string RoomSearchName { get; set; }
        public string RoomSearchPassword { get; set; }
        public string NewRoomName { get; set; }
        public string NewRoomPassword { get; set; }
        public bool Advertise { get; set; }
        public uint MaxConnections { get; set; }
        public int Elo { get; set; }

        public RoomInfo()
        {
            this.RoomSearchName = string.Empty;
            this.RoomSearchPassword = string.Empty;
            this.NewRoomName = string.Empty;
            this.NewRoomPassword = string.Empty;
            this.Advertise = true;
            this.MaxConnections = 2;
            this.Elo = 0;
        }
    }
}
