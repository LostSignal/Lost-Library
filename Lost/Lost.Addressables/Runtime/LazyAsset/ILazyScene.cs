//-----------------------------------------------------------------------
// <copyright file="ILazyScene.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY_2018_3_OR_NEWER || USING_UNITY_ADDRESSABLES

namespace Lost
{
    public interface ILazyScene
    {
        string AssetGuid { get; }
    }
}

#endif
