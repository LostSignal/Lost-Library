//-----------------------------------------------------------------------
// <copyright file="AppVersion.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class AppVersion
    {
        [SerializeField] private string version;
        [SerializeField] private string catalogVersion;
        [SerializeField] private int cloudScriptRevision;
        [SerializeField] private List<string> titleDataKeys;
        [SerializeField] private bool isDataOnlyVersion;
        [SerializeField] private List<CustomDataItem> customData;
        [SerializeField] private List<WhatsNew> whatsNew;
        [SerializeField] private AppUpdate appUpdate;

        public string Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        public string CatalogVersion
        {
            get { return this.catalogVersion; }
            set { this.catalogVersion = value; }
        }

        public int CloudScriptRevision
        {
            get { return this.cloudScriptRevision; }
            set { this.cloudScriptRevision = value; }
        }

        public List<string> TitleDataKeys
        {
            get { return this.titleDataKeys; }
            set { this.titleDataKeys = value; }
        }

        public bool IsDataOnlyVersion
        {
            get { return this.isDataOnlyVersion; }
            set { this.isDataOnlyVersion = value; }
        }

        public List<CustomDataItem> CustomData
        {
            get { return this.customData; }
            set { this.customData = value; }
        }

        // public List<WhatsNew> WhatsNew
        // {
        //     get { return this.whatsNew; }
        //     set { this.whatsNew = value; }
        // }
        //
        // public AppUpdate AppUpdate
        // {
        //     get { return this.appUpdate; }
        //     set { this.appUpdate = value; }
        // }

        [Serializable]
        public class AppUpdate
        {
            [SerializeField] private bool forceUserToUpdate;
            [SerializeField] private string appleStoreLink;
            [SerializeField] private string googlePlayLink;

            public bool ForceUserToUpdate
            {
                get { return this.forceUserToUpdate; }
                set { this.forceUserToUpdate = value; }
            }

            public string AppleStoreLink
            {
                get { return this.appleStoreLink; }
                set { this.appleStoreLink = value; }
            }

            public string GooglePlayLink
            {
                get { return this.googlePlayLink; }
                set { this.googlePlayLink = value; }
            }
        }

        [Serializable]
        public class WhatsNew
        {
            [SerializeField] private string language;
            [SerializeField] private string summary;
            [SerializeField] private List<string> bulletPoints;

            public string Language
            {
                get { return this.language; }
                set { this.language = value; }
            }

            public string Summary
            {
                get { return this.summary; }
                set { this.summary = value; }
            }

            public List<string> BulletPoints
            {
                get { return this.bulletPoints; }
                set { this.bulletPoints = value; }
            }
        }

        [Serializable]
        public class CustomDataItem
        {
            [SerializeField] private string key;
            [SerializeField] private string value;

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

        ////{
        ////    "Version": "0.9.0",
        ////    "CatalogVersion": "1.0",
        ////    "CloudScriptVersion": 7,
        ////    "TitleDataKeys": ["Challenges_1.2", "Levels_1.2", "Venues_1.2"],
        ////    "IsDataOnlyVersion": false,
        ////    "AppUpdate": {
        ////        "ForceUserToUpdate": true,
        ////        "AppLinks": {
        ////            "Apple": "",
        ////            "GooglePlay": "",
        ////            "Amazon": ""
        ////        },
        ////    },
        ////    "WhatsNew": {
        ////        "Enlgish": {
        ////            "Summary": "",
        ////            "BulletPoints": [
        ////                "",
        ////                "",
        ////                "",
        ////                ""
        ////            ]
        ////        },
        ////        "Vietnamese":  {
        ////            "Summary": "",
        ////            "BulletPoints": [
        ////                "",
        ////                "",
        ////                "",
        ////                ""
        ////            ]
        ////        }
        ////    }
        ////
        ////    "CustomData": {
        ////        "MatchMakingGameMode": "Default",
        ////        "MatchMakingBuildVersion": "0.6.2",
        ////        "MatchMakingRegions": [0],
        ////        "MatchMakingStartNewIfNoneFount": true,
        ////    }
        ////}
    }
}
