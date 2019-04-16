//-----------------------------------------------------------------------
// <copyright file="LazyAsset.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER || USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class LazyAsset
    {
        #pragma warning disable 0649
        [SerializeField] private string assetGuid;
        #pragma warning restore 0649

        public string AssetGuid
        {
            get { return this.assetGuid; }

            #if UNITY_EDITOR
            set { this.assetGuid = value; }
            #endif
        }

        public virtual System.Type Type
        {
            get { return null; }
        }
    }
}

#endif
