//-----------------------------------------------------------------------
// <copyright file="Platform.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.IO;
    using Lost.AppConfig;
    using UnityEngine;
    using UnityEngine.Networking;

    //// NOTE [bgish]:  Windows Universal May Support System.IO.File class now
    //// TODO add events for pen and mouse detected, that way if someone uses a pen
    //// TODO controller too?  maybe only if InControl is detected?

    public static class Platform
    {
        // TODO [bgish] - make sure <uses-permission android:name="android.permission.VIBRATE"/> is in the AndroidManifest.xml file
        #if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        private static AndroidJavaObject CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        private static AndroidJavaObject Vibrator = CurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        #endif

        // TODO [bgish]: Try to make this function work by purely using Application.platform
        public static DevicePlatform CurrentDevicePlatform
        {
            get
            {
                // all windows
                #if UNITY_STANDALONE_WIN
                return DevicePlatform.Windows;
                #elif UNITY_WSA_10_0
                return DevicePlatform.WindowsUniversal;
                #elif UNITY_XBOXONE
                return DevicePlatform.XboxOne;
                #endif

                // mobile
                #if UNITY_IPHONE || UNITY_IOS
                return DevicePlatform.iOS;
                #elif UNITY_ANDROID
                return DevicePlatform.Android;
                #endif

                // web
                #if UNITY_WEBGL
                return DevicePlatform.WebGL;
                #endif

                // mac / linux
                #if UNITY_STANDALONE_OSX
                return DevicePlatform.Mac;
                #elif UNITY_STANDALONE_LINUX
                return DevicePlatform.Linux;
                #endif

                //// switch (Application.platform)
                //// {
                ////     case RuntimePlatform.IPhonePlayer:
                ////         return DevicePlatform.iOS;
                ////
                ////     case RuntimePlatform.Android:
                ////         return DevicePlatform.Android;
                ////
                ////     case RuntimePlatform.BlackBerryPlayer:
                ////     case RuntimePlatform.FlashPlayer:
                ////
                ////     case RuntimePlatform.LinuxPlayer:
                ////
                ////     case RuntimePlatform.MetroPlayerARM:
                ////     case RuntimePlatform.MetroPlayerX64:
                ////     case RuntimePlatform.MetroPlayerX86:
                ////
                ////     case RuntimePlatform.WSAPlayerARM:
                ////     case RuntimePlatform.WSAPlayerX64:
                ////     case RuntimePlatform.WSAPlayerX86:
                ////
                ////     case RuntimePlatform.OSXPlayer:
                ////
                ////     case RuntimePlatform.PS4:
                ////     case RuntimePlatform.Switch:
                ////
                ////     case RuntimePlatform.WindowsPlayer:
                ////     case RuntimePlatform.XboxOne:
                ////
                ////     case RuntimePlatform.WebGLPlayer:
                ////     case RuntimePlatform.LinuxEditor:
                ////     case RuntimePlatform.NaCl:
                ////     case RuntimePlatform.OSXDashboardPlayer:
                ////     case RuntimePlatform.OSXEditor:
                ////     case RuntimePlatform.OSXWebPlayer:
                ////     case RuntimePlatform.PS3:
                ////     case RuntimePlatform.PSM:
                ////     case RuntimePlatform.PSP2:
                ////     case RuntimePlatform.SamsungTVPlayer:
                ////     case RuntimePlatform.TizenPlayer:
                ////     case RuntimePlatform.tvOS:
                ////     case RuntimePlatform.WiiU:
                ////     case RuntimePlatform.WindowsEditor:
                ////     case RuntimePlatform.WindowsWebPlayer:
                ////     case RuntimePlatform.WP8Player:
                ////     case RuntimePlatform.XBOX360:
                ////     default:
                ////         break;
                //// }
            }
        }

        public static bool IsUnityCloudBuild
        {
            get
            {
                // TODO [bgish]: Should be able to figure this out by finding build config json file
                #if UNITY_CLOUD_BUILD
                return true;
                #else
                return false;
                #endif
            }
        }

        public static bool IsIosOrAndroid
        {
            get
            {
                switch (CurrentDevicePlatform)
                {
                    case DevicePlatform.iOS:
                    case DevicePlatform.Android:
                        return true;

                    default:
                        return false;
                }
            }
        }

        public static bool IsTouchSupported
        {
            get { return UnityEngine.Input.touchSupported; }
        }

        public static bool IsMousePresent
        {
            get { return UnityEngine.Input.mousePresent; }
        }

        public static bool IsPenPresent
        {
            get { return false; }
        }

        public static void QuitApplication()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public static void Vibrate(long milliseconds)
        {
            switch (CurrentDevicePlatform)
            {
                case DevicePlatform.Android:
                    #if UNITY_ANDROID && !UNITY_EDITOR
                    Vibrator.Call("vibrate", milliseconds);
                    #endif
                    break;

                case DevicePlatform.iOS:
                    #if UNITY_IOS
                    Handheld.Vibrate();
                    #endif
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public static string GetStreamingAssetsURL(string path)
        {
            switch (CurrentDevicePlatform)
            {
                case DevicePlatform.Android:
                    return Path.Combine(Application.streamingAssetsPath, path).Replace(@"\", "/");

                default:
                    return "file://" + Path.Combine(Application.streamingAssetsPath, path).Replace(@"\", "/");
            }
        }

        public static void RateApp()
        {
            switch (CurrentDevicePlatform)
            {
                case DevicePlatform.Android:
                    Application.OpenURL(string.Format("market://details?id={0}", Application.identifier));
                    break;

                case DevicePlatform.iOS:
                    Application.OpenURL(string.Format("itms-apps://itunes.apple.com/app/{0}", Application.identifier));
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public static void SendEmail(string email, string subject = null, string body = null)
        {
            string mailToUrl = "mailto:" + email;

            if (string.IsNullOrEmpty(subject) == false)
            {
                mailToUrl += "?subject=" + UnityWebRequest.EscapeURL(subject).Replace("+", "%20");
            }

            if (string.IsNullOrEmpty(body) == false)
            {
                mailToUrl += "?body=" + UnityWebRequest.EscapeURL(body).Replace("+", "%20");
            }

            Application.OpenURL(mailToUrl);
        }

        //// NOTE [bgish]: Probably wont need these till we start saving Analytics and Logging to disk
        ////
        //// public static bool DoesLocalFileExist(string localFileName)
        //// {
        ////     switch (CurrentDevicePlatform)
        ////     {
        ////         case DevicePlatform.WebGL:
        ////         case DevicePlatform.XboxOne:
        ////         case DevicePlatform.WindowsUniversal:
        ////             {
        ////                 return PlayerPrefs.HasKey(localFileName);
        ////             }
        ////
        ////         default:
        ////             {
        ////                 try
        ////                 {
        ////                     return File.Exists(Path.Combine(Application.persistentDataPath, localFileName));
        ////                 }
        ////                 catch
        ////                 {
        ////                     return false;
        ////                 }
        ////             }
        ////     }
        //// }
        ////
        //// public static byte[] GetLocalFile(string localFileName)
        //// {
        ////     switch (CurrentDevicePlatform)
        ////     {
        ////         case DevicePlatform.WebGL:
        ////         case DevicePlatform.XboxOne:
        ////         case DevicePlatform.WindowsUniversal:
        ////             return Convert.FromBase64String(PlayerPrefs.GetString(localFileName));
        ////
        ////         default:
        ////             return File.ReadAllBytes(Path.Combine(Application.persistentDataPath, localFileName));
        ////     }
        //// }
        ////
        //// public static void SaveLocalFile(string localFileName, byte[] fileBytes)
        //// {
        ////     switch (CurrentDevicePlatform)
        ////     {
        ////         case DevicePlatform.WebGL:
        ////         case DevicePlatform.XboxOne:
        ////         case DevicePlatform.WindowsUniversal:
        ////             PlayerPrefs.SetString(localFileName, Convert.ToBase64String(fileBytes));
        ////             PlayerPrefs.Save();
        ////             break;
        ////
        ////         default:
        ////             File.WriteAllBytes(Path.Combine(Application.persistentDataPath, localFileName), fileBytes);
        ////             break;
        ////     }
        //// }
    }
}
