//-----------------------------------------------------------------------
// <copyright file="Cylinder.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [System.Serializable]
    public class Cylinder
    {
        #pragma warning disable 0649
        [SerializeField] private float radius;
        [SerializeField] private float height;
        #pragma warning restore 0649

        public Cylinder()
        {
            this.radius = 5.0f;
            this.height = 1.0f;
        }

        public Cylinder(float radius, float height)
        {
            this.radius = radius;
            this.height = height;
        }

        public float Radius
        {
            get { return this.radius; }
            set { this.radius = value; }
        }

        public float Height
        {
            get { return this.height; }
            set { this.height = value; }
        }

        public bool IsInside(Vector3 position, Vector3 point)
        {
            float sqrMagnitude = (position - point).sqrMagnitude;

            if (sqrMagnitude < this.radius * this.radius)
            {
                float heightDifference = Mathf.Abs(position.y - point.y);

                if (heightDifference < this.height / 2.0f)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
