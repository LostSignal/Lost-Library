//-----------------------------------------------------------------------
// <copyright file="AssetBundleDownloadFromWebOperation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class AssetBundleDownloadFromWebOperation : AssetBundleDownloadOperation
    {
        private WWW www;
        private string url;

        public AssetBundleDownloadFromWebOperation(string assetBundleName, WWW www) : base(assetBundleName)
        {
            if (www == null)
            {
                throw new System.ArgumentNullException("www");
            }

            this.url = www.url;
            this.www = www;
        }

        protected override bool IsDownloadDone
        {
            get { return this.www == null || this.www.isDone; }
        }

        public override string GetSourceURL()
        {
            return this.url;
        }

        protected override void FinishDownload()
        {
            this.Error = this.www.error;

            if (string.IsNullOrEmpty(this.Error) == false)
            {
                return;
            }

            AssetBundle bundle = this.www.assetBundle;
            if (bundle == null)
            {
                this.Error = string.Format("{0} is not a valid asset bundle.", this.AssetBundleName);
            }
            else
            {
                this.AssetBundle = new LoadedAssetBundle(this.www.assetBundle);
            }

            this.www.Dispose();
            this.www = null;
        }
    }
}
