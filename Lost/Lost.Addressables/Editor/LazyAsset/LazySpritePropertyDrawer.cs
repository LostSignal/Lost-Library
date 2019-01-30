//-----------------------------------------------------------------------
// <copyright file="LazySpritePropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_ADDRESSABLES

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(LazySprite))]
    public class LazySpritePropertyDrawer : LazyAssetPropertyDrawer<Sprite>
    {
    }
}

#endif
