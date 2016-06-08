//-----------------------------------------------------------------------
// <copyright file="DestroyOnAwake.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public class DestroyOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            GameObject.DestroyImmediate(this.gameObject);
        }
    }
}
