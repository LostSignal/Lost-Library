//-----------------------------------------------------------------------
// <copyright file="DelayDisableGameObject.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class DelayDisableGameObject : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private float secondsToWait = 2.0f;
        #pragma warning restore 0649

        private void OnEnable()
        {
            this.ExecuteDelayed(this.secondsToWait, this.DisableGameObject);
        }

        private void DisableGameObject()
        {
            this.gameObject.SafeSetActive(false);
        }
    }
}
