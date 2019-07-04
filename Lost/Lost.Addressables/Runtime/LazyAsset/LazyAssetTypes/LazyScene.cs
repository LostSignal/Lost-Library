//-----------------------------------------------------------------------
// <copyright file="LazyScene.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER || USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System;
    using UnityEngine;

    #if UNITY_2018_3_OR_NEWER
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using UnityEngine.SceneManagement;
    #endif

    [Serializable]
    public class LazyScene : LazyAsset, ILazyScene
    {
        #if UNITY_2018_3_OR_NEWER
        private AsyncOperationHandle operation;

        public bool IsLoaded => true;

        public AsyncOperationHandle LoadScene(LoadSceneMode loadSceneMode)
        {
            this.operation = Addressables.LoadSceneAsync(this.RuntimeKey, LoadSceneMode.Additive);
            return this.operation;
        }

        public void Release()
        {
            if (this.operation.IsValid() == false)
            {
                Debug.LogWarning("Cannot release a null or unloaded asset.");
                return;
            }

            Addressables.Release(this.operation);
            this.operation = default(AsyncOperationHandle);
        }
        #endif
    }
}

#endif
