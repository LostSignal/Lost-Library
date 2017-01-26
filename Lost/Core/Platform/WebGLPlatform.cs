//-----------------------------------------------------------------------
// <copyright file="WebGLPlatform.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_WEBGL

namespace Lost
{
    using System;
    
    public class WebGLPlatform : Platform
    {
        public override DevicePlatform CurrentDevicePlatform
        {
            get { return DevicePlatform.WebGL; }
        }
        
        public override void Sleep(int milliSeconds)
        {
            System.Threading.Thread.Sleep(milliSeconds);
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