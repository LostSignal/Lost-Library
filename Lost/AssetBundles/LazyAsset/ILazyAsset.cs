//-----------------------------------------------------------------------
// <copyright file="ILazyAsset.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
