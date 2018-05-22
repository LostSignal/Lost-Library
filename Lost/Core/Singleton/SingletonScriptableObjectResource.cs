//-----------------------------------------------------------------------
// <copyright file="SingletonScriptableObjectResource.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public abstract class SingletonScriptableObjectResource<T> : ScriptableObject where T : ScriptableObject
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

                string className = typeof(T).Name;

                // loading the scriptable object using the class name
                var resource = Resources.Load<T>(className);

                // instantiating the resource
                if (resource)
                {
                    instance = ScriptableObject.Instantiate<T>(resource);
                }

                if (!instance)
                {
                    Debug.LogErrorFormat("Couldn't load ScriptablObject {0}.  Is there a resource named \"{0}\" with a component of type {0} in the project?", className);
                }
            }
        }
    }
}
