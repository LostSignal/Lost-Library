//-----------------------------------------------------------------------
// <copyright file="LazySprite.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY || USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    #if UNITY
    public class LazySprite : LazyAsset<Sprite>
    #else
    public class LazySprite : LazyAsset<object>
    #endif
    {
    }
}

#endif
