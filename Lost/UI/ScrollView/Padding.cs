//-----------------------------------------------------------------------
// <copyright file="Padding.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class Padding
    {
        #pragma warning disable 0649
        [SerializeField] private float top;
        [SerializeField] private float bottom;
        [SerializeField] private float left;
        [SerializeField] private float right;
        #pragma warning restore 0649

        public float Top
        {
            get { return this.top; }
        }

        public float Bottom
        {
            get { return this.bottom; }
        }

        public float Left
        {
            get { return this.left; }
        }

        public float Right
        {
            get { return this.right; }
        }
    }
}
