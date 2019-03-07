﻿//-----------------------------------------------------------------------
// <copyright file="LoginHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using Lost.AppConfig;
    using PlayFab;
    using PlayFab.ClientModels;
    using PlayFab.SharedModels;
    using UnityEngine;

    public class LoginHelper
    {
        private UserAccountInfo userAccountInfo;
        private bool forceRelogin;

        public string SessionTicket { get; private set; }

        #if USING_FACEBOOK_SDK
        public Facebook.Unity.ILoginResult FacebookLoginResult { get; private set; }

        public List<string> FacebookPermissions { get; private set; } = new List<string> { "public_profile", "email" };
        #endif

        public LoginHelper()
        {
            PF.PlayfabEvents.OnLoginResultEvent += this.PlayfabEvents_OnLoginResultEvent;
            PF.PlayfabEvents.OnGlobalErrorEvent += this.PlayfabEvents_OnGlobalErrorEvent;
        }

        public bool IsLoggedIn
        {
            get { return forceRelogin == false && PlayFabClientAPI.IsClientLoggedIn(); }
        }

        public void Logout()
        {
            this.userAccountInfo = null;
            this.forceRelogin = true;
            this.SessionTicket = null;
        }

        public bool IsDeviceLinked
        {
            get
            {
                if (Application.isEditor || Platform.IsIosOrAndroid == false)
                {
                    return this.userAccountInfo?.CustomIdInfo?.CustomId == this.DeviceId;
                }
                else if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
                {
                    return this.userAccountInfo?.IosDeviceInfo?.IosDeviceId == this.DeviceId;
                }
                else if (Platform.CurrentDevicePlatform == DevicePlatform.Android)
                {
                    return this.userAccountInfo?.AndroidDeviceInfo?.AndroidDeviceId == this.DeviceId;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public string DeviceId
        {
            get { return AnalyticsManager.Instance.AnonymousId; }
        }

        #region Login and linking with device id

        public UnityTask<LoginResult> LoginWithDeviceId(bool createAccount, string deviceId, GetPlayerCombinedInfoRequestParams combinedInfoParams = null)
        {
            if (Application.isEditor || Platform.IsIosOrAndroid == false)
            {
                return PF.Do<LoginWithCustomIDRequest, LoginResult>(
                    new LoginWithCustomIDRequest
                    {
                        CreateAccount = createAccount,
                        CustomId = deviceId,
                        InfoRequestParameters = this.GetCombinedInfoRequest(combinedInfoParams),
                    },
                    PlayFabClientAPI.LoginWithCustomID);
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
            {
                return PF.Do<LoginWithIOSDeviceIDRequest, LoginResult>(
                    new LoginWithIOSDeviceIDRequest
                    {
                        CreateAccount = createAccount,
                        DeviceId = deviceId,
                        DeviceModel = SystemInfo.deviceModel,
                        OS = SystemInfo.operatingSystem,
                        InfoRequestParameters = this.GetCombinedInfoRequest(combinedInfoParams),
                    },
                    PlayFabClientAPI.LoginWithIOSDeviceID);
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.Android)
            {
                return PF.Do<LoginWithAndroidDeviceIDRequest, LoginResult>(
                    new LoginWithAndroidDeviceIDRequest
                    {
                        CreateAccount = createAccount,
                        AndroidDeviceId = deviceId,
                        AndroidDevice = SystemInfo.deviceModel,
                        OS = SystemInfo.operatingSystem,
                        InfoRequestParameters = this.GetCombinedInfoRequest(combinedInfoParams),
                    },
                    PlayFabClientAPI.LoginWithAndroidDeviceID);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public UnityTask<PlayFabResultCommon> LinkDeviceId(string deviceId)
        {
            return UnityTask<PlayFabResultCommon>.Run(LinkDeviceIdIterator(deviceId));
        }

        public UnityTask<PlayFabResultCommon> UnlinkDeviceId(string deviceId)
        {
            return UnityTask<PlayFabResultCommon>.Run(UnlinkDeviceIdIterator(deviceId));
        }

        #endregion

        #region Login and linking with Facebook

        public UnityTask<LoginResult> LoginWithFacebook(bool createAccount)
        {
            return UnityTask<LoginResult>.Run(this.LoginWithFacebookCoroutine(createAccount, null, null));
        }

        public UnityTask<LoginResult> LoginWithFacebook(bool createAccount, List<string> facebookPermissions)
        {
            return UnityTask<LoginResult>.Run(this.LoginWithFacebookCoroutine(createAccount, null, facebookPermissions));
        }

        public UnityTask<LoginResult> LoginWithFacebook(bool createAccount, GetPlayerCombinedInfoRequestParams combinedInfoParams)
        {
            return UnityTask<LoginResult>.Run(this.LoginWithFacebookCoroutine(createAccount, combinedInfoParams, null));
        }
        
        public UnityTask<LoginResult> LoginWithFacebook(bool createAccount, GetPlayerCombinedInfoRequestParams combinedInfoParams, List<string> facebookPermissions)
        {
            return UnityTask<LoginResult>.Run(this.LoginWithFacebookCoroutine(createAccount, combinedInfoParams, facebookPermissions));
        }

        public UnityTask<LinkFacebookAccountResult> LinkFacebook()
        {
            return UnityTask<LinkFacebookAccountResult>.Run(this.LinkFacebookCoroutine());
        }

        public UnityTask<UnlinkFacebookAccountResult> UnlinkFacebook()
        {
            return PF.Do<UnlinkFacebookAccountRequest, UnlinkFacebookAccountResult>(new UnlinkFacebookAccountRequest(), PlayFabClientAPI.UnlinkFacebookAccount);
        }

        #endregion

        private UnityTask<string> GetFacebookAccessToken()
        {
            return UnityTask<string>.Run(this.GetFacebookAccessTokenCoroutine());
        }

        private IEnumerator<string> GetFacebookAccessTokenCoroutine()
        {
            #if !USING_FACEBOOK_SDK
            throw new FacebookException("USING_FACEBOOK_SDK is not defined!  Check your AppSettings.");
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
                this.FacebookLoginResult = null;

                Facebook.Unity.FB.LogInWithReadPermissions(this.FacebookPermissions, (loginResult) => { this.FacebookLoginResult = loginResult; });

                // waiting for FB login to complete
                while (this.FacebookLoginResult == null)
                {
                    yield return null;
                }

                // checking for errors
                if (this.FacebookLoginResult.Cancelled)
                {
                    throw new FacebookException("User Canceled");
                }
                else if (this.FacebookLoginResult.AccessToken == null)
                {
                    throw new FacebookException("AccessToken is Null!");
                }
                else if (string.IsNullOrEmpty(this.FacebookLoginResult.AccessToken.TokenString))
                {
                    throw new FacebookException("TokenString is Null! or Empty!");
                }
            }

            yield return Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
            #endif
        }

        private IEnumerator<LoginResult> LoginWithFacebookCoroutine(bool createAccount, GetPlayerCombinedInfoRequestParams combinedInfoParams, List<string> facebookPermissions)
        {
            // Making sure all passed in facebook permissions are appended to the global list
            if (facebookPermissions != null)
            {
                foreach (var facebookPermission in facebookPermissions)
                {
                    this.FacebookPermissions.AddIfUnique(facebookPermission);
                }
            }

            var accessToken = this.GetFacebookAccessToken();

            while (accessToken.IsDone == false)
            {
                yield return default(LoginResult);
            }

            var facebookLoginRequest = new LoginWithFacebookRequest
            {
                AccessToken = accessToken.Value,
                CreateAccount = createAccount,
                InfoRequestParameters = this.GetCombinedInfoRequest(combinedInfoParams),
            };

            var facebookLogin = PF.Do<LoginWithFacebookRequest, LoginResult>(facebookLoginRequest, PlayFabClientAPI.LoginWithFacebook);

            while (facebookLogin.IsDone == false)
            {
                yield return default(LoginResult);
            }

            yield return facebookLogin.Value;
        }

        private IEnumerator<LinkFacebookAccountResult> LinkFacebookCoroutine()
        {
            var accessToken = this.GetFacebookAccessToken();

            while (accessToken.IsDone == false)
            {
                yield return default(LinkFacebookAccountResult);
            }

            var request = new LinkFacebookAccountRequest { AccessToken = accessToken.Value };

            var link = PF.Do<LinkFacebookAccountRequest, LinkFacebookAccountResult>(request, PlayFabClientAPI.LinkFacebookAccount);

            while (link.IsDone == false)
            {
                yield return default(LinkFacebookAccountResult);
            }

            yield return link.Value;
        }

        private IEnumerator<PlayFabResultCommon> LinkDeviceIdIterator(string deviceId)
        {
            if (Application.isEditor || Platform.IsIosOrAndroid == false)
            {
                var coroutine = PF.DoIterator<LinkCustomIDRequest, LinkCustomIDResult>(new LinkCustomIDRequest { CustomId = deviceId }, PlayFabClientAPI.LinkCustomID);

                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current as PlayFabResultCommon;
                }
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
            {
                var coroutine = PF.DoIterator<LinkIOSDeviceIDRequest, LinkIOSDeviceIDResult>(
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
                var coroutine = PF.DoIterator<LinkAndroidDeviceIDRequest, LinkAndroidDeviceIDResult>(
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

        private IEnumerator<PlayFabResultCommon> UnlinkDeviceIdIterator(string deviceId)
        {
            if (Application.isEditor || Platform.IsIosOrAndroid == false)
            {
                var coroutine = PF.Do<UnlinkCustomIDRequest, UnlinkCustomIDResult>(new UnlinkCustomIDRequest { CustomId = deviceId }, PlayFabClientAPI.UnlinkCustomID);

                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current as PlayFabResultCommon;
                }
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
            {
                var coroutine = PF.Do<UnlinkIOSDeviceIDRequest, UnlinkIOSDeviceIDResult>(new UnlinkIOSDeviceIDRequest { DeviceId = deviceId }, PlayFabClientAPI.UnlinkIOSDeviceID);

                while (coroutine.MoveNext())
                {
                    yield return coroutine.Current as PlayFabResultCommon;
                }
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.Android)
            {
                var coroutine = PF.Do<UnlinkAndroidDeviceIDRequest, UnlinkAndroidDeviceIDResult>(new UnlinkAndroidDeviceIDRequest { AndroidDeviceId = deviceId }, PlayFabClientAPI.UnlinkAndroidDeviceID);

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

        private GetPlayerCombinedInfoRequestParams GetCombinedInfoRequest(GetPlayerCombinedInfoRequestParams request)
        {
            request = request ?? new GetPlayerCombinedInfoRequestParams();
            request.GetUserVirtualCurrency = true;
            request.GetUserInventory = true;
            request.GetUserAccountInfo = true;

            return request;
        }

        private void PlayfabEvents_OnLoginResultEvent(LoginResult result)
        {
            this.userAccountInfo = result?.InfoResultPayload?.AccountInfo;
            this.SessionTicket = result?.SessionTicket;
            this.forceRelogin = false;
        }

        private void PlayfabEvents_OnGlobalErrorEvent(PlayFabRequestCommon request, PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.InvalidSessionTicket)
            {
                this.forceRelogin = true;
            }
        }
    }
}

#endif