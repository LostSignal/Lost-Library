//-----------------------------------------------------------------------
// <copyright file="PF.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USE_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using PlayFab;
    using PlayFab.ClientModels;
    using PlayFab.Events;
    using PlayFab.SharedModels;
    using UnityEngine;

    public static class PF
    {
        private const string DeviceIdKey = "DeviceId";
        private static string deviceId;

        static PF()
        {
            PlayfabEvents = PlayFabEvents.Init();
            PlayfabEvents.OnLoginResultEvent += PlayfabEvents_OnLoginResultEvent;
        }

        public static PlayFabEvents PlayfabEvents { get; private set; }

        public static UserAccountInfo UserAccountInfo { get; private set; }

        public static LoginResult LoginResult { get; private set; }
        
        public static bool IsLoggedIn
        {
            get { return PlayFabClientAPI.IsClientLoggedIn(); }
        }

        public static string Id
        {
            get { return UserAccountInfo != null ? UserAccountInfo.PlayFabId : null; }
        }

        public static string FacebookId
        {
            get { return UserAccountInfo != null && UserAccountInfo.FacebookInfo != null ? UserAccountInfo.FacebookInfo.FacebookId : null; }
        }

        public static bool IsFacebookLinked
        {
            get { return string.IsNullOrEmpty(FacebookId) == false; }
        }

        #region Login and linking with device id

        public static string DeviceId
        {
            get
            {
                if (string.IsNullOrEmpty(deviceId))
                {
                    string playerPrefsDeviceId = PlayerPrefs.GetString(DeviceIdKey, null);

                    if (string.IsNullOrEmpty(playerPrefsDeviceId))
                    {
                        deviceId = Guid.NewGuid().ToString();
                        PlayerPrefs.SetString(DeviceIdKey, deviceId);
                        PlayerPrefs.Save();
                    }
                    else
                    {
                        deviceId = playerPrefsDeviceId;
                    }
                }

                return deviceId;
            }
        }

        public static bool IsDeviceLinked
        {
            get
            {
                #if UNITY_EDITOR || UNITY_STANDALONE
                return UserAccountInfo != null && UserAccountInfo.CustomIdInfo != null && UserAccountInfo.CustomIdInfo.CustomId == DeviceId;
                #elif UNITY_IOS
                return UserAccountInfo != null && UserAccountInfo.IosDeviceInfo != null && UserAccountInfo.IosDeviceInfo.IosDeviceId == DeviceId;
                #elif UNITY_ANDROID
                return UserAccountInfo != null && UserAccountInfo.AndroidDeviceInfo != null && UserAccountInfo.AndroidDeviceInfo.AndroidDeviceId == DeviceId;
                #endif
            }
        }
        
        public static void ResetDeviceId()
        {
            PlayerPrefs.DeleteKey(DeviceIdKey);
            PlayerPrefs.Save();
        }

        public static IEnumerator<LoginResult> LoginWithDeviceId(bool createAccount, GetPlayerCombinedInfoRequestParams infoRequest = null)
        {
            // making sure we get User Account Info at login
            infoRequest = infoRequest ?? new GetPlayerCombinedInfoRequestParams();
            infoRequest.GetUserAccountInfo = true;

            #if UNITY_EDITOR || UNITY_STANDALONE

            return Do<LoginWithCustomIDRequest, LoginResult>(
                new LoginWithCustomIDRequest
                {
                    CreateAccount = createAccount,
                    CustomId = DeviceId,
                    InfoRequestParameters = infoRequest
                }, 
                PlayFabClientAPI.LoginWithCustomID);
            
            #elif UNITY_ANDROID

            return Do<LoginWithAndroidDeviceIDRequest, LoginResult>(
                new LoginWithAndroidDeviceIDRequest
                {
                    CreateAccount = createAccount,
                    AndroidDeviceId = DeviceId,
                    AndroidDevice = UnityEngine.SystemInfo.deviceModel,
                    OS = UnityEngine.SystemInfo.operatingSystem,
                    InfoRequestParameters = infoRequest,
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
                    InfoRequestParameters = infoRequest,
                },
                PlayFabClientAPI.LoginWithIOSDeviceID);
            
            #endif
        }
        
        public static IEnumerator<PlayFabResultCommon> LinkDeviceId()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
            
            var coroutine = Do<LinkCustomIDRequest, LinkCustomIDResult>(new LinkCustomIDRequest { CustomId = DeviceId }, PlayFabClientAPI.LinkCustomID);
            
            #elif UNITY_ANDROID 
            
            var coroutine = Do<LinkAndroidDeviceIDRequest, LinkAndroidDeviceIDResult>(
                new LinkAndroidDeviceIDRequest
                {
                    AndroidDeviceId = DeviceId,
                    AndroidDevice = UnityEngine.SystemInfo.deviceModel,
                    OS = UnityEngine.SystemInfo.operatingSystem,
                }, 
                PlayFabClientAPI.LinkAndroidDeviceID);
            
            #elif UNITY_IOS

            var coroutine = Do<LinkIOSDeviceIDRequest, LinkIOSDeviceIDResult>(
                new LinkIOSDeviceIDRequest
                {
                    DeviceId = DeviceId,
                    DeviceModel = UnityEngine.SystemInfo.deviceModel,
                    OS = UnityEngine.SystemInfo.operatingSystem,
                }, 
                PlayFabClientAPI.LinkIOSDeviceID);

            #endif
            
            while (coroutine.MoveNext())
            {
                yield return coroutine.Current as PlayFabResultCommon;
            }
        }
        
        public static IEnumerator<PlayFabResultCommon> UnlinkDeviceId()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
            
            var coroutine = Do<UnlinkCustomIDRequest, UnlinkCustomIDResult>(new UnlinkCustomIDRequest { CustomId = DeviceId }, PlayFabClientAPI.UnlinkCustomID);
            
            #elif UNITY_ANDROID 
            
            var coroutine = Do<UnlinkAndroidDeviceIDRequest, UnlinkAndroidDeviceIDResult>(new UnlinkAndroidDeviceIDRequest { AndroidDeviceId = DeviceId }, PlayFabClientAPI.UnlinkAndroidDeviceID);
            
            #elif UNITY_IOS
            
            var coroutine = Do<UnlinkIOSDeviceIDRequest, UnlinkIOSDeviceIDResult>(new UnlinkIOSDeviceIDRequest { DeviceId = DeviceId }, PlayFabClientAPI.UnlinkIOSDeviceID);
            
            #endif
            
            while (coroutine.MoveNext())
            {
                yield return coroutine.Current as PlayFabResultCommon;
            }
        }

        #endregion

        #region Login and linking with Facebook

        public static IEnumerator<LoginResult> LoginWithFacebook(bool createAccount, GetPlayerCombinedInfoRequestParams infoRequest = null)
        {
            IEnumerator<string> getAccessTokenCoroutine = GetFacebookAccessToken();
            string accessToken = null;

            while (getAccessTokenCoroutine.MoveNext())
            {
                accessToken = getAccessTokenCoroutine.Current;
            }

            // making sure we get User Account Info at login
            infoRequest = infoRequest ?? new GetPlayerCombinedInfoRequestParams();
            infoRequest.GetUserAccountInfo = true;

            var facebookLoginRequest = new LoginWithFacebookRequest
            {
                AccessToken = accessToken,
                CreateAccount = createAccount,
                InfoRequestParameters = infoRequest,
            };

            return Do<LoginWithFacebookRequest, LoginResult>(facebookLoginRequest, PlayFabClientAPI.LoginWithFacebook);
        }

        public static IEnumerator<LinkFacebookAccountResult> LinkFacebook()
        {
            IEnumerator<string> getAccessTokenCoroutine = GetFacebookAccessToken();
            string accessToken = null;

            while (getAccessTokenCoroutine.MoveNext())
            {
                accessToken = getAccessTokenCoroutine.Current;
            }

            var request = new LinkFacebookAccountRequest { AccessToken = accessToken };

            return Do<LinkFacebookAccountRequest, LinkFacebookAccountResult>(request, PlayFabClientAPI.LinkFacebookAccount);
        }

        public static IEnumerator<UnlinkFacebookAccountResult> UnlinkFacebook()
        {
            return Do<UnlinkFacebookAccountRequest, UnlinkFacebookAccountResult>(new UnlinkFacebookAccountRequest(), PlayFabClientAPI.UnlinkFacebookAccount);
        }

        #endregion

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

        public static IEnumerator<WriteEventResponse> Do(WriteClientPlayerEventRequest request)
        {
            return Do<WriteClientPlayerEventRequest, WriteEventResponse>(request, PlayFabClientAPI.WritePlayerEvent);
        }

        #region User Data Related Functions

        public static IEnumerator<GetUserDataResult> Do(GetUserDataRequest request)
        {
            return Do<GetUserDataRequest, GetUserDataResult>(request, PlayFabClientAPI.GetUserData);
        }

        public static IEnumerator<UpdateUserDataResult> Do(UpdateUserDataRequest request)
        {
            return Do<UpdateUserDataRequest, UpdateUserDataResult>(request, PlayFabClientAPI.UpdateUserData);
        }

        #endregion

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

        public static IEnumerator<GetLeaderboardAroundPlayerResult> Do(GetLeaderboardAroundPlayerRequest request)
        {
            return Do<GetLeaderboardAroundPlayerRequest, GetLeaderboardAroundPlayerResult>(request, PlayFabClientAPI.GetLeaderboardAroundPlayer);
        }
        
        public static IEnumerator<GetFriendLeaderboardAroundPlayerResult> Do(GetFriendLeaderboardAroundPlayerRequest request)
        {
            return Do<GetFriendLeaderboardAroundPlayerRequest, GetFriendLeaderboardAroundPlayerResult>(request, PlayFabClientAPI.GetFriendLeaderboardAroundPlayer);
        }

        #endregion

        #region Purchasing Related Functions

        public static IEnumerator<GetStoreItemsResult> Do(GetStoreItemsRequest request)
        {
            return Do<GetStoreItemsRequest, GetStoreItemsResult>(request, PlayFabClientAPI.GetStoreItems);
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

        private static void PlayfabEvents_OnLoginResultEvent(LoginResult result)
        {
            LoginResult = result;
            UserAccountInfo = result.InfoResultPayload.AccountInfo;
        }

        private static IEnumerator<string> GetFacebookAccessToken()
        {
            #if !USE_FACEBOOK_SDK

            throw new FacebookException("USE_FACEBOOK_SDK is not defined!  Check your AppSettings.");
            
            #else

            if (Facebook.Unity.FB.IsInitialized == false)
            {
                bool initializationFinished = false;
                Facebook.Unity.FB.Init(() => { initializationFinished = true; });
            
                // waiting for FB to initialize
                while (initializationFinished == false)
                {
                    yield return null;
                }
            }
            
            if (Facebook.Unity.FB.IsInitialized == false)
            {
                throw new FacebookException("Initialization Failed!");
            }
            else if (Facebook.Unity.FB.IsLoggedIn == false)
            {
                Facebook.Unity.ILoginResult facebookLoginResult = null;
                Facebook.Unity.FB.LogInWithReadPermissions(new[] { "public_profile", "email", "user_friends" }, (loginResult) => { facebookLoginResult = loginResult; });
            
                // waiting for FB login to complete
                while (facebookLoginResult == null)
                {
                    yield return null;
                }

                // checking for errors
                if (facebookLoginResult.Cancelled)
                {
                    throw new FacebookException("User Canceled");
                }
                else if (facebookLoginResult.AccessToken == null)
                {
                    throw new FacebookException("AccessToken is Null!");
                }
                else if (string.IsNullOrEmpty(facebookLoginResult.AccessToken.TokenString))
                {
                    throw new FacebookException("TokenString is Null! or Empty!");
                }
            }

            yield return Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
            
            #endif
        }
    }
}

#endif
