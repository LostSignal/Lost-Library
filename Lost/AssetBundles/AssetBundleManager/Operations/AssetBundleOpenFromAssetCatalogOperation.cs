//-----------------------------------------------------------------------
// <copyright file="AssetBundleOpenFromAssetCatalogOperation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    #if ENABLE_IOS_APP_SLICING

    /// <summary>
    /// Read asset bundle synchronously from an iOS / tvOS asset catalog.
    /// </summary>
    public class AssetBundleOpenFromAssetCatalogOperation : AssetBundleDownloadOperation
    {
        public AssetBundleOpenFromAssetCatalogOperation(string assetBundleName) : base(assetBundleName)
        {
            var path = "res://" + assetBundleName;
            var bundle = UnityEngine.AssetBundle.LoadFromFile(path);

            if (bundle == null)
            {
                this.Error = string.Format("Failed to load {0}", path);
            }
            else
            {
                this.AssetBundle = new LoadedAssetBundle(bundle);
            }
        }

        protected override bool IsDownloadDone
        {
            get { return true; }
        }

        public override string GetSourceURL()
        {
            return "res://" + this.AssetBundleName;
        }

        protected override void FinishDownload()
        {
        }
    }

    #endif
}
