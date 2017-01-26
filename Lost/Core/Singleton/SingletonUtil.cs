//-----------------------------------------------------------------------
// <copyright file="SingletonUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class SingletonUtil
    {
        private static Dictionary<Type, object> instances = new Dictionary<Type, object>();
        private static object instancesLock = new object();
        private static readonly string RootSingletonName = "Singletons";
        private static GameObject singletonRoot = null;

        /// <summary>
        /// Invokes the given function and returns the result.  It also makes sure that this
        /// only happens once every for the given Type.  You should not call this frequently, 
        /// you should only call it once and cache off the value.
        /// </summary>
        /// <typeparam name="T">The class to create.</typeparam>
        /// <param name="creator">The function that creates the instance of T.</param>
        /// <returns>The instance of class T.</returns>
        public static T GetInstance<T>(Func<T> creator) where T : class
        {
            lock (instancesLock)
            {
                if (instances.ContainsKey(typeof(T)) == false)
                {
                    instances.Add(typeof(T), creator.Invoke());
                }

                return instances[typeof(T)] as T;
            }
        }
        
        public static Transform GetSingletonContainer()
        {
            if (singletonRoot == null)
            {
                singletonRoot = GameObject.Find("/" + RootSingletonName) ?? new GameObject(RootSingletonName);
                GameObject.DontDestroyOnLoad(singletonRoot);
            }

            return singletonRoot.transform;
        }

        public static Transform GetOrCreateSingletonChildObject(string childName)
        {
            var containerTransform = GetSingletonContainer();

            var child = containerTransform.FindChild(childName);

            if (child == null)
            {
                var childGameObject = new GameObject(childName);
                childGameObject.transform.SetParent(containerTransform);
                childGameObject.transform.Reset();

                return childGameObject.transform;
            }
            else
            {
                return child;
            }
        }
    }
}
