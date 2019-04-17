//-----------------------------------------------------------------------
// <copyright file="TransformExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
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

        public static Coroutine LookAt(this Transform transform, Transform lookAtTransform, float time)
        {
            return CoroutineRunner.Instance.StartCoroutine(LookAtCoroutine());

            IEnumerator LookAtCoroutine()
            {
                Quaternion startRotation = transform.rotation;

                float currentTime = 0.0f;

                while (currentTime / time < 1.0f)
                {
                    Quaternion lookAtRotation = Quaternion.LookRotation(lookAtTransform.position - transform.position);

                    transform.rotation = Quaternion.Lerp(startRotation, lookAtRotation, currentTime / time);

                    currentTime += Time.deltaTime;

                    yield return null;
                }

                transform.rotation = Quaternion.LookRotation(lookAtTransform.position - transform.position);
            }
        }
    }
}
