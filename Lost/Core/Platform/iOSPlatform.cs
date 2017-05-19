//-----------------------------------------------------------------------
// <copyright file="iOSPlatform.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_IOS

namespace Lost
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Would make it less readable.")]
    public class iOSPlatform : Platform
    {
        public override DevicePlatform CurrentDevicePlatform
        {
            get { return DevicePlatform.iOS; }
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

        public override void RateApp()
        {
            Application.OpenURL(string.Format("itms-apps://itunes.apple.com/app/{0}", Application.identifier));
        }

        public override void Vibrate(long milliseconds)
        {
            Handheld.Vibrate();
        }
    }
}

#endif
