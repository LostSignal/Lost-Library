//-----------------------------------------------------------------------
// <copyright file="DebugMenuSettingsRuntime.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    public static class DebugMenuSettingsRuntime
    {
        public static readonly string InitializeAtStartupKey = "DebugMenu.InitializeAtStartup";

        // Overlay
        public static readonly string ShowAppVersionInLowerLeftKey = "DebugMenu.ShowAppVersionInLowerLeft";
        public static readonly string ShowPlayFabIdInLowerRightKey = "DebugMenu.ShowPlayFabIdInLowerRight";

        // Menu Options
        public static readonly string ShowTestAdKey = "DebugMenu.ShowTestAd";
        public static readonly string ShowToggleFpsKey = "DebugMenu.ShowToggleFps";
        public static readonly string ShowPrintAdsInfoKey = "DebugMenu.ShowPrintAdsInfo";

        [RuntimeInitializeOnLoadMethod]
        public static void OnStartup()
        {
            if (RuntimeAppConfig.Instance.GetBool(InitializeAtStartupKey) == false)
            {
                return;
            }

            if (RuntimeAppConfig.Instance.GetBool(ShowAppVersionInLowerLeftKey))
            {
                DebugMenu.Instance.SetText(Corner.LowerLeft, RuntimeAppConfig.Instance.VersionAndCommitId);
            }

            #if USING_PLAYFAB_SDK
            if (RuntimeAppConfig.Instance.GetBool(ShowPlayFabIdInLowerRightKey))
            {
                PF.PlayfabEvents.OnLoginResultEvent += PlayfabEvents_OnLoginResultEvent;
            }
            #endif

            if (RuntimeAppConfig.Instance.GetBool(ShowTestAdKey))
            {

                DebugMenu.Instance.AddItem("Show Test Ad", ShowTestAd);
            }

            if (RuntimeAppConfig.Instance.GetBool(ShowToggleFpsKey))
            {

                DebugMenu.Instance.AddItem("Toggle FPS", ToggleFps);
            }

            if (RuntimeAppConfig.Instance.GetBool(ShowPrintAdsInfoKey))
            {

                DebugMenu.Instance.AddItem("Print Ads Info", PrintAdsInfo);
            }
        }

        #if USING_PLAYFAB_SDK
        private static void PlayfabEvents_OnLoginResultEvent(PlayFab.ClientModels.LoginResult result)
        {
            DebugMenu.Instance.SetText(Corner.LowerRight, PF.Login.IsLoggedIn ? PF.User.PlayFabId : "Login Error!");
        }
        #endif

        private static void ShowTestAd()
        {
            AdsManager.Instance.ShowAd(null, false, (result) =>
            {
                Debug.Log("ShowAd Result String = " + result.ToString());
            });
        }

        private static void ToggleFps()
        {
            DebugMenu.Instance.ToggleFPS();
        }

        private static void PrintAdsInfo()
        {
            #if USING_UNITY_ADS
            Debug.Log("USING_UNITY_ADS Is On");
            #else
            Debug.Log("USING_UNITY_ADS Is Off");
            #endif

            #if UNITY_ADS
            Debug.Log("UNITY_ADS Is On");
            #else
            Debug.Log("UNITY_ADS Is Off");
            #endif

            Debug.Log("AreAdsInitialized: " + AdsManager.Instance.AreAdsInitialized);
            Debug.Log("AreAdsSupported: " + AdsManager.Instance.AreAdsSupported);
            Debug.Log("CurrentProviderName: " + AdsManager.Instance.CurrentProviderName);

            #if UNITY_ADS && UNITY_IOS && UNITY_ANDROID
            Debug.Log("UnityEngine.Advertisements.Advertisement.isSupported: " + UnityEngine.Advertisements.Advertisement.isSupported);
            Debug.Log("UnityEngine.Advertisements.Advertisement.isInitialized: " + UnityEngine.Advertisements.Advertisement.isInitialized);
            #endif
        }
    }
}
