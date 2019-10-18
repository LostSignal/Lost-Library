//-----------------------------------------------------------------------
// <copyright file="SpriteAtlasLoader.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.U2D;

    public class SpriteAtlasLoader : SingletonResource<SpriteAtlasLoader>
    {
        #pragma warning disable 0649
        [SerializeField] private List<Atlas> atlases = new List<Atlas>();
        #pragma warning restore 0649

        private Dictionary<string, Action<SpriteAtlas>> unknownAtlasRequests = new Dictionary<string, Action<SpriteAtlas>>();
        private Dictionary<string, Atlas> atlasesMap = null;

        #if UNITY_EDITOR
        public List<Atlas> Atlases => this.atlases;
        #endif

        public void RegisterAtlas(string tag, string guid)
        {
            Dictionary<string, Atlas> atlasMap = this.GetAtlasMap();
            atlasMap.Add(tag, new Atlas(tag, guid));

            if (this.unknownAtlasRequests.ContainsKey(tag))
            {
                this.RequestAtlas(tag, this.unknownAtlasRequests[tag]);
                this.unknownAtlasRequests.Remove(tag);
            }
        }

        public bool IsAtlasTagLoaded(string tag)
        {
            if (this.GetAtlasMap().TryGetValue(tag, out Atlas atlas))
            {
                return atlas.SpriteAtlas.IsLoaded;
            }

            return false;
        }

        public UnityTask<SpriteAtlas> LoadAtlasTag(string tag)
        {
            if (this.GetAtlasMap().TryGetValue(tag, out Atlas atlas))
            {
                return atlas.SpriteAtlas.Load();
            }

            return null;
        }

        public void UnloadAtlas(string tag)
        {
            if (this.GetAtlasMap().TryGetValue(tag, out Atlas atlas))
            {
                if (atlas.SpriteAtlas.IsLoaded)
                {
                    atlas.SpriteAtlas.Release();
                }
            }
        }

        private void OnEnable()
        {
            SpriteAtlasManager.atlasRequested += this.RequestAtlas;
        }

        private void OnDisable()
        {
            SpriteAtlasManager.atlasRequested -= this.RequestAtlas;
        }

        private Dictionary<string, Atlas> GetAtlasMap()
        {
            if (this.atlasesMap == null)
            {
                this.atlasesMap = new Dictionary<string, Atlas>();

                foreach (var atlas in this.atlases)
                {
                    this.atlasesMap.Add(atlas.Tag, atlas);
                }
            }

            return this.atlasesMap;
        }

        private void RequestAtlas(string tag, Action<SpriteAtlas> callback)
        {
            Dictionary<string, Atlas> atlasMap = this.GetAtlasMap();

            if (atlasMap.TryGetValue(tag, out Atlas atlas))
            {
                if (atlas.SpriteAtlas.IsLoaded)
                {
                    callback?.Invoke(atlas.SpriteAtlas.Load().Value);
                }
                else
                {
                    CoroutineRunner.Instance.StartCoroutine(LoadSpriteAtlasCoroutine());
                }
            }
            else
            {
                if (this.unknownAtlasRequests.ContainsKey(tag) == false)
                {
                    this.unknownAtlasRequests.Add(tag, callback);
                }
            }

            IEnumerator LoadSpriteAtlasCoroutine()
            {
                var loadAtlas = atlas.SpriteAtlas.Load();
                yield return loadAtlas;
                callback?.Invoke(loadAtlas.Value);
            }
        }

        [Serializable]
        public class Atlas
        {
            #pragma warning disable 0649
            [SerializeField] private string tag;
            [SerializeField] private LazySpriteAtlas spriteAtlas;
            #pragma warning restore 0649

            public Atlas(string tag, string guid)
            {
                this.tag = tag;
                this.spriteAtlas = new LazySpriteAtlas(guid);
            }

            public LazySpriteAtlas SpriteAtlas
            {
                get { return this.spriteAtlas; }
                set { this.spriteAtlas = value; }
            }

            public string Tag
            {
                get { return this.tag; }
                set { this.tag = value; }
            }
        }
    }
}
