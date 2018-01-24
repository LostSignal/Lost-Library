//-----------------------------------------------------------------------
// <copyright file="RoomInfo.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public class RoomInfo
    {
        // TODO [bgish]: Add RoomSearchString?
        public string RoomName { get; set; }
        public string RoomPassword { get; set; }
        public bool Advertise { get; set; }
        public uint MaxConnections { get; set; }
        public int Elo { get; set; }

        public RoomInfo()
        {
            this.RoomName = string.Empty;
            this.RoomPassword = string.Empty;
            this.Advertise = true;
            this.MaxConnections = 2;
            this.Elo = 0;
        }
    }
}
