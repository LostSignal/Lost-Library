//-----------------------------------------------------------------------
// <copyright file="Platform.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.IO;
    using System.Text;
    using UnityEngine;
    
    public enum DevicePlatform
    {
        Windows,           // UNITY_STANDALONE_WIN
        WindowsUniversal,  // UNITY_WSA_10_0
        XboxOne,           // UNITY_XBOXONE

        iOS,               // UNITY_IPHONE
        Android,           // UNITY_ANDROID

        WebGL,             // UNITY_WEBGL
        
        Mac,               // UNITY_STANDALONE_OSX
        Linux,             // UNITY_STANDALONE_LINUX
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
