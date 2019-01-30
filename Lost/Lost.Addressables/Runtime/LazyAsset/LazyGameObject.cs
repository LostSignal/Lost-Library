//-----------------------------------------------------------------------
// <copyright file="LazyGameObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class LazyGameObject : LazyAsset<GameObject>
    {
    }
}

#endif
