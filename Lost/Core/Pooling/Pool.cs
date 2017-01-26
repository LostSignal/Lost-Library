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
        private List<GameObject> unactive = new List<GameObject>();
        
        public int InstanceId { get; set; }
        
        // TODO [bgish]: these will need to come in later sometime
        // public int InitialSize { get; set; }
        // public int MaxSize { get; set; } 

        public GameObject GetObjectFromPool(GameObject prefab, Transform parent, bool instantiateInWorldSpace)
        {
            if (this.unactive.Count > 0)
            {
                int lastIndex = this.unactive.Count - 1;
                var last = this.unactive[lastIndex];
                this.unactive.RemoveAt(lastIndex);
                this.active.Add(last);

                // setting it's new parent and making sure it's active
                if (parent != null)
                {
                    last.transform.SetParent(parent);

                    // TODO [bgish]: need to double check that this is the desired behavior
                    if (instantiateInWorldSpace)
                    {
                        last.transform.position = prefab.transform.position;
                    }
                    else
                    {
                        last.transform.localPosition = prefab.transform.position;
                    }
                }
                else
                {
                    last.transform.SetParent(null);
                }

                last.SetActive(true);

                return last;
            }
            else
            {
                GameObject gameObject = null;

                if (parent == null)
                {
                    gameObject = GameObject.Instantiate(prefab);
                }
                else
                {
                    gameObject = GameObject.Instantiate(prefab, parent, instantiateInWorldSpace);
                }

                gameObject.AddComponent<Pooled>().Pool = this;
                this.active.Add(gameObject);
                return gameObject;
            }
        }

        public void ReturnObjectToPool(GameObject gameObject)
        {
            this.active.Remove(gameObject);
            this.unactive.Add(gameObject);
        }
    }
}
