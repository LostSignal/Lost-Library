//-----------------------------------------------------------------------
// <copyright file="AndroidStoreSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

////
//// * Switching Andriod Stores
////   * https://support.unity3d.com/hc/en-us/articles/213564363-How-do-I-build-for-different-Android-stores-
////   * UnityPurchasingEditor.TargetAndroidStore(AndroidStore.AmazonAppStore);
////   * HOW CAN I TELL WHICH IS THE TARGET STORE AT RUNTIME?????
////
//// ```csharp
//// var module = StandardPurchasingModule.Instance();
//// bool m_IsUnityChannelSelected = Application.platform == RuntimePlatform.Android && module.androidStore == AndroidStore.XiaomiMiPay;
//// ```
////

#if UNITY_IAP

namespace Lost
{
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEditor.Build;
    using UnityEngine;
    using UnityEngine.Purchaseing;

    [AppConfigSettingsOrder()]
    public class AndroidStoreSettings : BuildConfigSettings, IPreprocessBuild
    {
        #pragma warning disable 0649
        [SerializeField] private AndroidStore androidStore;
        #pragma warning restore 0649

        int IOrderedCallback.callbackOrder => 100;

        void IPreprocessBuild.OnPreprocessBuild(BuildTarget target, string path)
        {
            var storeSettings = EditorBuildConfig.ActiveConfig.GetSettings<AndroidStoreSettings>();

            if (storeSettings != null)
            {
                UnityPurchasingEditor.TargetAndroidStore(AndroidStore.AmazonAppStore);
            }
        }
    }
}

#endif
