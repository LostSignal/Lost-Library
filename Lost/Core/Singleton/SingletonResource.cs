//-----------------------------------------------------------------------
// <copyright file="SingletonDialogResource.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public abstract class SingletonResource<T> : MonoBehaviour where T : MonoBehaviour
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
                    instance = GameObject.Instantiate<T>(Resources.Load<T>(className));

                    if (instance == null)
                    {
                        Debug.LogErrorFormat("Couldn't load Dialog {0}.  Is there a resource named \"{0}\" with a component of type {0} in the project?", className);
                        return;
                    }

                    instance.name = className;
                    instance.transform.SetParent(SingletonUtil.GetSingletonContainer());
                    instance.transform.Reset();
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
                Debug.LogErrorFormat(instance, "Singleton Resource {0} has been created multiple times!", typeof(T).Name);
            }
        }
    }
}
