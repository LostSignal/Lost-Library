//-----------------------------------------------------------------------
// <copyright file="WindowsPlatform.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN

namespace Lost
{
    using System.IO;
    
    public class WindowsPlatform : Platform
    {
        public override DevicePlatform CurrentDevicePlatform
        {
            get { return DevicePlatform.Windows; }
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
    }
}

#endif