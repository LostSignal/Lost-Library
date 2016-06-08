//-----------------------------------------------------------------------
// <copyright file="MoveToSingletonContainer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class MoveToSingletonContainer : MonoBehaviour
    {
        private void Awake()
        {
            var singletonContainer = SingletonUtil.GetSingletonContainer();

            // if the container doesn't already contain this object, then reparent it
            if (singletonContainer.FindChild(this.name) == null)
            {
                this.transform.SetParent(singletonContainer);
            }
            else
            {
                GameObject.DestroyImmediate(this.gameObject);
            }
        }
    }
}
