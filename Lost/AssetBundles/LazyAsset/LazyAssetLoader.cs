//-----------------------------------------------------------------------
// <copyright file="LazyAssetLoader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    //// TODO [bgish]: At a later date introduce the concept of limiting how many coroutines can run at once

    [Serializable]
    public class LazyAssetLoader : CustomYieldInstruction
    {
        private List<ILazyAsset> lazyAssets;
        private int maxCoroutineCount = 1;   // NOTE [bgish]: AssetBundleManager can't handle multiple coroutines querying it at once, so doing it one by one for now

        public LazyAssetLoader(List<ILazyAsset> lazyAssets)
        {
            this.lazyAssets = lazyAssets;

            Queue<ILazyAsset> assetQueue = new Queue<ILazyAsset>(this.lazyAssets);

            for (int i = 0; i < this.maxCoroutineCount; i++)
            {
                CoroutineRunner.Start(this.LoadAssetsFromQueue(assetQueue));
            }
        }
        
        private IEnumerator LoadAssetsFromQueue(Queue<ILazyAsset> assetQueue)
        {
            while (assetQueue.Count != 0)
            {
                var lazyAsset = assetQueue.Dequeue();

                if (lazyAsset.IsLoaded == false)
                {
                    yield return lazyAsset.Load();
                }
            }
        }

        public override bool keepWaiting
        {
            get { return this.lazyAssets.Any(x => x.IsLoaded == false); }
        }
    }
}
