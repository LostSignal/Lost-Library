//-----------------------------------------------------------------------
// <copyright file="AssetBundleLoadAssetOperationFull.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
    {
        private string assetBundleName;
        private string assetName;
        private string downloadingError;
        private System.Type type;
        private AssetBundleRequest request = null;

        public AssetBundleLoadAssetOperationFull(string bundleName, string assetName, System.Type type)
        {
            this.assetBundleName = bundleName;
            this.assetName = assetName;
            this.type = type;
        }

        protected AssetBundleRequest Request
        {
            get { return this.request; }
        }

        public override T GetAsset<T>()
        {
            if (this.request != null && this.request.isDone)
            {
                return this.request.asset as T;
            }
            else
            {
                return null;
            }
        }

        // Returns true if more Update calls are required.
        public override bool Update()
        {
            if (this.request != null)
            {
                return false;
            }

            LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(this.assetBundleName, out this.downloadingError);
            if (bundle != null)
            {
                // TODO: When asset bundle download fails this throws an exception...
                this.request = bundle.AssetBundle.LoadAssetAsync(this.assetName, this.type);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool IsDone()
        {
            // Return if meeting downloading error, downloadingError might come from the dependency downloading.
            if (this.request == null && this.downloadingError != null)
            {
                Debug.LogError(this.downloadingError);
                return true;
            }

            return this.request != null && this.request.isDone;
        }
    }
}
