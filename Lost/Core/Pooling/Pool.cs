//-----------------------------------------------------------------------
// <copyright file="Pool.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Pool
    {
        private List<GameObject> active = new List<GameObject>();
        private List<GameObject> inactive = new List<GameObject>();
        private Transform inactivePoolParent;

        public string PrefabName { get; private set; }

        public int InstanceId { get; private set; }

        // TODO [bgish]: these will need to come in later sometime
        // public int InitialSize { get; set; }
        // public int MaxSize { get; set; }

        public Pool(GameObject prefab, int initialCount = 0)
        {
            this.PrefabName = prefab.name;
            this.InstanceId = prefab.GetInstanceID();

            this.inactivePoolParent = new GameObject(string.Format("{0} ({1})", this.PrefabName, this.InstanceId)).transform;
            this.inactivePoolParent.SetParent(SingletonUtil.GetOrCreateSingletonChildObject("Pool"));

            if (initialCount < 1)
            {
                return;
            }

            var initialPooledObjects = new List<PooledObject>(initialCount);

            for (int i = 0; i < initialCount; i++)
            {
                initialPooledObjects.Add(this.GetObjectFromPool(prefab).GetComponent<PooledObject>());
            }

            for (int i = 0; i < initialCount; i++)
            {
                initialPooledObjects[i].Recycle();
            }
        }

        public void PermanentlyRemoveObjectFromPool(PooledObject pooledObject)
        {
            Debug.Assert(pooledObject.Pool == this, "Tried permanently removing pooled object from the wrong pool.");

            if (pooledObject.State == PooledObjectState.Active)
            {
                this.active.Remove(pooledObject.gameObject);
            }
            else if (pooledObject.State == PooledObjectState.Inactive)
            {
                this.inactive.Remove(pooledObject.gameObject);
            }
            else
            {
                Debug.LogErrorFormat("Pool.PermanentlyRemoveObjectFromPool found unknown Pooled State {0}", pooledObject.State);
            }

            Debug.LogWarningFormat("Pool.PermanentlyRemoveObjectFromPool called on {0}.  Could this have been avoided?", pooledObject.Pool.PrefabName);
        }

        public GameObject GetObjectFromPool(GameObject prefab, Transform parent = null)
        {
            Debug.Assert(prefab.GetInstanceID() == this.InstanceId, "Trying to get object pool using the wrong prefab.");

            if (this.inactive.Count > 0)
            {
                int lastIndex = this.inactive.Count - 1;
                var last = this.inactive[lastIndex];
                this.inactive.RemoveAt(lastIndex);
                this.active.Add(last);

                last.transform.SetParent(parent);
                last.SetActive(true);
                last.GetComponent<PooledObject>().State = PooledObjectState.Active;

                return last;
            }
            else
            {
                GameObject gameObject = parent == null ? GameObject.Instantiate(prefab) : GameObject.Instantiate(prefab, parent);

                var pooledObject = gameObject.AddComponent<PooledObject>();
                pooledObject.State = PooledObjectState.Active;
                pooledObject.Pool = this;

                this.active.Add(gameObject);

                return gameObject;
            }
        }

        public void ReturnObjectToPool(PooledObject pooledObject)
        {
            Debug.Assert(pooledObject.State == PooledObjectState.Active, "Tried returning an already inactive object to the pool.");
            Debug.Assert(pooledObject.Pool == this, "Tried returning object to the wrong pool.");

            pooledObject.State = PooledObjectState.Inactive;
            pooledObject.transform.SetParent(this.inactivePoolParent);
            pooledObject.gameObject.SetActive(false);

            this.active.Remove(pooledObject.gameObject);
            this.inactive.Add(pooledObject.gameObject);
        }
    }
}
