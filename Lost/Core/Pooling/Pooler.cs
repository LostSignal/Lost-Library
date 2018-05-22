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

        public static void PoolPrefab<T>(T prefab, int initialCount = 0) where T : MonoBehaviour
        {
            PoolPrefab(prefab.gameObject, initialCount);
        }

        public static void PoolPrefab(GameObject prefab, int initialCount = 0)
        {
            int instanceId = prefab.GetInstanceID();

            if (pools.ContainsKey(instanceId))
            {
                Debug.LogWarningFormat("Tried pooling the same Prefab \"{0}\"multiple times.", prefab.name);
                return;
            }

            pools.Add(instanceId, new Pool(prefab, initialCount));
        }

        public static UnityEngine.GameObject Instantiate(UnityEngine.GameObject prefab)
        {
            return Instantiate(prefab, null);
        }

        public static UnityEngine.GameObject Instantiate(UnityEngine.GameObject prefab, Transform parent)
        {
            int instanceId = prefab.GetInstanceID();
            Pool pool = null;

            if (pools.TryGetValue(instanceId, out pool))
            {
                return pool.GetObjectFromPool(prefab, parent);
            }
            else
            {
                return GameObject.Instantiate(prefab, parent);
            }
        }

        public static T Instantiate<T>(T prefab) where T : UnityEngine.MonoBehaviour
        {
            return Instantiate<T>(prefab, null, false);
        }

        public static T Instantiate<T>(T prefab, bool reset) where T : UnityEngine.MonoBehaviour
        {
            return Instantiate<T>(prefab, null, reset);
        }

        public static T Instantiate<T>(T prefab, Transform parent) where T : UnityEngine.MonoBehaviour
        {
            return Instantiate<T>(prefab, parent, false);
        }

        public static T Instantiate<T>(T prefab, Transform parent, bool reset) where T : UnityEngine.MonoBehaviour
        {
            int instanceId = prefab.gameObject.GetInstanceID();
            Pool pool = null;

            if (pools.TryGetValue(instanceId, out pool))
            {
                var obj = pool.GetObjectFromPool(prefab.gameObject, parent).GetComponent<T>();

                if (reset)
                {
                    obj.transform.Reset();
                }

                return obj;
            }
            else
            {
                var obj = GameObject.Instantiate(prefab, parent);

                if (reset)
                {
                    obj.transform.Reset();
                }

                return obj;
            }
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
            var pooled = gameObject.GetComponent<PooledObject>();

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
