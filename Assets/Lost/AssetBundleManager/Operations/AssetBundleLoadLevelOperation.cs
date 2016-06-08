//-----------------------------------------------------------------------
// <copyright file="AssetBundleLoadLevelOperation.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
    {
        private string assetBundleName;
        private string levelName;
        private bool isAdditive;
        private string downloadingError;
        private AsyncOperation request;

        public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
        {
            this.assetBundleName = assetbundleName;
            this.levelName = levelName;
            this.isAdditive = isAdditive;
        }

        public override bool Update()
        {
            if (this.request != null)
            {
                return false;
            }

            LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(this.assetBundleName, out this.downloadingError);
            if (bundle != null)
            {
                if (this.isAdditive)
                {
                    this.request = SceneManager.LoadSceneAsync(this.levelName, LoadSceneMode.Additive);
                }
                else
                {
                    this.request = SceneManager.LoadSceneAsync(this.levelName, LoadSceneMode.Single);
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool IsDone()
        {
            // return if meeting downloading error, downloadingError might come from the dependency downloading.
            if (this.request == null && this.downloadingError != null)
            {
                Debug.LogError(this.downloadingError);
                return true;
            }

            return this.request != null && this.request.isDone;
        }
    }
}
