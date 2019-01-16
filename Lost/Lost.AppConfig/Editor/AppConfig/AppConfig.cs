//-----------------------------------------------------------------------
// <copyright file="AppConfig.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu]
    public class AppConfig : ScriptableObject
    {
        public AppConfig Parent;
        public List<AppConfigSettings> Settings = new List<AppConfigSettings>();
        public DevicePlatform SupportedPlatform;
        public List<string> Defines = new List<string>();

        [NonSerialized] public bool ShowInherited;

        public string SafeName
        {
            get { return this.name.Replace(" ", string.Empty); }
        }

        public T GetSettings<T>() where T : AppConfigSettings
        {
            return this.GetSettings(typeof(T)) as T;
        }

        public AppConfigSettings GetSettings(System.Type type)
        {
            bool isInherited;
            return GetSettings(type, out isInherited);
        }

        public AppConfigSettings GetSettings(System.Type type, out bool isInherited)
        {
            foreach (var settings in this.Settings)
            {
                if (settings != null && settings.GetType() == type)
                {
                    isInherited = false;
                    return settings;
                }
            }

            isInherited = true;
            return RecursiveGetSettings(this.Parent, type);
        }

        private AppConfigSettings RecursiveGetSettings(AppConfig parentBuildConfig, System.Type type)
        {
            if (parentBuildConfig == null)
            {
                return null;
            }

            if (parentBuildConfig.Settings != null)
            {
                foreach (var settings in parentBuildConfig.Settings)
                {
                    if (settings != null && settings.GetType() == type)
                    {
                        return settings;
                    }
                }
            }

            return RecursiveGetSettings(parentBuildConfig.Parent, type);
        }
    }
}
