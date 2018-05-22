//-----------------------------------------------------------------------
// <copyright file="Pooled.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public enum PooledObjectState
    {
        Active,
        Inactive,
    }

    [AddComponentMenu("")]
    public class PooledObject : MonoBehaviour
    {
        private IPoolable[] poolableComponents;

        public Pool Pool { get; set; }

        public PooledObjectState State { get; set; }

        private bool isApplicationQuitting = false;

        private void Awake()
        {
            this.poolableComponents = this.GetComponents<IPoolable>();
        }

        private void OnDestroy()
        {
            #if UNITY_EDITOR
            this.isApplicationQuitting = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode == false;
            #endif

            if (this.isApplicationQuitting == false)
            {
                this.Pool.PermanentlyRemoveObjectFromPool(this);
            }
        }

        private void OnApplicationQuit()
        {
            this.isApplicationQuitting = true;
        }

        public void Recycle()
        {
            // telling the pool to recycle this object
            this.Pool.ReturnObjectToPool(this);

            if (this.poolableComponents.IsNullOrEmpty() == false)
            {
                // calling recycle on all components that want to know
                for (int i = 0; i < this.poolableComponents.Length; i++)
                {
                    this.poolableComponents[i].Recycle();
                }
            }
        }
    }
}
