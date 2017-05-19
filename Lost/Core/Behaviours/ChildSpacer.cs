//-----------------------------------------------------------------------
// <copyright file="ChildSpacer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class ChildSpacer : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 spacing;
        #pragma warning restore 0649

        public Vector3 StartPosition
        {
            get { return this.startPosition; }
        }

        public Vector3 Spacing
        {
            get { return this.spacing; }
        }
    }
}
