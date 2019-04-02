//-----------------------------------------------------------------------
// <copyright file="LazyScene.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY || USING_UNITY_ADDRESSABLES

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
