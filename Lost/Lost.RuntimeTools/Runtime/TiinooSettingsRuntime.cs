//-----------------------------------------------------------------------
// <copyright file="TiinooSettingsRuntime.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_TIINOO

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    public static class TiinooSettingsRuntime
    {
        public static readonly string InitializeAtStartupKey = "Tiinoo.InitializeAtStartup";

        [RuntimeInitializeOnLoadMethod]
        private static void OnStartup()
        {
            if (RuntimeAppConfig.Instance.GetBool(InitializeAtStartupKey))
            {
                Tiinoo.DeviceConsole.DCLoader.Load();
            }
        }
    }
}

#endif
