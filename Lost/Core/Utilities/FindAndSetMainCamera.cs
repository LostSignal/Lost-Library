//-----------------------------------------------------------------------
// <copyright file="FindAndSetMainCamera.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(Canvas))]
    public class FindAndSetMainCamera : MonoBehaviour
    {
        private void Update()
        {
            if (Camera.main)
            {
                this.GetComponent<Canvas>().worldCamera = Camera.main;
                this.enabled = false;
            }
        }
    }
}
