//-----------------------------------------------------------------------
// <copyright file="LoginHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections;
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

        public string LastLoginEmail
        {
            get
            {
                return LostPlayerPrefs.GetString("LastLoginEmail", string.Empty);
            }

            set
            {
                LostPlayerPrefs.SetString("LastLoginEmail", value);
                LostPlayerPrefs.Save();
            }
        }

        public bool AutoLoginWithDeviceId
        {
            get
            {
                return LostPlayerPrefs.GetBool("AutoLoginWithDeviceId", false);
            }

            set
            {
                LostPlayerPrefs.SetBool("AutoLoginWithDeviceId", value);
                LostPlayerPrefs.Save();
            }
        }

        public bool HasEverLoggedIn
        {
            get
            {
                return LostPlayerPrefs.GetBool("HasEverLoggedIn", false);
            }

            set
            {
                LostPlayerPrefs.SetBool("HasEverLoggedIn", value);
                LostPlayerPrefs.Save();
            }
        }

        #region Login and linking with device id

        public IEnumerator LoginWithEmailAndPasswordDialog(GetPlayerCombinedInfoRequestParams infoRequestParams = null)
        {
            if (PF.Login.HasEverLoggedIn)
            {
                if (PF.Login.AutoLoginWithDeviceId)
                {
                    var login = PF.Login.LoginWithDeviceId(false, PF.Login.DeviceId, infoRequestParams);

                    yield return login;

                    if (login.HasError)
                    {
                        DialogManager.GetDialog<LogInDialog>().Show(infoRequestParams);
                    }
                    else if (PF.Login.IsLoggedIn)
                    {
                        yield break;
                    }
                    else
                    {
                        // NOTE [bgish]: This should never happen, but catching this case just in case
                        Debug.LogError("LoginHelper.LoginWithDeviceId failed, but didn't correctly report the error.");
                        DialogManager.GetDialog<LogInDialog>().Show(infoRequestParams);
                    }
                }
                else
                {
                    DialogManager.GetDialog<LogInDialog>().Show(infoRequestParams);
                }
            }
            else
            {
                DialogManager.GetDialog<SignUpDialog>().Show(infoRequestParams);
            }

            while (PF.Login.IsLoggedIn == false)
            {
                yield return null;
            }
        }

        public UnityTask<SendAccountRecoveryEmailResult> SendAccountRecoveryEmail(string email, string emailTemplateId)
        {
            return PF.Do<SendAccountRecoveryEmailRequest, SendAccountRecoveryEmailResult>(
                new SendAccountRecoveryEmailRequest
                {
                    Email = email,
                    EmailTemplateId = emailTemplateId,
                },
                PlayFabClientAPI.SendAccountRecoveryEmail);
        }

        public UnityTask<PlayFabLoginResultCommon> LoginWithEmail(bool createAccount, string email, string password, GetPlayerCombinedInfoRequestParams combinedInfoParams = null)
        {
            if (createAccount)
            {
                return PF.Do<RegisterPlayFabUserRequest, PlayFabLoginResultCommon>(
                    new RegisterPlayFabUserRequest
                    {
                        Email = email,
                        Password = password,
                        InfoRequestParameters = this.GetCombinedInfoRequest(combinedInfoParams),
                        RequireBothUsernameAndEmail = false,
                    },
                    PlayFabClientAPI.RegisterPlayFabUser);
            }
            else
            {
                return PF.Do<LoginWithEmailAddressRequest, PlayFabLoginResultCommon>(
                        new LoginWithEmailAddressRequest
                        {
                            Email = email,
                            Password = password,
                            InfoRequestParameters = this.GetCombinedInfoRequest(combinedInfoParams),
                        },
                        PlayFabClientAPI.LoginWithEmailAddress);
            }
        }

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

        public void LogoutOfFacebook()
        {
            #if !USING_FACEBOOK_SDK
            throw new FacebookException("USING_FACEBOOK_SDK is not defined!  Check your AppSettings.");
            #else

            if (Facebook.Unity.FB.IsLoggedIn)
            {
                Facebook.Unity.FB.LogOut();
            }

            #endif
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

                Debug.Log("Calling Facebook.Unity.FB.Init()...");
                Facebook.Unity.FB.Init(() => { initializationFinished = true; });

                // waiting for FB to initialize
                while (initializationFinished == false)
                {
                    yield return null;
                }
            }

            this.PrintFacebookInfo();

            if (Facebook.Unity.FB.IsInitialized == false)
            {
                throw new FacebookException("Initialization Failed!");
            }
            else if (Facebook.Unity.FB.IsLoggedIn == false || PF.User.HasFacebookPermissions(this.FacebookPermissions) == false)
            {
                this.FacebookLoginResult = null;

                Debug.Log("Calling Facebook.Unity.FB.LogInWithReadPermissions(...)");
                Facebook.Unity.FB.LogInWithReadPermissions(this.FacebookPermissions, (loginResult) => { this.FacebookLoginResult = loginResult; });

                // waiting for FB login to complete
                while (this.FacebookLoginResult == null)
                {
                    yield return null;
                }

                this.PrintFacebookInfo();

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
            #if !USING_FACEBOOK_SDK
            throw new FacebookException("USING_FACEBOOK_SDK is not defined!  Check your AppSettings.");
            #else

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

            #endif
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
            this.HasEverLoggedIn = true;
        }

        private void PlayfabEvents_OnGlobalErrorEvent(PlayFabRequestCommon request, PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.InvalidSessionTicket)
            {
                this.forceRelogin = true;
            }
        }

        #if USING_FACEBOOK_SDK
        private void PrintFacebookInfo()
        {
            Debug.Log("Facebook.Unity.FB.IsInitialized = " + Facebook.Unity.FB.IsInitialized);
            Debug.Log("Facebook.Unity.FB.IsLoggedIn = " + Facebook.Unity.FB.IsLoggedIn);

            var currentAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            var permissionsList = currentAccessToken?.Permissions;
            var permissionsString = (permissionsList != null ? string.Join(", ", permissionsList) : string.Empty);

            Debug.Log("Facebook.Unity.AccessToken.CurrentAccessToken.ExpirationTime = " + currentAccessToken?.ExpirationTime);
            Debug.Log("Facebook.Unity.AccessToken.CurrentAccessToken.LastRefresh = " + currentAccessToken?.LastRefresh);
            Debug.Log("Facebook.Unity.AccessToken.CurrentAccessToken.Permissions = " + permissionsString);
            Debug.Log("Facebook.Unity.AccessToken.CurrentAccessToken.UserId = " + currentAccessToken?.UserId);
            Debug.Log("Facebook.Unity.AccessToken.CurrentAccessToken.TokenString = " + currentAccessToken?.TokenString);
        }
        #endif
    }
}

#endif
