//-----------------------------------------------------------------------
// <copyright file="RuntimeAppConfig.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lost.AppConfig
{
    [Serializable]
    public class RuntimeAppConfig
    {
        public const string FilePath = "Assets/Resources/appconfig.json";
        public const string ResourcePath = "appconfig";

        private static RuntimeAppConfig instance;

        public static RuntimeAppConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    var configJson = Resources.Load<TextAsset>(ResourcePath);

                    if (configJson != null && string.IsNullOrEmpty(configJson.text) == false)
                    {
                        instance = JsonUtility.FromJson<RuntimeAppConfig>(configJson.text);
                    }
                }

                return instance;
            }
        }

        public static void Reset()
        {
            instance = null;
        }

        [SerializeField] private string appConfigGuid;
        [SerializeField] private string appConfigName;
        [SerializeField] private List<KeyValuePair> keyValuePairs;

        private bool isInitialized = false;
        private Dictionary<string, string> values;
        private CloudBuildManifest cloudBuildManifest;
        private string versionAndBuildNumber;
        private string versionAndCommitId;
        private string version;

        public RuntimeAppConfig()
        {
        }

        public RuntimeAppConfig(string guid, string name, Dictionary<string, string> variables)
        {
            this.appConfigGuid = guid;
            this.appConfigName = name;
            this.cloudBuildManifest = CloudBuildManifest.Find();

            if (variables != null)
            {
                this.keyValuePairs = new List<KeyValuePair>();
                foreach (var keyValuePair in variables)
                {
                    this.keyValuePairs.Add(new KeyValuePair(keyValuePair.Key, keyValuePair.Value));
                }
            }
        }

        public string AppConfigGuid
        {
            get { return this.appConfigGuid; }
            set { this.appConfigGuid = value; }
        }

        public string AppConfigName
        {
            get { return this.appConfigName; }
            set { this.appConfigName = value; }
        }

        public string Version
        {
            get
            {
                this.Initialize();
                return this.version;
            }
        }

        public int BuildNumber
        {
            get
            {
                this.Initialize();
                return this.cloudBuildManifest == null ? 0 : this.cloudBuildManifest.BuildNumber;
            }
        }

        public string CommitId
        {
            get
            {
                this.Initialize();
                return this.cloudBuildManifest?.ScmCommitId;
            }
        }

        public string VersionAndBuildNumber
        {
            get
            {
                this.Initialize();
                return this.versionAndBuildNumber;
            }
        }

        public string VersionAndCommitId
        {
            get
            {
                this.Initialize();
                return this.versionAndCommitId;
            }
        }

        public string GetString(string key)
        {
            this.Initialize();

            string value;
            if (this.values.TryGetValue(key, out value))
            {
                return value;
            }

            return null;
        }

        private void Initialize()
        {
            if (this.isInitialized == false)
            {
                this.isInitialized = true;

                // caching all the key/value pairs in a dictionary
                this.values = new Dictionary<string, string>();

                for (int i = 0; i < this.keyValuePairs.Count; i++)
                {
                    this.values.Add(this.keyValuePairs[i].Key, this.keyValuePairs[i].Value);
                }

                this.cloudBuildManifest = CloudBuildManifest.Find();
                this.version = Application.version;
                this.versionAndBuildNumber = this.BuildNumber == 0 ? this.Version : string.Format("{0} ({1})", this.Version, this.BuildNumber);
                this.versionAndCommitId = this.CommitId == null ? this.Version : string.Format("{0} ({1})", this.Version, this.CommitId);
            }
        }

        [Serializable]
        public class KeyValuePair
        {
            [SerializeField] private string key;
            [SerializeField] private string value;

            public KeyValuePair() : this(null, null)
            {
            }

            public KeyValuePair(string key, string value)
            {
                this.key = key;
                this.value = value;
            }

            public string Key
            {
                get { return this.key; }
                set { this.key = value; }
            }

            public string Value
            {
                get { return this.value; }
                set { this.value = value; }
            }
        }
    }
}
