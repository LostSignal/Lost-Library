
namespace Lost
{
    using System;
    using System.Collections.Generic;
    using PlayFab;
    using PlayFab.ClientModels;
    using PlayFab.Events;

    public static class PF
    {
        //private static PlayFabEvents playfabEvents;
        private static UserAccountInfo userAccountInfo = null;

        static PF()
        {
            // TODO register for Logged In event and get the userAccountInfo once you have a successful login
            // PlayFab.Events.PlayFabEvents.Init();
            // PlayFab.Events.PlayFabEvents.PlayFabResultEvent<LoginResult> += ;

            //playfabEvents = PlayFabEvents.Init();
            //playfabEvents.OnLoginResultEvent += PlayfabEvents_OnLoginResultEvent;
        }

        //private static void PlayfabEvents_OnLoginResultEvent(LoginResult result)
        //{
        //    throw new NotImplementedException();
        //}

        public static string DeviceId
        {
            // TODO [bgish]: instead of using this, should make a new guid and store the file locally and use that
            get { return UnityEngine.SystemInfo.deviceUniqueIdentifier;  }
        }

        public static bool IsLoggedIn
        {
            get { return PlayFabClientAPI.IsClientLoggedIn(); }
        }

        public static string Id
        {
            get { return userAccountInfo != null ? userAccountInfo.PlayFabId : null; }
        }

        public static string FacebookId
        {
            get { return userAccountInfo != null && userAccountInfo.FacebookInfo != null ? userAccountInfo.FacebookInfo.FacebookId : null; }
        }

        public static bool IsFacebookLinked
        {
            get { return string.IsNullOrEmpty(FacebookId); }
        }

        public static bool IsDeviceLinked
        {
            get
            {
                #if UNITY_EDITOR
                return userAccountInfo != null && userAccountInfo.CustomIdInfo != null && userAccountInfo.CustomIdInfo.CustomId == DeviceId;
                #elif UNITY_IOS
                return userAccountInfo != null && userAccountInfo.IosDeviceInfo != null && userAccountInfo.IosDeviceInfo.IosDeviceId == DeviceId;
                #elif UNITY_ANDROID
                return userAccountInfo != null && userAccountInfo.AndroidDeviceInfo != null && userAccountInfo.AndroidDeviceInfo.AndroidDeviceId == DeviceId;
                #endif
            }
        }

        public static IEnumerator<LoginResult> LoginWithDeviceId(bool createAccount)
        {
            #if UNITY_EDITOR
            
            return Do<LoginWithCustomIDRequest, LoginResult>(new LoginWithCustomIDRequest { CreateAccount = createAccount, CustomId = DeviceId }, PlayFabClientAPI.LoginWithCustomID);
            
            #elif UNITY_ANDROID

            return Do<LoginWithAndroidDeviceIDRequest, LoginResult>(
                new LoginWithAndroidDeviceIDRequest
                {
                    CreateAccount = createAccount,
                    AndroidDeviceId = DeviceId,
                    AndroidDevice = UnityEngine.SystemInfo.deviceModel,
                    OS = UnityEngine.SystemInfo.operatingSystem,
                }, 
                PlayFabClientAPI.LoginWithAndroidDeviceID);
            
            #elif UNITY_IOS
            
            return Do<LoginWithIOSDeviceIDRequest, LoginResult>(
                new LoginWithIOSDeviceIDRequest
                {
                    CreateAccount = createAccount,
                    DeviceId = DeviceId,
                    DeviceModel = UnityEngine.SystemInfo.deviceModel,
                    OS = UnityEngine.SystemInfo.operatingSystem,
                }, 
                PlayFabClientAPI.LoginWithIOSDeviceID);
            
            #endif
        }

        // LinkDeviceId
        // UnlinkDeviceId

        // LoginWithFacebook
        // LinkFacebook
        // UnlinkFacebook
        // GetFacebookFriends: Get PlayFab Friends list (cache it), then get FB Friends (from FB SDK), then GetPlayFabIDsFromFacebookIDs and only return ones you haven't added yet

        public static IEnumerator<UpdateUserTitleDisplayNameResult> Do(UpdateUserTitleDisplayNameRequest request)
        {
            return Do<UpdateUserTitleDisplayNameRequest, UpdateUserTitleDisplayNameResult>(request, PlayFabClientAPI.UpdateUserTitleDisplayName);
        }

        public static IEnumerator<ExecuteCloudScriptResult> Do(ExecuteCloudScriptRequest request)
        {
            return Do<ExecuteCloudScriptRequest, ExecuteCloudScriptResult>(request, PlayFabClientAPI.ExecuteCloudScript);
        }

        public static IEnumerator<GetAccountInfoResult> Do(GetAccountInfoRequest request)
        {
            return Do<GetAccountInfoRequest, GetAccountInfoResult>(request, PlayFabClientAPI.GetAccountInfo);
        }

        public static IEnumerator<GetContentDownloadUrlResult> Do(GetContentDownloadUrlRequest request)
        {
            return Do<GetContentDownloadUrlRequest, GetContentDownloadUrlResult>(request, PlayFabClientAPI.GetContentDownloadUrl);
        }
        
        public static IEnumerator<GetUserInventoryResult> Do(GetUserInventoryRequest request)
        {
            return Do<GetUserInventoryRequest, GetUserInventoryResult>(request, PlayFabClientAPI.GetUserInventory);
        }
        
        public static IEnumerator<GetPlayerCombinedInfoResult> Do(GetPlayerCombinedInfoRequest request)
        {
            return Do<GetPlayerCombinedInfoRequest, GetPlayerCombinedInfoResult>(request, PlayFabClientAPI.GetPlayerCombinedInfo);
        }

        public static IEnumerator<GetPlayerSegmentsResult> Do(GetPlayerSegmentsRequest request)
        {
            return Do<GetPlayerSegmentsRequest, GetPlayerSegmentsResult>(request, PlayFabClientAPI.GetPlayerSegments);
        }

        public static IEnumerator<LogEventResult> Do(LogEventRequest request)
        {
            return Do<LogEventRequest, LogEventResult>(request, PlayFabClientAPI.LogEvent);
        }

        #region Title Data Related Functions

        public static IEnumerator<GetTitleDataResult> Do(GetTitleDataRequest request)
        {
            return Do<GetTitleDataRequest, GetTitleDataResult>(request, PlayFabClientAPI.GetTitleData);
        }

        public static IEnumerator<GetTitleNewsResult> Do(GetTitleNewsRequest request)
        {
            return Do<GetTitleNewsRequest, GetTitleNewsResult>(request, PlayFabClientAPI.GetTitleNews);
        }
        
        #endregion

        #region Push Notification Related Functions

        public static IEnumerator<AndroidDevicePushNotificationRegistrationResult> Do(AndroidDevicePushNotificationRegistrationRequest request)
        {
            return Do<AndroidDevicePushNotificationRegistrationRequest, AndroidDevicePushNotificationRegistrationResult>(request, PlayFabClientAPI.AndroidDevicePushNotificationRegistration);
        }

        public static IEnumerator<RegisterForIOSPushNotificationResult> Do(RegisterForIOSPushNotificationRequest request)
        {
            return Do<RegisterForIOSPushNotificationRequest, RegisterForIOSPushNotificationResult>(request, PlayFabClientAPI.RegisterForIOSPushNotification);
        }

        #endregion

        #region Leaderboard Related Functions

        public static IEnumerator<GetLeaderboardResult> Do(GetLeaderboardRequest request)
        {
            return Do<GetLeaderboardRequest, GetLeaderboardResult>(request, PlayFabClientAPI.GetLeaderboard);
        }

        public static IEnumerator<GetLeaderboardResult> Do(GetFriendLeaderboardRequest request)
        {
            return Do<GetFriendLeaderboardRequest, GetLeaderboardResult>(request, PlayFabClientAPI.GetFriendLeaderboard);
        }

        public static IEnumerator<GetLeaderboardAroundCurrentUserResult> Do(GetLeaderboardAroundCurrentUserRequest request)
        {
            return Do<GetLeaderboardAroundCurrentUserRequest, GetLeaderboardAroundCurrentUserResult>(request, PlayFabClientAPI.GetLeaderboardAroundCurrentUser);
        }
        
        public static IEnumerator<GetFriendLeaderboardAroundCurrentUserResult> Do(GetFriendLeaderboardAroundCurrentUserRequest request)
        {
            return Do<GetFriendLeaderboardAroundCurrentUserRequest, GetFriendLeaderboardAroundCurrentUserResult>(request, PlayFabClientAPI.GetFriendLeaderboardAroundCurrentUser);
        }

        #endregion

        #region Purchasing Related Functions

        public static IEnumerator<PurchaseItemResult> Do(PurchaseItemRequest request)
        {
            return Do<PurchaseItemRequest, PurchaseItemResult>(request, PlayFabClientAPI.PurchaseItem);
        }

        public static IEnumerator<ConfirmPurchaseResult> Do(ConfirmPurchaseRequest request)
        {
            return Do<ConfirmPurchaseRequest, ConfirmPurchaseResult>(request, PlayFabClientAPI.ConfirmPurchase);
        }

        public static IEnumerator<ValidateIOSReceiptResult> Do(ValidateIOSReceiptRequest request)
        {
            return Do<ValidateIOSReceiptRequest, ValidateIOSReceiptResult>(request, PlayFabClientAPI.ValidateIOSReceipt);
        }

        public static IEnumerator<ValidateGooglePlayPurchaseResult> Do(ValidateGooglePlayPurchaseRequest request)
        {
            return Do<ValidateGooglePlayPurchaseRequest, ValidateGooglePlayPurchaseResult>(request, PlayFabClientAPI.ValidateGooglePlayPurchase);
        }
        
        public static IEnumerator<ValidateAmazonReceiptResult> Do(ValidateAmazonReceiptRequest request)
        {
            return Do<ValidateAmazonReceiptRequest, ValidateAmazonReceiptResult>(request, PlayFabClientAPI.ValidateAmazonIAPReceipt);
        }

        #endregion

        #region Friends Related Functions

        public static IEnumerator<AddFriendResult> Do(AddFriendRequest request)
        {
            // should update the cached list after it's done
            return Do<AddFriendRequest, AddFriendResult>(request, PlayFabClientAPI.AddFriend);
        }

        public static IEnumerator<RemoveFriendResult> Do(RemoveFriendRequest request)
        {
            // should update the cached list after it's done
            return Do<RemoveFriendRequest, RemoveFriendResult>(request, PlayFabClientAPI.RemoveFriend);
        }

        public static IEnumerator<GetFriendsListResult> Do(GetFriendsListRequest request)
        {
            // TODO [bgish] - should really cache this, and update friends list whenever add/remove.  If cached, then return the list instead of call this
            return Do<GetFriendsListRequest, GetFriendsListResult>(request, PlayFabClientAPI.GetFriendsList);
        }

        #endregion
        
        private static IEnumerator<Result> Do<Request, Result>(Request request, Action<Request, Action<Result>, Action<PlayFabError>, object> playfabFunction)
            where Request : class
            where Result : class
        {
            Result result = null;
            PlayFabError error = null;

            playfabFunction.Invoke(request, (r) => { result = r; }, (e) => { error = e; }, null);

            while (result == null && error == null)
            {
                yield return null;
            }

            if (error != null)
            {
                throw new PlayFabException(error);
            }

            yield return result;
        }
    }
}
