//-----------------------------------------------------------------------
// <copyright file="LazyAsset.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections;
    using UnityEngine;

    [Serializable]
    #if UNITY_EDITOR
    public class LazyAsset<T> : ILazyAsset, ISerializationCallbackReceiver where T : UnityEngine.Object
    #else
    public class LazyAsset<T> : ILazyAsset where T : UnityEngine.Object
    #endif
    {
        #pragma warning disable 0649
        [SerializeField] private string assetGuid;
        [SerializeField] private string assetPath;
        [SerializeField] private string assetBundleName;
        #pragma warning restore 0649
        
        private bool isLoading;
        private T asset;

        public bool IsLoaded
        {
            get { return string.IsNullOrEmpty(this.assetPath) || this.asset != null; }
        }

        public T Asset
        {
            get
            {
                if (this.IsLoaded == false)
                {
                    Debug.LogErrorFormat("LazyAsset {0} was accessed before Load was called", this.assetPath);
                }

                if (this.isLoading)
                {
                    Debug.LogErrorFormat("Tried accessing LazyAsset {0} before loading finished.", this.assetPath);
                }

                return this.asset;
            }

            #if UNITY_EDITOR
            set
            {
                // load the current asset into memory if it exists
                if (this.asset == null && string.IsNullOrEmpty(this.assetPath) == false)
                {
                    this.asset = UnityEditor.AssetDatabase.LoadAssetAtPath(this.assetPath, typeof(T)) as T;
                }

                if (this.asset != value)
                {
                    this.asset = value;
                    this.assetPath = UnityEditor.AssetDatabase.GetAssetPath(this.asset);
                    this.assetGuid = UnityEditor.AssetDatabase.AssetPathToGUID(this.assetPath);
                    this.UpdateAssetInformation();
                }
            }
            #endif
        }

        public string AssetGuid
        {
            get { return this.assetGuid; }
        }

        public string AssetPath
        {
            get { return this.assetPath; }
        }

        public string AssetBundleName
        {
            get { return this.assetBundleName; }
        }
        
        #if UNITY_EDITOR
        
        public static string FindAssetBundleName(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return null;
            }

            string assetBundleName = UnityEditor.AssetImporter.GetAtPath(assetPath).assetBundleName;

            if (string.IsNullOrEmpty(assetBundleName))
            {
                return FindAssetBundleName(System.IO.Path.GetDirectoryName(assetPath));
            }
            else
            {
                return assetBundleName;
            }
        }

        public void UpdateAssetInformation()
        {
            if (string.IsNullOrEmpty(this.assetGuid))
            {
                this.assetPath = this.assetBundleName = null;
                return;
            }

            this.assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(this.assetGuid);
            this.assetBundleName = FindAssetBundleName(this.assetPath);
            
            if (string.IsNullOrEmpty(this.assetGuid) == false && string.IsNullOrEmpty(this.assetPath))
            {
                Debug.LogErrorFormat("LazyAsset {0} has invalid GUID.  Couldn't find AssetPath.", this.assetGuid);
            }
            else if (string.IsNullOrEmpty(this.assetPath) == false && string.IsNullOrEmpty(this.assetBundleName))
            {
                Debug.LogErrorFormat("LazyAsset {0} doesn't live in an asset bundle and will not work correctly.", this.assetPath);
            }
        }

        #endif

        public IEnumerator Load()
        {
            if (this.IsLoaded)
            {
                // do nothing
            }
            else if (this.isLoading)
            {
                Debug.LogErrorFormat("LazyAsset.Load() called on {0} multiple times before loading had finished.", this.assetPath);
            }
            else if (string.IsNullOrEmpty(this.assetPath))
            {
                // no valid assetPath, so nothing to load
            }
            else if (string.IsNullOrEmpty(this.assetPath) == false && string.IsNullOrEmpty(this.assetBundleName) == false)
            {
                this.isLoading = true;

                #if UNITY_EDITOR

                if (AssetBundleManager.SimulateAssetBundleInEditor)
                {
                    this.LoadWithUnityEditor();
                }
                else
                {
                    yield return LoadWithAssetBundleManager();
                }
                
                #else

                yield return LoadWithAssetBundleManager();

                #endif

                this.isLoading = false;
            }
            else if (string.IsNullOrEmpty(this.assetPath) == false && string.IsNullOrEmpty(this.assetBundleName))
            {
                Debug.LogErrorFormat("LazyAsset.Load() tried to load {0}, but has no asset bundle associated with it.", this.AssetPath);
            }
        }

        //// TODO [bgish]: Need to way to unload the asset, and it should have reference counting so the asset bundle should also unload if no assets reference it anymore

        #if UNITY_EDITOR

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
            this.UpdateAssetInformation();
        }

        private void LoadWithUnityEditor()
        {
            // load the current asset into memory if it exists
            if (this.asset == null && string.IsNullOrEmpty(this.assetPath) == false)
            {
                this.asset = UnityEditor.AssetDatabase.LoadAssetAtPath(this.assetPath, typeof(T)) as T;
            }
        }
        
        #endif

        private IEnumerator LoadWithAssetBundleManager()
        {
            var loadRequest = AssetBundleManager.LoadAssetAsync(this.assetBundleName, this.assetPath, typeof(T));
            yield return loadRequest;
            this.asset = loadRequest.GetAsset<T>();
        }
    }
}
