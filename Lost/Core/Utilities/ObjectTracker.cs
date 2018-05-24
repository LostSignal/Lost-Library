//-----------------------------------------------------------------------
// <copyright file="ObjectTracker.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class ObjectTracker
    {
        private static Dictionary<Type, List<object>> objects = new Dictionary<Type, List<object>>();

        public static void Register<T>(T obj) where T : class
        {
            if (obj == null)
            {
                Debug.LogError("ObjectTracker tried to register a NULL object!");
                return;
            }

            Type objectType = typeof(T);

            List<object> objectList = null;

            if (objects.TryGetValue(objectType, out objectList))
            {
                if (Debug.isDebugBuild && objectList.Contains(obj))
                {
                    Debug.LogErrorFormat(obj as UnityEngine.Object, "ObjectTracker had an object of type {0} added multiple times!", objectType.Name);
                }
                else
                {
                    objectList.Add(obj);
                }
            }
            else
            {
                objects.Add(objectType, new List<object> { obj });
            }
        }

        public static void Deregister<T>(T obj) where T : class
        {
            if (obj == null)
            {
                Debug.LogError("ObjectTracker tried to deregister a NULL object!");
                return;
            }

            Type objectType = typeof(T);

            List<object> objectList = null;
            bool success = false;

            if (objects.TryGetValue(objectType, out objectList))
            {
                success = objectList.Remove(obj);
            }

            if (success == false)
            {
                Debug.LogErrorFormat(obj as UnityEngine.Object, "ObjectTracker tried to remove object of type {0} before adding it!", objectType.Name);
            }
        }

        public static IEnumerable<T> GetObjects<T>() where T : class
        {
            List<object> objectsList = null;

            if (objects.TryGetValue(typeof(T), out objectsList))
            {
                for (int i = 0; i < objectsList.Count; i++)
                {
                    yield return objectsList[i] as T;
                }
            }
        }
    }
}
