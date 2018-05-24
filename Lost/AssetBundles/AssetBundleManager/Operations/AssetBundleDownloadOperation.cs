//-----------------------------------------------------------------------
// <copyright file="AssetBundleDownloadOperation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    public abstract class AssetBundleDownloadOperation : AssetBundleLoadOperation
    {
        private bool done;

        public AssetBundleDownloadOperation(string assetBundleName)
        {
            this.AssetBundleName = assetBundleName;
        }

        public string AssetBundleName
        {
            get; private set;
        }

        public LoadedAssetBundle AssetBundle
        {
            get; protected set;
        }

        public string Error
        {
            get; protected set;
        }

        protected abstract bool IsDownloadDone { get; }

        public override bool Update()
        {
            if (!this.done && this.IsDownloadDone)
            {
                this.FinishDownload();
                this.done = true;
            }

            return !this.done;
        }

        public override bool IsDone()
        {
            return this.done;
        }

        public abstract string GetSourceURL();

        protected abstract void FinishDownload();
    }
}
