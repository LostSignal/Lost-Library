//-----------------------------------------------------------------------
// <copyright file="ILazyScene.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY || USING_UNITY_ADDRESSABLES

namespace Lost
{
    public interface ILazyScene
    {
        string AssetGuid { get; }
    }
}

#endif
