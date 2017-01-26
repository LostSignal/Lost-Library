//-----------------------------------------------------------------------
// <copyright file="CoroutineRunner.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CoroutineRunner : Lost.SingletonGameObject<CoroutineRunner>
    {
        public static Coroutine Start(IEnumerator coroutine)
        {
            return Instance.StartCoroutine(coroutine);
        }

        public static Coroutine<T> Start<T>(IEnumerator<T> coroutine)
        {
            return Instance.StartCoroutine<T>(coroutine);
        }
    }
}
