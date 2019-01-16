//-----------------------------------------------------------------------
// <copyright file="PlayFabGameClient.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost.Networking
{
    using PlayFab.ClientModels;

    public abstract class PlayFabGameClient : GameClient
    {
        private static UserInfo SetUserId(UserInfo userInfo, string playfabId)
        {
            userInfo.UserId = System.Convert.ToInt64(playfabId, 16);
            return userInfo;
        }

        public PlayFabGameClient(IClientTransportLayer transportLayer, UserInfo myUserInfo, string playfabId, string matchmakingTicket)
            : base(transportLayer, SetUserId(myUserInfo, playfabId), matchmakingTicket)
        {
        }
    }
}

#endif
