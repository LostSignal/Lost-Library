//-----------------------------------------------------------------------
// <copyright file="LostLoggingSettingsRuntime.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{

    using Lost.AppConfig;

    using UnityEngine;

    public static class LostLoggingSettingsRuntime
    {
        public static readonly string InitializeAtStartupKey = "Lost.Logging.InitializeAtStartup";

        public static readonly string ForwardLoggingAsAnalyticsEventsKey = "Lost.Logging.ForwardLoggingAsAnalyticEvents";

        public static readonly string DontforwardInfoLevelLoggingKey = "Lost.Logging.IgnoreInfoLoggingLevel";



        [RuntimeInitializeOnLoadMethod]

        public static void OnStartup()

        {

            if (RuntimeAppConfig.Instance.GetBool(InitializeAtStartupKey))

            {

                LoggingManager.Initialize();

                LoggingManager.Instance.ForwardLoggingAsAnalyticEvents = RuntimeAppConfig.Instance.GetBool(ForwardLoggingAsAnalyticsEventsKey);

                LoggingManager.Instance.DontForwardInfoLevelLogging = RuntimeAppConfig.Instance.GetBool(DontforwardInfoLevelLoggingKey);

            }

        }
    }
}
