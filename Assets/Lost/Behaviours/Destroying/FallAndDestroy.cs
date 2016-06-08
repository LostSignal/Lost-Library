//-----------------------------------------------------------------------
// <copyright file="FallAndDestroy.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class FallAndDestroy : MonoBehaviour
    {
        private void Update()
        {
            if (this.transform.position.y < -500.0f)
            {
                Pooler.Destroy(this.gameObject);
            }
        }
    }
}