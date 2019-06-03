//-----------------------------------------------------------------------
// <copyright file="SpriteAtlasLoader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.U2D;

    public class SpriteAtlasLoader : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private List<Atlas> atlases = new List<Atlas>();
        #pragma warning restore 0649

        public List<Atlas> Atlases => this.atlases;

        private void OnEnable()
        {
            SpriteAtlasManager.atlasRequested += RequestAtlas;
        }

        private void OnDisable()
        {
            SpriteAtlasManager.atlasRequested -= RequestAtlas;
        }

        private void RequestAtlas(string tag, Action<SpriteAtlas> callback)
        {
            foreach (var atlas in atlases)
            {
                if (atlas.Tag == tag)
                {
                    Addressables.LoadAssetAsync<SpriteAtlas>(atlas.Guid).Completed += (operation) =>
                    {
                        if (operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        {
                            callback(operation.Result);
                        }
                    };

                    return;
                }
            }

            Debug.LogErrorFormat("Unable to find atlas with tag {0}!", tag);
        }

        [Serializable]
        public class Atlas
        {
            #pragma warning disable 0649
            [SerializeField] private string tag;
            [SerializeField] private string guid;
            #pragma warning restore 0649

            public string Tag
            {
                get { return this.tag; }
                set { this.tag = value; }
            }

            public string Guid
            {
                get { return this.guid; }
                set { this.guid = value; }
            }
        }
    }
}
