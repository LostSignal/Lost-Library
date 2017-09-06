//-----------------------------------------------------------------------
// <copyright file="LoadedAssetBundle.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
    /// </summary>
    public class LoadedAssetBundle
    {
        public LoadedAssetBundle(AssetBundle assetBundle)
        {
            this.AssetBundle = assetBundle;
            this.ReferencedCount = 1;
        }

        internal event Action Unload;

        public AssetBundle AssetBundle { get; set; }

        public int ReferencedCount { get; set; }

        internal void OnUnload()
        {
            this.AssetBundle.Unload(false);

            if (this.Unload != null)
            {
                this.Unload();
            }
        }
    }
}
