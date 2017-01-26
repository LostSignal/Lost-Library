//-----------------------------------------------------------------------
// <copyright file="Pooled.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [AddComponentMenu("")]
    public class Pooled : MonoBehaviour
    {
        private static Transform poolContainer;

        private static Transform PoolContainer
        {
            get
            {
                if (poolContainer == null)
                {
                    poolContainer = SingletonUtil.GetOrCreateSingletonChildObject("Pool");
                }

                return poolContainer;
            }
        }

        private IPoolable[] poolableComponents;

        public Pool Pool { get; set; }

        private void Awake()
        {
            this.poolableComponents = this.GetComponents<IPoolable>();
        }

        public void Recycle()
        {
            // resetting the parent and disabling it
            this.transform.SetParent(PoolContainer);
            this.gameObject.SetActive(false);

            // telling the pool to recycle this object 
            this.Pool.ReturnObjectToPool(this.gameObject);

            if (this.poolableComponents == null)
            {
                return;
            }

            // calling recycle on all components that want to know
            for (int i = 0; i < this.poolableComponents.Length; i++)
            {
                this.poolableComponents[i].Recycle();
            }
        }
    }
}
