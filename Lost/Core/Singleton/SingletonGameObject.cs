//-----------------------------------------------------------------------
// <copyright file="SingletonGameObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public abstract class SingletonGameObject<T> : MonoBehaviour where T : MonoBehaviour
    {
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

        /// <summary>
        /// Should be called before you use this Singleton GameObject.
        /// </summary>
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

        /// <summary>
        /// Called when object starts up, makes sure only one instance is created.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogErrorFormat(instance, "Singleton GameObject {0} has been created multiple times!", typeof(T).Name);
            }
        }
    }
}
