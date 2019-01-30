//-----------------------------------------------------------------------
// <copyright file="LazySprite.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class LazySprite : LazyAsset<Sprite>
    {
    }
}

#endif
