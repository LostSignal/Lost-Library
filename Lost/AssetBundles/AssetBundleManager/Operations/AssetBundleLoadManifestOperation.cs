//-----------------------------------------------------------------------
// <copyright file="AssetBundleLoadManifestOperation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class AssetBundleLoadManifestOperation : AssetBundleLoadAssetOperationFull
    {
        public AssetBundleLoadManifestOperation(string bundleName, string assetName, System.Type type)
            : base(bundleName, assetName, type)
        {
        }

        public override bool Update()
        {
            base.Update();

            if (this.Request != null && this.Request.isDone)
            {
                AssetBundleManager.AssetBundleManifestObject = this.GetAsset<AssetBundleManifest>();
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
