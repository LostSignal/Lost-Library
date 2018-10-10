//-----------------------------------------------------------------------
// <copyright file="PlayFabServerSettingsData.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY || ENABLE_PLAYFABSERVER_API

namespace Lost.Networking
{
    using System.Collections.Generic;
    using PlayFab.ServerModels;

    public class PlayFabServerSettingsData
    {
        // external matchmaking data
        public bool IsExternalServer { get; set; }
        public List<string> Tags { get; set; }

        // playfab data
        public string TitleId { get; set; }
        public string TitleSecretKey { get; set; }
        public string PlayFabApiEndpoint { get; set; }
        public string LobbyId { get; set; }

        // server data
        public string ServerHostDomain { get; set; }
        public int ServerHostPort { get; set; }
        public Region ServerHostRegion { get; set; }

        // game mode data
        public string GameMode { get; set; }
        public string GameBuildVersion { get; set; }
        public string CustomData { get; set; }

        // logging / output data
        public string LogFilePath { get; set; }
        public string OutputFilesDirectory { get; set; }

        public override string ToString()
        {
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("IsExternalServer=");
            stringBuilder.Append(this.IsExternalServer);

            stringBuilder.Append(" Tags=");
            stringBuilder.Append(this.Tags != null ? string.Join(",", this.Tags) : string.Empty);

            stringBuilder.Append(" TitleId=");
            stringBuilder.Append(this.TitleId);

            stringBuilder.Append(" TitleSecretKey=");
            stringBuilder.Append(this.TitleSecretKey);

            stringBuilder.Append(" PlayFabApiEndpoint=");
            stringBuilder.Append(this.PlayFabApiEndpoint);

            stringBuilder.Append(" LobbyId=");
            stringBuilder.Append(this.LobbyId);

            stringBuilder.Append(" ServerHostDomain=");
            stringBuilder.Append(this.ServerHostDomain);

            stringBuilder.Append(" ServerHostPort=");
            stringBuilder.Append(this.ServerHostPort);

            stringBuilder.Append(" ServerHostRegion=");
            stringBuilder.Append(this.ServerHostRegion);

            stringBuilder.Append(" GameMode=");
            stringBuilder.Append(this.GameMode);

            stringBuilder.Append(" GameBuildVersion=");
            stringBuilder.Append(this.GameBuildVersion);

            stringBuilder.Append(" CustomData=");
            stringBuilder.Append(this.CustomData);

            stringBuilder.Append(" LogFilePath=");
            stringBuilder.Append(this.LogFilePath);

            stringBuilder.Append(" OutputFilesDirectory=");
            stringBuilder.Append(this.OutputFilesDirectory);

            return stringBuilder.ToString();
        }
    }
}

#endif
