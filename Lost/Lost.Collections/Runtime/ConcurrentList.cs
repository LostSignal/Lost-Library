//-----------------------------------------------------------------------
// <copyright file="ConcurrentList.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ConcurrentList<T>
    {
        private readonly object itemsLock = new object();

        [SerializeField]
        private List<T> items = new List<T>();

        public void Add(T item)
        {
            lock (this.itemsLock)
            {
                this.items.Add(item);
            }
        }

        public T[] GetCopy()
        {
            lock (this.itemsLock)
            {
                return this.items.ToArray();
            }
        }

        public void RemoveAll(T[] itemsToRemove)
        {
            if (itemsToRemove == null || itemsToRemove.Length == 0)
            {
                return;
            }

            lock (this.itemsLock)
            {
                for (int i = 0; i < itemsToRemove.Length; i++)
                {
                    this.items.Remove(itemsToRemove[i]);
                }
            }
        }
    }
}
