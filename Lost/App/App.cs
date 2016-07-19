//-----------------------------------------------------------------------
// <copyright file="App.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
   
    public static class App
    {
        private static AppData current = null;
        private static string appVersion = null;
        
        public static AppData Current
        {
            get { return current; }
        }

        public static string Version
        {
            get
            {
                if (appVersion == null)
                {
                    var versionTextAsset = Resources.Load<TextAsset>("version");
                    appVersion = versionTextAsset != null ? versionTextAsset.text : Application.version;
                }

                return appVersion;
            }
        }

        public static void Initialize(string resourcePath)
        {
            if (current != null)
            {
                Logger.LogError(current, "App.Current already set!");
                return;
            }

            current = Resources.Load(resourcePath) as AppData;
        }
    }
}
