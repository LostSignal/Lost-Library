//-----------------------------------------------------------------------
// <copyright file="CloudBuildManifest.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class CloudBuildManifest
    {
        private Dictionary<string, object> dictionary;
        private string json;
        
        private CloudBuildManifest(string json)
        {
            this.json = json;
            this.dictionary = Lost.MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
        }

        public string CloudBuildTargetName
        {
            get { return this.GetString("cloudBuildTargetName"); }
        }

        public int BuildNumber
        {
            get { return (int)this.GetInt("buildNumber", -1); }
        }

        public string ScmCommitId
        {
            get { return this.GetString("scmCommitId"); }
        }

        public string ScmBranch
        {
            get { return this.GetString("scmBranch"); }
        }

        public string ProjectId
        {
            get { return this.GetString("projectId"); }
        }

        public string BundleId
        {
            get { return this.GetString("bundleId"); }
        }

        public string UnityVersion
        {
            get { return this.GetString("unityVersion"); }
        }

        //// TODO [bgish] - These are not consistant between PreExport and PostExport
        ////
        //// public string BuildStartTime
        //// {
        ////     get { return this.GetString("buildStartTime");  } // "2016-01-14T07:32:24.000Z" or "1/14/2016 7:32:24 AM"
        //// }
        //// 
        //// public string AssetBundlesHostingOption
        //// {
        ////     get { return this.GetString("assetBundles.hostingOption"); }
        //// }
        //// 
        //// public string AssetBundlesRelativePath
        //// {
        ////     get { return this.GetString("assetBundles.relativePath"); }
        //// }
        //// 
        //// TODO [bgish] - These also need to be added
        //// "assetBundles.localBundlesRelativePath":"AssetBundles/Android",
        //// "assetBundles.localBundles":["Android","redcircus"]
        //// 

        public static CloudBuildManifest Find()
        {
            var resourcesManifest = Resources.Load<TextAsset>("UnityCloudBuildManifest.json");
            var buildManifestJson = Path.Combine(Directory.GetCurrentDirectory(), "build_manifest.json");
            
            if (resourcesManifest != null)
            {
                return new CloudBuildManifest(resourcesManifest.text);
            }
            else if (File.Exists(buildManifestJson))
            {
                return new CloudBuildManifest(File.ReadAllText(buildManifestJson));
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return this.json;
        }

        private string GetString(string key, string defaultValue = "")
        {
            object value;
            if (this.dictionary.TryGetValue(key, out value))
            {
                return value as string;
            }
            else
            {
                return defaultValue;
            }
        }

        private int GetInt(string key, int defaultValue = 0)
        {
            object value;
            if (this.dictionary.TryGetValue(key, out value))
            {
                return (int)((long)value);
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
