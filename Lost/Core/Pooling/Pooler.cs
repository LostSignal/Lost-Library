//-----------------------------------------------------------------------
// <copyright file="Pooler.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public static class Pooler
    {
        // TODO [bgish]: need to figure out how to add items to the pool (scriptable object?  components register object?)
        private static Dictionary<int, Pool> pools = new Dictionary<int, Pool>();

        public static UnityEngine.GameObject Instantiate(UnityEngine.GameObject prefab)
        {
            return Instantiate(prefab, null, false);
        }

        public static UnityEngine.GameObject Instantiate(UnityEngine.GameObject prefab, Transform parent)
        {
            return Instantiate(prefab, parent, false);
        }

        public static UnityEngine.GameObject Instantiate(UnityEngine.GameObject prefab, Transform parent, bool instantiateInWorldSpace)
        {
            int instanceId = prefab.GetInstanceID();
            Pool pool = null;

            if (pools.TryGetValue(instanceId, out pool))
            {
                return pool.GetObjectFromPool(prefab, parent, instantiateInWorldSpace);
            }
            else
            {
                return GameObject.Instantiate(prefab, parent, instantiateInWorldSpace);
            }
        }
        
        public static T Instantiate<T>(T prefab) where T : UnityEngine.MonoBehaviour
        {
            int instanceId = prefab.gameObject.GetInstanceID();
            Pool pool = null;

            if (pools.TryGetValue(instanceId, out pool))
            {
                return pool.GetObjectFromPool(prefab.gameObject, null, false).GetComponent<T>();
            }
            else
            {
                return GameObject.Instantiate<T>(prefab);
            }
        }

        public static T Instantiate<T>(T prefab, Transform parent) where T : UnityEngine.MonoBehaviour
        {
            var result = Instantiate(prefab);
            result.transform.SetParent(parent);
            return result;
        }

        public static void Destroy(GameObject gameObject)
        {
            InternalDestory(gameObject, false);
        }

        public static void DestroyImmediate(GameObject gameObject)
        {
            InternalDestory(gameObject, true);
        }

        private static void InternalDestory(GameObject gameObject, bool destroyImmediate)
        {
            var pooled = gameObject.GetComponent<Pooled>();

            if (pooled != null)
            {
                pooled.Recycle();
            }
            else
            {
                if (destroyImmediate)
                {
                    GameObject.DestroyImmediate(gameObject);
                }
                else
                {
                    GameObject.Destroy(gameObject);
                }
            }
        }
    }
}
