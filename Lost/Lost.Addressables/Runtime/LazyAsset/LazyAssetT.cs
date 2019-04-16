//-----------------------------------------------------------------------
// <copyright file="LazyAsset.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER || USING_UNITY_ADDRESSABLES

namespace Lost
{
    #if UNITY_2018_3_OR_NEWER

    using System;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    [Serializable]
    public class LazyAsset<T> : LazyAsset, ILazyAsset, IValidate where T : UnityEngine.Object
    {
        // TODO [bgish]: Once addressables are working, make asset NonSerialized and load it from addressables

        #pragma warning disable 0649
        [SerializeField] private T asset;
        #pragma warning restore 0649

        [NonSerialized] private bool isLoaded = true;

        public override Type Type
        {
            get { return typeof(T); }
        }

        public T Asset
        {
            get
            {
                if (this.isLoaded == false)
                {
                    Debug.LogWarningFormat("Tried accessing asset {0} before loading it.", this.AssetGuid);
                }

                return this.asset;
            }
        }

        public bool IsLoaded
        {
            get { return this.isLoaded; }
        }

        public IAsyncOperation<T> LoadAsset()
        {
            if (this.isLoaded)
            {
                Debug.LogWarningFormat("Loading asset {0} when already loaded.", this.AssetGuid);
            }

            var loadOp = Addressables.LoadAsset<T>(this.GetRuntimeKey());

            loadOp.Completed += op =>
            {
                this.asset = op.Result;
                this.isLoaded = true;
            };

            return loadOp;
        }

        public void ReleaseAsset()
        {
            if (this.isLoaded == false)
            {
                Debug.LogWarningFormat("Tried releasing asset {0} when not loaded.", this.AssetGuid);
                return;
            }

            Addressables.ReleaseAsset(this.asset);
            this.asset = null;
            this.isLoaded = false;
        }

        private Hash128 GetRuntimeKey()
        {
            return Hash128.Parse(this.AssetGuid);
        }

        void IValidate.Validate()
        {
            #if UNITY_EDITOR
            // TODO [bgish]: Verify Guid is valid and the object can be cast as T
            throw new NotImplementedException();
            #endif
        }
    }

    #else

    [System.Serializable]
    public class LazyAsset<T> : LazyAsset, ILazyAsset
    {
        public bool IsLoaded => false;
    }

    #endif
}

#endif
