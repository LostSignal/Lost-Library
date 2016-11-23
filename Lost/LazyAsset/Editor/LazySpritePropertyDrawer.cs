//-----------------------------------------------------------------------
// <copyright file="LazySpritePropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(LazySprite))]
    public class LazySpritePropertyDrawer : LazyAssetPropertyDrawer<Sprite>
    {
    }
}
