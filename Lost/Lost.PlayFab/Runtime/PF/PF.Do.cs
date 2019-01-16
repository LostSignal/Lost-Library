//-----------------------------------------------------------------------
// <copyright file="PF.Do.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using PlayFab;
    using PlayFab.ClientModels;
    using System;
    using System.Collections.Generic;

    public static partial class PF
    {
        public static UnityTask<ExecuteCloudScriptResult> Do(ExecuteCloudScriptRequest request)
        {
            return Do<ExecuteCloudScriptRequest, ExecuteCloudScriptResult>(request, PlayFabClientAPI.ExecuteCloudScript);
        }

        public static UnityTask<EmptyResponse> Do(UpdateAvatarUrlRequest request)
        {
            return Do<UpdateAvatarUrlRequest, EmptyResponse>(request, PlayFabClientAPI.UpdateAvatarUrl);
        }

        public static UnityTask<UpdateUserTitleDisplayNameResult> Do(UpdateUserTitleDisplayNameRequest request)
        {
            return Do<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResult>(request, PlayFabClientAPI.UpdateUserTitleDisplayName);
        }

        public static UnityTask<GetAccountInfoResult> Do(GetAccountInfoRequest request)
        {
            return Do<GetAccountInfoRequest, GetAccountInfoResult>(request, PlayFabClientAPI.GetAccountInfo);
        }

        public static UnityTask<GetContentDownloadUrlResult> Do(GetContentDownloadUrlRequest request)
        {
            return Do<GetContentDownloadUrlRequest, GetContentDownloadUrlResult>(request, PlayFabClientAPI.GetContentDownloadUrl);
        }

        public static UnityTask<GetUserInventoryResult> Do(GetUserInventoryRequest request)
        {
            return Do<GetUserInventoryRequest, GetUserInventoryResult>(request, PlayFabClientAPI.GetUserInventory);
        }

        public static UnityTask<GetPlayerCombinedInfoResult> Do(GetPlayerCombinedInfoRequest request)
        {
            return Do<GetPlayerCombinedInfoRequest, GetPlayerCombinedInfoResult>(request, PlayFabClientAPI.GetPlayerCombinedInfo);
        }

        public static UnityTask<GetPlayerSegmentsResult> Do(GetPlayerSegmentsRequest request)
        {
            return Do<GetPlayerSegmentsRequest, GetPlayerSegmentsResult>(request, PlayFabClientAPI.GetPlayerSegments);
        }

        public static UnityTask<WriteEventResponse> Do(WriteClientPlayerEventRequest request)
        {
            return Do<WriteClientPlayerEventRequest, WriteEventResponse>(request, PlayFabClientAPI.WritePlayerEvent);
        }

        public static UnityTask<GetTimeResult> RefreshServerTime()
        {
            return Do<GetTimeRequest, GetTimeResult>(new GetTimeRequest(), PlayFabClientAPI.GetTime);
        }

        #region User Data Related Functions

        public static UnityTask<GetUserDataResult> Do(GetUserDataRequest request)
        {
            return Do<GetUserDataRequest, GetUserDataResult>(request, PlayFabClientAPI.GetUserData);
        }

        public static UnityTask<UpdateUserDataResult> Do(UpdateUserDataRequest request)
        {
            return Do<UpdateUserDataRequest, UpdateUserDataResult>(request, PlayFabClientAPI.UpdateUserData);
        }

        #endregion

        #region Player Statistics Related Functions

        public static UnityTask<GetPlayerStatisticsResult> Do(GetPlayerStatisticsRequest request)
        {
            return Do<GetPlayerStatisticsRequest, GetPlayerStatisticsResult>(request, PlayFabClientAPI.GetPlayerStatistics);
        }

        #endregion

        #region Title Data Related Functions

        public static UnityTask<GetTitleDataResult> Do(GetTitleDataRequest request)
        {
            return Do<GetTitleDataRequest, GetTitleDataResult>(request, PlayFabClientAPI.GetTitleData);
        }

        public static UnityTask<GetTitleNewsResult> Do(GetTitleNewsRequest request)
        {
            return Do<GetTitleNewsRequest, GetTitleNewsResult>(request, PlayFabClientAPI.GetTitleNews);
        }

        #endregion

        #region Push Notification Related Functions

        public static UnityTask<AndroidDevicePushNotificationRegistrationResult> Do(AndroidDevicePushNotificationRegistrationRequest request)
        {
            return Do<AndroidDevicePushNotificationRegistrationRequest, AndroidDevicePushNotificationRegistrationResult>(request, PlayFabClientAPI.AndroidDevicePushNotificationRegistration);
        }

        public static UnityTask<RegisterForIOSPushNotificationResult> Do(RegisterForIOSPushNotificationRequest request)
        {
            return Do<RegisterForIOSPushNotificationRequest, RegisterForIOSPushNotificationResult>(request, PlayFabClientAPI.RegisterForIOSPushNotification);
        }

        #endregion

        #region Leaderboard Related Functions

        public static UnityTask<GetLeaderboardResult> Do(GetLeaderboardRequest request)
        {
            return Do<GetLeaderboardRequest, GetLeaderboardResult>(request, PlayFabClientAPI.GetLeaderboard);
        }

        public static UnityTask<GetLeaderboardResult> Do(GetFriendLeaderboardRequest request)
        {
            return Do<GetFriendLeaderboardRequest, GetLeaderboardResult>(request, PlayFabClientAPI.GetFriendLeaderboard);
        }

        public static UnityTask<GetLeaderboardAroundPlayerResult> Do(GetLeaderboardAroundPlayerRequest request)
        {
            return Do<GetLeaderboardAroundPlayerRequest, GetLeaderboardAroundPlayerResult>(request, PlayFabClientAPI.GetLeaderboardAroundPlayer);
        }

        public static UnityTask<GetFriendLeaderboardAroundPlayerResult> Do(GetFriendLeaderboardAroundPlayerRequest request)
        {
            return Do<GetFriendLeaderboardAroundPlayerRequest, GetFriendLeaderboardAroundPlayerResult>(request, PlayFabClientAPI.GetFriendLeaderboardAroundPlayer);
        }

        #endregion

        #region Purchasing Related Functions

        public static UnityTask<ConfirmPurchaseResult> Do(ConfirmPurchaseRequest request)
        {
            return Do<ConfirmPurchaseRequest, ConfirmPurchaseResult>(request, PlayFabClientAPI.ConfirmPurchase);
        }

        public static UnityTask<ValidateIOSReceiptResult> Do(ValidateIOSReceiptRequest request)
        {
            return Do<ValidateIOSReceiptRequest, ValidateIOSReceiptResult>(request, PlayFabClientAPI.ValidateIOSReceipt);
        }

        public static UnityTask<ValidateGooglePlayPurchaseResult> Do(ValidateGooglePlayPurchaseRequest request)
        {
            return Do<ValidateGooglePlayPurchaseRequest, ValidateGooglePlayPurchaseResult>(request, PlayFabClientAPI.ValidateGooglePlayPurchase);
        }

        public static UnityTask<ValidateAmazonReceiptResult> Do(ValidateAmazonReceiptRequest request)
        {
            return Do<ValidateAmazonReceiptRequest, ValidateAmazonReceiptResult>(request, PlayFabClientAPI.ValidateAmazonIAPReceipt);
        }

        #endregion

        #region Friends Related Functions

        public static UnityTask<AddFriendResult> Do(AddFriendRequest request)
        {
            // should update the cached list after it's done
            return Do<AddFriendRequest, AddFriendResult>(request, PlayFabClientAPI.AddFriend);
        }

        public static UnityTask<RemoveFriendResult> Do(RemoveFriendRequest request)
        {
            // should update the cached list after it's done
            return Do<RemoveFriendRequest, RemoveFriendResult>(request, PlayFabClientAPI.RemoveFriend);
        }

        public static UnityTask<GetFriendsListResult> Do(GetFriendsListRequest request)
        {
            // TODO [bgish] - should really cache this, and update friends list whenever add/remove.  If cached, then return the list instead of call this
            return Do<GetFriendsListRequest, GetFriendsListResult>(request, PlayFabClientAPI.GetFriendsList);
        }

        #endregion

        #region Matchmaking Related Functions

        public static UnityTask<CurrentGamesResult> Do(CurrentGamesRequest request)
        {
            return Do<CurrentGamesRequest, CurrentGamesResult>(request, PlayFabClientAPI.GetCurrentGames);
        }

        public static UnityTask<MatchmakeResult> Do(MatchmakeRequest request)
        {
            return Do<MatchmakeRequest, MatchmakeResult>(request, PlayFabClientAPI.Matchmake);
        }

        public static UnityTask<StartGameResult> Do(StartGameRequest request)
        {
            return Do<StartGameRequest, StartGameResult>(request, PlayFabClientAPI.StartGame);
        }

        #endregion

        public static UnityTask<Result> Do<Request, Result>(Request request, Action<Request, Action<Result>, Action<PlayFabError>, object, Dictionary<string, string>> playfabFunction)
            where Request : class
            where Result : class
        {
            return UnityTask<Result>.Run(DoIterator(request, playfabFunction));
        }

        public static IEnumerator<Result> DoIterator<Request, Result>(Request request, Action<Request, Action<Result>, Action<PlayFabError>, object, Dictionary<string, string>> playfabFunction)
            where Request : class
            where Result : class
        {
            Result result = null;
            PlayFabError error = null;

            playfabFunction.Invoke(request, (r) => { result = r; }, (e) => { error = e; }, null, null);

            while (result == null && error == null)
            {
                yield return null;
            }

            if (error != null)
            {
                throw new PlayFabException(error);
            }

            // Checking cloud script logging for errors
            var executeCloudScriptResult = result as ExecuteCloudScriptResult;

            if (executeCloudScriptResult != null)
            {
                for (int i = 0; i < executeCloudScriptResult.Logs.Count; i++)
                {
                    if (executeCloudScriptResult.Logs[i].Level == "Error")
                    {
                        throw new PlayFabCloudScriptException(executeCloudScriptResult.Logs[i].Message);
                    }
                }
            }

            yield return result;
        }
    }
}

#endif
