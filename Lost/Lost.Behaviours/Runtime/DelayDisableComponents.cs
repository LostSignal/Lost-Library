//-----------------------------------------------------------------------
// <copyright file="DelayDisableComponents.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections;
    using UnityEngine;

    public class DelayDisableComponents : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private int framesToWait = 1;
        [SerializeField] private MonoBehaviour[] components;
        #pragma warning restore 0649

        private void OnEnable()
        {
            if (components == null || components.Length == 0)
            {
                return;
            }

            bool needsToRun = false;

            if (this.components.IsNullOrEmpty() == false)
            {
                for (int i = 0; i < this.components.Length; i++)
                {
                    needsToRun |= this.components[i].enabled;
                }
            }

            if (needsToRun)
            {
                CoroutineRunner.Instance.StartCoroutine(this.DisableCoroutine());
            }
        }

        private IEnumerator DisableCoroutine()
        {
            for (int i = 0; i < this.framesToWait; i++)
            {
                yield return null;
            }

            for (int i = 0; i < this.components.Length; i++)
            {
                if (this.components[i] != null)
                {
                    this.components[i].enabled = false;
                }
            }
        }
    }
}
