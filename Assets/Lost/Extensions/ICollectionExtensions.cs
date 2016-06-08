//-----------------------------------------------------------------------
// <copyright file="ICollectionExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    
    public static class ICollectionExtensions
    {
        public static void AddIfNotNull<T>(this ICollection<T> collection, T value) where T : class
        {
            if (value != null)
            {
                collection.Add(value);
            }
        }
        
        public static void AddIfNotNullAndUnique<T>(this ICollection<T> collection, T value) where T : class
        {
            if (value != null && collection.Contains(value) == false)
            {
                collection.Add(value);
            }
        }
    }
}
