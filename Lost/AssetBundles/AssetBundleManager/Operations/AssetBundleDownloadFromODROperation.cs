//-----------------------------------------------------------------------
// <copyright file="AssetBundleDownloadFromODROperation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    #if ENABLE_IOS_ON_DEMAND_RESOURCES
    
    using UnityEngine.iOS;

    /// <summary>
    /// Read asset bundle asynchronously from iOS / tvOS asset catalog that is downloaded using on demand resources functionality.
    /// </summary>
    public class AssetBundleDownloadFromODROperation : AssetBundleDownloadOperation
    {
        private OnDemandResourcesRequest request;

        public AssetBundleDownloadFromODROperation(string assetBundleName) : base(assetBundleName)
        {
            this.request = OnDemandResources.PreloadAsync(new string[] { assetBundleName });
        }

        protected override bool IsDownloadDone
        {
            get
            {
                return (this.request == null) || this.request.isDone;
            }
        }

        public override string GetSourceURL()
        {
            return "odr://" + this.AssetBundleName;
        }

        protected override void FinishDownload()
        {
            this.Error = this.request.error;

            if (this.Error != null)
            {
                return;
            }   

            var path = "res://" + this.AssetBundleName;
            var bundle = UnityEngine.AssetBundle.LoadFromFile(path);
            if (bundle == null)
            {
                this.Error = string.Format("Failed to load {0}", path);
                this.request.Dispose();
            }
            else
            {
                this.AssetBundle = new LoadedAssetBundle(bundle);

                // At the time of unload request is already set to null, so capture it to local variable.
                var localRequest = this.request;
                
                // Dispose of request only when bundle is unloaded to keep the ODR pin alive.
                this.AssetBundle.Unload += () =>
                {
                    localRequest.Dispose();
                };
            }

            this.request = null;
        }
    }

    #endif
}
