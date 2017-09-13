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
        public delegate void Action<T1, T2, T3, T4, T5>(T1 arg, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

        static PF()
        {
            PlayfabEvents = PlayFabEvents.Init();
            PlayFabSettings.TitleId = AppSettings.ActiveConfig.PlayfabTitleId;
            CatalogVersion = AppSettings.ActiveConfig.CatalogVersion;
            CloudScriptRevision = AppSettings.ActiveConfig.CloudScriptRevision;
        }

        public static PlayFabEvents PlayfabEvents { get; private set; }
        public static string CatalogVersion { get; private set; }
        public static int CloudScriptRevision { get; private set; }

        #region Login and linking with device id

        public static UnityTask<LoginResult> LoginWithDeviceId(bool createAccount, string deviceId, GetPlayerCombinedInfoRequestParams combinedInfoParams = null)
        {
            if (Application.isEditor || Platform.IsIosOrAndroid == false)
            {
                return Do<LoginWithCustomIDRequest, LoginResult>(
                    new LoginWithCustomIDRequest
                    {
                        CreateAccount = createAccount,
                        CustomId = deviceId,
                        InfoRequestParameters = combinedInfoParams,
                    },
                    PlayFabClientAPI.LoginWithCustomID);
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
            {
                return Do<LoginWithIOSDeviceIDRequest, LoginResult>(
                    new LoginWithIOSDeviceIDRequest
                    {
                        CreateAccount = createAccount,
                        DeviceId = deviceId,
                        DeviceModel = SystemInfo.deviceModel,
                        OS = SystemInfo.operatingSystem,
                        InfoRequestParameters = combinedInfoParams,
                    },
                    PlayFabClientAPI.LoginWithIOSDeviceID);
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.Android)
            {
                return Do<LoginWithAndroidDeviceIDRequest, LoginResult>(
                    new LoginWithAndroidDeviceIDRequest
                    {
                        CreateAccount = createAccount,
                        AndroidDeviceId = deviceId,
                        AndroidDevice = SystemInfo.deviceModel,
                        OS = SystemInfo.operatingSystem,
                        InfoRequestParameters = combinedInfoParams,
                    },
                    PlayFabClientAPI.LoginWithAndroidDeviceID);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static UnityTask<PlayFabResultCommon> LinkDeviceId(string deviceId)
        {
            return UnityTask<PlayFabResultCommon>.Run(LinkDeviceIdIterator(deviceId));
        }

        public static UnityTask<PlayFabResultCommon> UnlinkDeviceId(string deviceId)
        {
            return UnityTask<PlayFabResultCommon>.Run(UnlinkDeviceIdIterator(deviceId));
        }

        #endregion

        #region Login and linking with Facebook

        public static UnityTask<LoginResult> LoginWithFacebook(bool createAccount, GetPlayerCombinedInfoRequestParams combinedInfoParams = null)
        {
            IEnumerator<string> getAccessTokenCoroutine = GetFacebookAccessToken();
            string accessToken = null;

            while (getAccessTokenCoroutine.MoveNext())
            {
                accessToken = getAccessTokenCoroutine.Current;
            }

            var facebookLoginRequest = new LoginWithFacebookRequest
            {
                AccessToken = accessToken,
                CreateAccount = createAccount,
                InfoRequestParameters = combinedInfoParams,
            };

            return Do<LoginWithFacebookRequest, LoginResult>(facebookLoginRequest, PlayFabClientAPI.LoginWithFacebook);
        }

        public static UnityTask<LinkFacebookAccountResult> LinkFacebook()
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

        public static UnityTask<UnlinkFacebookAccountResult> UnlinkFacebook()
        {
            return Do<UnlinkFacebookAccountRequest, UnlinkFacebookAccountResult>(new UnlinkFacebookAccountRequest(), PlayFabClientAPI.UnlinkFacebookAccount);
        }

        #endregion

        #region CloudScript

        public static UnityTask<ExecuteCloudScriptResult> ExecuteCloudScript(string functionName, object functionParameters = null)
        {
            return PF.Do(new ExecuteCloudScriptRequest
            {
                FunctionName = functionName,
                RevisionSelection = CloudScriptRevisionOption.Specific,
                SpecificRevision = CloudScriptRevision,
                GeneratePlayStreamEvent = true,
                FunctionParameter = functionParameters,
            });
        }

        public static UnityTask<ExecuteCloudScriptResult> Do(ExecuteCloudScriptRequest request)
        {
            return Do<ExecuteCloudScriptRequest, ExecuteCloudScriptResult>(request, PlayFabClientAPI.ExecuteCloudScript);
        }

        public static T GetCloudScrtipResult<T>(UnityTask<ExecuteCloudScriptResult> result)
        {
            return PlayFab.Json.JsonWrapper.DeserializeObject<T>(result.Value.FunctionResult.ToString());
        }

        #endregion

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

        public static UnityTask<GetTimeResult> GetServerTime()
        {
            return Do<GetTimeRequest, GetTimeResult>(new GetTimeRequest(), PlayFabClientAPI.GetTime);
        }

        #region Purchasing Related Functions

        public static UnityTask<PurchaseItemResult> Do(PurchaseItemRequest request)
        {
            if (string.IsNullOrEmpty(request.CatalogVersion))
            {
                request.CatalogVersion = CatalogVersion;
            }

            return Do<PurchaseItemRequest, PurchaseItemResult>(request, PlayFabClientAPI.PurchaseItem);
        }

        #endregion

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

        public static UnityTask<GetStoreItemsResult> Do(GetStoreItemsRequest request)
        {
            if (string.IsNullOrEmpty(request.CatalogVersion))
            {
                request.CatalogVersion = CatalogVersion;
            }

            return Do<GetStoreItemsRequest, GetStoreItemsResult>(request, PlayFabClientAPI.GetStoreItems);
        }

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

        private static UnityTask<Result> Do<Request, Result>(Request request, Action<Request, Action<Result>, Action<PlayFabError>, object, Dictionary<string, string>> playfabFunction)
            where Request : class
            where Result : class
        {
            return UnityTask<Result>.Run(DoIterator(request, playfabFunction));
        }

        private static IEnumerator<Result> DoIterator<Request, Result>(Request request, Action<Request, Action<Result>, Action<PlayFabError>, object, Dictionary<string, string>> playfabFunction)
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

            yield return result;
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

        private static IEnumerator<PlayFabResultCommon> LinkDeviceIdIterator(string deviceId)
        {
            if (Application.isEditor || Platform.IsIosOrAndroid == false)
            {
                var coroutine = DoIterator<LinkCustomIDRequest, LinkCustomIDResult>(new LinkCustomIDRequest { CustomId = deviceId }, PlayFabClientAPI.LinkCustomID);

                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current as PlayFabResultCommon;
                }
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
            {
                var coroutine = DoIterator<LinkIOSDeviceIDRequest, LinkIOSDeviceIDResult>(
                    new LinkIOSDeviceIDRequest
                    {
                        DeviceId = deviceId,
                        DeviceModel = UnityEngine.SystemInfo.deviceModel,
                        OS = UnityEngine.SystemInfo.operatingSystem,
                    },
                    PlayFabClientAPI.LinkIOSDeviceID);

                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current as PlayFabResultCommon;
                }
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.Android)
            {
                var coroutine = DoIterator<LinkAndroidDeviceIDRequest, LinkAndroidDeviceIDResult>(
                    new LinkAndroidDeviceIDRequest
                    {
                        AndroidDeviceId = deviceId,
                        AndroidDevice = UnityEngine.SystemInfo.deviceModel,
                        OS = UnityEngine.SystemInfo.operatingSystem,
                    },
                    PlayFabClientAPI.LinkAndroidDeviceID);

                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current as PlayFabResultCommon;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static IEnumerator<PlayFabResultCommon> UnlinkDeviceIdIterator(string deviceId)
        {
            if (Application.isEditor || Platform.IsIosOrAndroid == false)
            {
                var coroutine = Do<UnlinkCustomIDRequest, UnlinkCustomIDResult>(new UnlinkCustomIDRequest { CustomId = deviceId }, PlayFabClientAPI.UnlinkCustomID);

                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current as PlayFabResultCommon;
                }
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
            {
                var coroutine = Do<UnlinkIOSDeviceIDRequest, UnlinkIOSDeviceIDResult>(new UnlinkIOSDeviceIDRequest { DeviceId = deviceId }, PlayFabClientAPI.UnlinkIOSDeviceID);

                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current as PlayFabResultCommon;
                }
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.Android)
            {
                var coroutine = Do<UnlinkAndroidDeviceIDRequest, UnlinkAndroidDeviceIDResult>(new UnlinkAndroidDeviceIDRequest { AndroidDeviceId = deviceId }, PlayFabClientAPI.UnlinkAndroidDeviceID);

                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current as PlayFabResultCommon;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

#endif
