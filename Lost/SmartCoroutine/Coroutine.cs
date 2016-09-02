//-----------------------------------------------------------------------
// <copyright file="Coroutine.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Heavily inspired by TwistedOakStudios SpecialCoroutines
    /// https://github.com/TwistedOakStudios/TOUnityUtilities/tree/master/Assets/TOUtilities
    /// https://www.youtube.com/watch?v=ciDD6Wl-Evk
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Coroutine<T>
    {
        private float timeoutInSeconds;
        private DateTime startTime;
        private T value;

        public Coroutine(MonoBehaviour gameObject, IEnumerator<T> coroutine, float timeoutInSeconds)
        {
            this.startTime = DateTime.Now;
            this.timeoutInSeconds = timeoutInSeconds;
            this.UnityCoroutine = gameObject.StartCoroutine(this.InternalCoroutine(coroutine));
        }

        public Coroutine UnityCoroutine { get; private set; }

        public Exception Exception { get; private set; }

        public bool HasError
        {
            get { return this.Exception != null; }
        }

        public bool IsCanceled { get; private set; }

        public bool DidTimeout { get; private set; }

        public bool IsDone { get; private set; }

        public T Value
        {
            get
            {
                if (this.HasError)
                {
                    throw this.Exception;
                }

                return this.value;
            }
        }

        public void Cancel()
        {
            this.IsCanceled = true;
            this.Exception = new CoroutineCanceledException();
        }

        private IEnumerator<T> InternalCoroutine(IEnumerator<T> coroutine)
        {
            while (true)
            {
                // checking if the user canceled it
                if (this.IsCanceled)
                {
                    yield break;
                }

                try
                {
                    if (coroutine.MoveNext() == false)
                    {
                        this.IsDone = true;
                        yield break;
                    }
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                    this.IsDone = true;
                    yield break;
                }

                // checking for timeout
                if (DateTime.Now.Subtract(this.startTime).TotalSeconds > this.timeoutInSeconds)
                {
                    this.Exception = new CoroutineTimeoutException();
                    this.DidTimeout = true;
                    this.IsDone = true;
                    yield break;
                }

                this.value = coroutine.Current;

                yield return this.value;
            }
        }
    }
}
