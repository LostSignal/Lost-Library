//-----------------------------------------------------------------------
// <copyright file="PlayFabSettingsHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using System;
    using System.Reflection;

    public static class PlayFabSettingsHelper
    {
        #if UNITY

        public static void Initialize()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == "PlayFabServerSDK")
                {
                    Type t = assembly.GetType("PlayFab.PlayFabSettings");

                    // Setting the DeveloperSecretKey
                    FieldInfo developerSecretKeyField = t.GetField("DeveloperSecretKey", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    developerSecretKeyField.SetValue(null, EditorAppConfig.ActiveAppConfig.GetSettings<PlayFabSettings>().SecretKey);

                    // Setting the TitleId
                    FieldInfo titleIdField = t.GetField("TitleId", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    titleIdField.SetValue(null, EditorAppConfig.ActiveAppConfig.GetSettings<PlayFabSettings>().TitleId);
                }
            }
        }

        #else

        public static void Initialize(string titleId, string developerSecretKey)
        {
            PlayFab.PlayFabSettings.TitleId = titleId;
            PlayFab.PlayFabSettings.DeveloperSecretKey = developerSecretKey;
        }

        #endif
    }
}
