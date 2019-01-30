//-----------------------------------------------------------------------
// <copyright file="AppVersions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class AppVersionsData
    {
        [SerializeField] private List<AppVersion> appVersions = new List<AppVersion>();

        public List<AppVersion> AppVersions
        {
            get { return this.appVersions; }
            set { this.appVersions = value; }
        }
    }
}
