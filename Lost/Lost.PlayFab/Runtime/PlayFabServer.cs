//-----------------------------------------------------------------------
// <copyright file="PlayFabServer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections;
    using UnityEngine;

    public abstract class PlayFabServer<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Singleton Related Code

        private static object instanceLock = new object();
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (IsInitialized == false)
                {
                    Initialize();
                }

                return instance;
            }
        }

        public static bool IsInitialized
        {
            get { return instance != null; }
        }

        public static void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            // highly unlikely this would need a lock since all unity calls need to happen on the main thread, but hey, being safe
            lock (instanceLock)
            {
                if (IsInitialized)
                {
                    return;
                }

                T[] objects = GameObject.FindObjectsOfType<T>();
                string className = typeof(T).Name;

                if (objects != null && objects.Length != 0)
                {
                    Debug.LogErrorFormat(objects[0], "An object of type {0} already exists and wasn't created with Initialize function.", className);
                }
                else
                {
                    // constructing the singleton object
                    GameObject singleton = new GameObject(className, typeof(T));
                    singleton.transform.SetParent(SingletonUtil.GetSingletonContainer());
                    singleton.transform.Reset();

                    instance = singleton.GetComponent<T>();
                }
            }
        }

        #endregion

        // tracking time outs
        private bool regainFocusCoroutineRunning;
        private DateTime lostFocusTime = DateTime.UtcNow;

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogErrorFormat(instance, "Singleton GameObject {0} has been created multiple times!", typeof(T).Name);
            }

            PF.ServerNeedsRelogin += this.ServerNeedsRelogin;
        }

        protected abstract void ServerNeedsRelogin();

        protected virtual void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                double minutesSinceLostFocus = this.lostFocusTime.Subtract(DateTime.UtcNow).TotalMinutes;

                if (minutesSinceLostFocus > 1)
                {
                    this.StartCoroutine(this.RefreshServerTime());
                }
            }
            else
            {
                this.lostFocusTime = DateTime.UtcNow;
            }
        }

        private IEnumerator RefreshServerTime()
        {
            if (this.regainFocusCoroutineRunning)
            {
                yield break;
            }

            this.regainFocusCoroutineRunning = true;

            yield return PF.RefreshServerTime();

            this.regainFocusCoroutineRunning = false;
        }
    }
}

#endif
