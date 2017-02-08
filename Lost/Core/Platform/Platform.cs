//-----------------------------------------------------------------------
// <copyright file="Platform.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.IO;
    using UnityEngine;
    
    [Flags]
    public enum DevicePlatform
    {
        iOS               = 1 << 0,  // UNITY_IPHONE
        Android           = 1 << 1,  // UNITY_ANDROID
        Windows           = 1 << 2,  // UNITY_STANDALONE_WIN
        Mac               = 1 << 3,  // UNITY_STANDALONE_OSX
        Linux             = 1 << 4,  // UNITY_STANDALONE_LINUX
        WindowsUniversal  = 1 << 5,  // UNITY_WSA_10_0
        XboxOne           = 1 << 6,  // UNITY_XBOXONE
        WebGL             = 1 << 7,  // UNITY_WEBGL
    }

    //// TODO add events for pen and mouse detected, that way if someone uses a pen
    //// TODO controller too?  maybe only if InControl is detected?

    public abstract class Platform
    {        
        private static Platform instance;
        
        public static Platform Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = SingletonUtil.GetInstance<Platform>(GetInstance);
                }

                return instance;
            }
        }

        public abstract DevicePlatform CurrentDevicePlatform { get; }
        
        public virtual bool SupportsFileAccess 
        {
            get { return true; }
        }
        
        public virtual bool UnityAdsSupported
        {
            get { return false; }
        }

        public virtual bool IsTouchSupported
        {
            get { return UnityEngine.Input.touchSupported; }
        }
        
        public virtual bool IsMousePresent
        {
            get { return UnityEngine.Input.mousePresent; }
        }

        public virtual bool IsPenPresent
        {
            get { return false; }
        }
        
        public abstract void Sleep(int milliSeconds);
        
        public abstract bool DoesLocalFileExist(string localFileName);
        
        public abstract void SaveLocalFile(string localFileName, byte[] fileBytes);

        public abstract byte[] GetLocalFile(string localFileName);
        
        public virtual void QuitApplication()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public virtual void Vibrate(long milliseconds)
        {
            throw new NotImplementedException();
        }

        public virtual string GetStreamingAssetsURL(string path)
        {
            return "file://" + Path.Combine(Application.streamingAssetsPath, path).Replace(@"\", "/");
        }

        public virtual void RateApp()
        {
            throw new NotImplementedException();
        }

        public virtual void SendEmail(string email, string subject = null, string body = null)
        {
            string mailToUrl = "mailto:" + email;

            if (string.IsNullOrEmpty(subject) == false)
            {
                mailToUrl += "?subject=" + WWW.EscapeURL(subject).Replace("+", "%20");
            }

            if (string.IsNullOrEmpty(body) == false)
            {
                mailToUrl += "?body=" + WWW.EscapeURL(body).Replace("+", "%20");
            }

            Application.OpenURL(mailToUrl);
        }
        
        private static Platform GetInstance()
        {
            // all windows
            #if UNITY_STANDALONE_WIN
            return new WindowsPlatform();
            #elif UNITY_WSA_10_0
            return new WindowsUniversalPlatform();
            #elif UNITY_XBOXONE
            return new XboxOnePlatform();
            #endif

            // mobile
            #if UNITY_IPHONE
            return new iOSPlatform();
            #elif UNITY_ANDROID
            return new AndroidPlatform();
            #endif
            
            // web
            #if UNITY_WEBGL
            return new WebGLPlatform();
            #endif

            // mac / linux
            #if UNITY_STANDALONE_OSX
            return new MacPlatform();
            #elif UNITY_STANDALONE_LINUX
            return new LinuxPlatform();
            #endif
        }
    }
}
