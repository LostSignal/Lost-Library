//-----------------------------------------------------------------------
// <copyright file="ConcurrentQueue.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ConcurrentQueue<T>
    {
        private readonly object itemsLock = new object();

        [SerializeField]
        private Queue<T> items = new Queue<T>();
                
        public bool TryDequeue(out T output)
        {
            lock (this.itemsLock)
            {
                if (this.items.Count > 0)
                {
                    output = this.items.Dequeue();
                    return true;
                }
                else
                {
                    output = default(T);
                    return false;
                }
            }
        }

        public void Enqueue(T t)
        {
            lock (this.itemsLock)
            {
                this.items.Enqueue(t);
            }
        }
    }
}
