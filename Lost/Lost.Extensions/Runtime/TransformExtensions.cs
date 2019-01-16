//-----------------------------------------------------------------------
// <copyright file="TransformExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public static class TransformExtensions
    {
        public static void Reset(this Transform transform)
        {
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Pooler.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static void SafeSetActive(this Transform transform, bool active)
        {
            if (transform != null)
            {
                transform.gameObject.SafeSetActive(active);
            }
        }
    }
}
