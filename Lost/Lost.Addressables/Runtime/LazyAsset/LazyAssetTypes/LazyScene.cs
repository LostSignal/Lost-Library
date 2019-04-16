//-----------------------------------------------------------------------
// <copyright file="LazyScene.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER || USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System;

    [Serializable]
    public class LazyScene : LazyAsset, ILazyScene
    {
        public bool IsLoaded => true;
    }
}

#endif
