//-----------------------------------------------------------------------
// <copyright file="AndroidPlatform.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_ANDROID

namespace Lost
{
    using System.IO;
    using UnityEngine;

    public class AndroidPlatform : Platform
    {
        // TODO [bgish] - make sure <uses-permission android:name="android.permission.VIBRATE"/> is in the AndroidManifest.xml file
        #if !UNITY_EDITOR
        private static AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        private static AndroidJavaObject CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        private static AndroidJavaObject Vibrator = CurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        #endif
        
        public override DevicePlatform CurrentDevicePlatform
        {
            get { return DevicePlatform.Android; }
        }

        public override bool UnityAdsSupported
        {
            get { return true; }
        }
        
        public override void Sleep(int milliSeconds)
        {
            System.Threading.Thread.Sleep(milliSeconds);
        }

        public override bool DoesLocalFileExist(string localFileName)
        {
            try
            {
                return File.Exists(Path.Combine(UnityEngine.Application.persistentDataPath, localFileName));
            }
            catch
            {
                return false;
            }
        }

        public override byte[] GetLocalFile(string localFileName)
        {
            return File.ReadAllBytes(Path.Combine(UnityEngine.Application.persistentDataPath, localFileName));
        }

        public override void SaveLocalFile(string localFileName, byte[] fileBytes)
        {
            File.WriteAllBytes(Path.Combine(UnityEngine.Application.persistentDataPath, localFileName), fileBytes);
        }

        public override string GetStreamingAssetsURL(string path)
        {
            return Path.Combine(Application.streamingAssetsPath, path).Replace(@"\", "/");
        }

        public override void RateApp()
        {
            Application.OpenURL(string.Format("market://details?id={0}", Application.identifier));
        }

        public override void Vibrate(long milliseconds)
        {
            #if !UNITY_EDITOR
            Vibrator.Call("vibrate", milliseconds);
            #endif
        }
    }
}

#endif
