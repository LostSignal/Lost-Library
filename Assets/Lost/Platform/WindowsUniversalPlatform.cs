//-----------------------------------------------------------------------
// <copyright file="WindowsUniversalPlatform.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_WSA_10_0

namespace Lost
{
    using System;
    using UnityEngine;

    public class WindowsUniversalPlatform : Platform
    {
        public override DevicePlatform CurrentDevicePlatform
        {
            get { return DevicePlatform.WindowsUniversal; }
        }

        public override void Sleep(int milliSeconds)
        {
            #if WINDOWS_UWP
            System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(milliSeconds)).Wait();
            #else
            System.Threading.Thread.Sleep(milliSeconds);
            #endif
        }

        public override bool DoesLocalFileExist(string localFileName)
        {
            return PlayerPrefs.HasKey(localFileName);
        }

        public override void SaveLocalFile(string localFileName, byte[] fileBytes)
        {
            PlayerPrefs.SetString(localFileName, System.Convert.ToBase64String(fileBytes));
            PlayerPrefs.Save();
        }

        public override byte[] GetLocalFile(string localFileName)
        {
            return System.Convert.FromBase64String(PlayerPrefs.GetString(localFileName));
        }
    }
}

#endif