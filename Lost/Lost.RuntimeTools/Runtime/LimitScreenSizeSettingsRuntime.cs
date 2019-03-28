//-----------------------------------------------------------------------
// <copyright file="LimitScreenSizeSettingsRuntime.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost{
    using Lost.AppConfig;
    using UnityEngine;    public static class LimitScreenSizeSettingsRuntime    {
        private static readonly int MaxScreenSize = 1920;

        public static readonly string LimitScreenSizeKey = "LimitScreenSize";

        [RuntimeInitializeOnLoadMethod]
        private static void OnStartup()
        {
            if (RuntimeAppConfig.Instance.GetBool(LimitScreenSizeKey))
            {                LimitScreenResolution();
            }
        }

        private static void LimitScreenResolution()
        {
            bool isLandscape = Screen.width > Screen.height;

            if (isLandscape && Screen.width > MaxScreenSize)
            {
                float aspectRatio = Screen.height / (float)Screen.width;
                int newHeight = (int)(MaxScreenSize * aspectRatio);
                int newWidth = MaxScreenSize;

                Debug.LogFormat("Resizing Screen From {0}x{1} To {2}x{3}", Screen.width, Screen.height, newWidth, newHeight);
                Screen.SetResolution(newWidth, newHeight, true);
            }
            else if (isLandscape == false && Screen.height > MaxScreenSize)
            {
                float aspectRatio = Screen.width / (float)Screen.height;
                int newHeight = MaxScreenSize;
                int newWidth = (int)(MaxScreenSize * aspectRatio);

                Debug.LogFormat("Resizing Screen From {0}x{1} To {2}x{3}", Screen.width, Screen.height, newWidth, newHeight);
                Screen.SetResolution(newWidth, newHeight, true);
            }
        }
    }}