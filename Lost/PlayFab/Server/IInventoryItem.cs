//-----------------------------------------------------------------------
// <copyright file="IInventoryItem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;

    public interface IInventoryItem
    {
        string Id { get; }

        int Count { get; }

        Dictionary<string, string> Data { get; }
    }
}
