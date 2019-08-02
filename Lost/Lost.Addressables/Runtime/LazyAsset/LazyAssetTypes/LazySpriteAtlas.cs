//-----------------------------------------------------------------------
// <copyright file="LazySpriteAtlas.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER || USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System;

    #if UNITY_2018_3_OR_NEWER
    using UnityEngine.U2D;
    #endif

    [Serializable]
    #if UNITY_2018_3_OR_NEWER
    public class LazySpriteAtlas : LazyAsset<SpriteAtlas>
    #else
    public class LazySpriteAtlas : LazyAsset<object>
    #endif
    {
        #if UNITY_2018_3_OR_NEWER
        public LazySpriteAtlas()
        {
        }

        public LazySpriteAtlas(string guid) : base(guid)
        {
        }
        #endif
    }
}

#endif
