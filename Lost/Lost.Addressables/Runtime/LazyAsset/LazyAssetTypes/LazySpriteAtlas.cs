//-----------------------------------------------------------------------
// <copyright file="LazySpriteAtlas.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER || USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System;
    using UnityEngine.U2D;

    [Serializable]
    #if UNITY_2018_3_OR_NEWER
    public class LazySpriteAtlas : LazyAsset<SpriteAtlas>
    #else
    public class LazySpriteAtlas : LazyAsset<object>
    #endif
    {
    }
}

#endif
