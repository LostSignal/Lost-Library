//-----------------------------------------------------------------------
// <copyright file="ConcurrentStack.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class ConcurrentStack<T>
    {
        private readonly object itemsLock = new object();

        [SerializeField]
        private Stack<T> items = new Stack<T>();

        public void Push(T t)
        {
            lock (this.itemsLock)
            {
                this.items.Push(t);
            }
        }

        public bool TryPop(out T t)
        {
            lock (this.itemsLock)
            {
                if (this.items.Count > 0)
                {
                    t = this.items.Pop();
                    return true;
                }
                else
                {
                    t = default(T);
                    return false;
                }
            }
        }
    }
}
