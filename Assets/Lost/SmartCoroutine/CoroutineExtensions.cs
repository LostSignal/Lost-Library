//-----------------------------------------------------------------------
// <copyright file="CoroutineExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class CoroutineExtensions
    {
        public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour monobehaviour, IEnumerator<T> coroutine)
        {
            return new Coroutine<T>(monobehaviour, coroutine, float.MaxValue);
        }

        public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour monobehaviour, IEnumerator<T> coroutine, float timeoutInSeconds)
        {
            return new Coroutine<T>(monobehaviour, coroutine, timeoutInSeconds);
        }
    }
}
