//-----------------------------------------------------------------------
// <copyright file="LimitScreenSizeSettingsRuntime.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost{
    using Lost.AppConfig;
    using UnityEngine;    public static class LimitScreenSizeSettingsRuntime    {
        private static readonly int MaxScreenHeight = 1920;

        public static readonly string LimitScreenSizeKey = "LimitScreenSize";

        [RuntimeInitializeOnLoadMethod]
        private static void OnStartup()
        {
            if (RuntimeAppConfig.Instance.GetBool(LimitScreenSizeKey))
            {
                LimitScreenResolution();
            }
        }

        private static void LimitScreenResolution()
        {
            if (Screen.height > MaxScreenHeight)
            {
                float aspectRatio = Screen.width / (float)Screen.height;
                int newHeight = MaxScreenHeight;
                int newWidth = (int)(MaxScreenHeight * aspectRatio);

                Debug.LogFormat("Resizing Screen From {0}x{1} To {2}x{3}", Screen.width, Screen.height, newWidth, newHeight);
                Screen.SetResolution(newWidth, newHeight, true);
            }
        }
    }}