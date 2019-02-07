//-----------------------------------------------------------------------
// <copyright file="WeakReference.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class WeakReference
    {
        #pragma warning disable 0649
        [SerializeField] private string guid;
        #pragma warning restore 0649

        public string Guid => this.guid;

        public virtual System.Type Type
        {
            get { return typeof(GameObject); }
        }
    }
}
