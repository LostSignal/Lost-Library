//-----------------------------------------------------------------------
// <copyright file="ILazyAsset.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if !UNITY || USING_UNITY_ADDRESSABLES

namespace Lost
{
    public interface ILazyAsset
    {
        string AssetGuid { get; }

        bool IsLoaded { get; }
    }
}

#endif
