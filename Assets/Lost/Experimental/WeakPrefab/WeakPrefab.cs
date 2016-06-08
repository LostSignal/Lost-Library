//-----------------------------------------------------------------------
// <copyright file="WeakPrefab.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;
    
    [Serializable]
    public abstract class WeakPrefab
    {
        [SerializeField] private string prefabLocation;

        private UnityEngine.Object prefab;
        private bool ownesPrefab = false;

        public abstract Type PrefabeType { get; }
                
        public string PrefabLocation
        {
            get { return this.prefabLocation; }
        }
                
        public UnityEngine.Object Prefab
        {
            get
            {
                if (this.prefab == null && string.IsNullOrEmpty(this.prefabLocation) == false)
                {
                    this.ownesPrefab = true;
                    this.prefab = Resources.Load(this.prefabLocation);

                    if (this.prefab == null)
                    {
                        Logger.LogError("Found WeakPrefab that points to bad location {0}!", this.prefabLocation);
                        this.ownesPrefab = false;
                        this.prefabLocation = null;
                    }
                }

                return this.prefab;
            }

            set
            {
                if (this.prefab == value)
                {
                    return;
                }

                if (this.ownesPrefab && this.prefab != null)
                {
                    Resources.UnloadAsset(this.prefab);
                }

                this.prefab = null;
                this.prefabLocation = null;
                this.ownesPrefab = false;

                if (value != null)
                {
                    this.prefab = value;
                    
                    #if UNITY_EDITOR
                    this.prefabLocation = this.GetResourcePath(this.prefab);
                    #endif
                }
            }
        }

        //// TODO possible store a unique id for this prefab so if they move it we can find it again
        //// TODO we never call Resources.Unload!!!

        #if UNITY_EDITOR

        public bool IsValid()
        {
            GameObject gameObject = this.Prefab as GameObject;
            return gameObject != null && gameObject.GetComponent(this.PrefabeType) != null;
        }

        private string GetResourcePath(UnityEngine.Object obj)
        {
            string prefabAssetLocation = UnityEditor.AssetDatabase.GetAssetPath(obj.GetInstanceID());

            if (prefabAssetLocation.StartsWith("Assets/") && prefabAssetLocation.EndsWith(".prefab"))
            {
                // remove the Assets and .prefab from path (leaving the first '/' character)
                prefabAssetLocation = prefabAssetLocation.Substring(6, prefabAssetLocation.Length - 13);

                // now removing Resources folder from path
                int resourcesIndex = prefabAssetLocation.LastIndexOf("/Resources/");

                if (resourcesIndex != -1)
                {
                    return prefabAssetLocation.Substring(resourcesIndex + 11);
                }
            }

            return string.Empty;
        }
        
        #endif
    }
}