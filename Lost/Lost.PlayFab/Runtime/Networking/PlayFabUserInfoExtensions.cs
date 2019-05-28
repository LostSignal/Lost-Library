//-----------------------------------------------------------------------
// <copyright file="PlayFabUserInfoExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.Networking;

    public static class PlayFabUserInfoExtensions
    {
        public static string GetPlayFabId(this UserInfo userInfo)
        {
            return userInfo.CustomData["PlayFabId"];
        }

        public static void SetPlayFabId(this UserInfo userInfo, string playFabId)
        {
            userInfo.CustomData["PlayFabId"] = playFabId;
        }

        public static string GetCharacterId(this UserInfo userInfo)
        {
            return userInfo.CustomData["CharacterId"];
        }

        public static void SetCharacterId(this UserInfo userInfo, string characterId)
        {
            userInfo.CustomData["CharacterId"] = characterId;
        }

        public static string GetUsername(this UserInfo userInfo)
        {
            return userInfo.CustomData["Username"];
        }

        public static void SetUsername(this UserInfo userInfo, string username)
        {
            userInfo.CustomData["Username"] = username;
        }

        public static string GetDisplayName(this UserInfo userInfo)
        {
            return userInfo.CustomData["DisplayName"];
        }

        public static void SetDisplayName(this UserInfo userInfo, string displayName)
        {
            userInfo.CustomData["DisplayName"] = displayName;
        }
    }
}
