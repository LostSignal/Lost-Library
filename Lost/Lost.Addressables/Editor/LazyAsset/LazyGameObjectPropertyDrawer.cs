//-----------------------------------------------------------------------
// <copyright file="LazyGameObjectPropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(LazyGameObject))]
    public class LazyGameObjectPropertyDrawer : LazyAssetPropertyDrawer<GameObject>
    {
    }
}
