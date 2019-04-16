//-----------------------------------------------------------------------
// <copyright file="LazyGameObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER || USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    #if UNITY_2018_3_OR_NEWER
    public class LazyGameObject : LazyAsset<GameObject>
    #else
    public class LazyGameObject : LazyAsset<object>
    #endif
    {
    }
}

#endif
