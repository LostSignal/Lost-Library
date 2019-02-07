//-----------------------------------------------------------------------
// <copyright file="WeakReferenceT.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    public class WeakReferenceT<T> : WeakReference where T : Component
    {
        [NonSerialized] private T objectCache;

        public override Type Type => typeof(T);

        public T Value
        {
            get
            {
                if (this.objectCache != null)
                {
                    return this.objectCache;
                }

                if (this.Guid == null)
                {
                    // Log Error
                    return null;
                }

                var gameObject = GuidManager.Instance.GetGameObject(this.Guid);

                if (gameObject == null)
                {
                    // Log Error
                    return null;
                }

                this.objectCache = gameObject.GetComponent<T>();

                if (this.objectCache == null)
                {
                    // Log Error
                    return null;
                }

                return this.objectCache;
            }
        }
    }
}
