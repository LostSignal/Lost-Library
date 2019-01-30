//-----------------------------------------------------------------------
// <copyright file="ILazyAsset.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System.Collections;

    public interface ILazyAsset
    {
        string AssetGuid { get; }

        bool IsLoaded { get; }

        IEnumerator Load();
    }
}

#endif
