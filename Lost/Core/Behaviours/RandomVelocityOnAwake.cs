//-----------------------------------------------------------------------
// <copyright file="RandomVelocityOnAwake.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class RandomVelocityOnAwake : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private float minVelocity = 0.5f;
        [SerializeField] private float maxVelocity = 1.0f;
        #pragma warning restore 0649

        private void Start()
        {
            var rigidBody = this.GetComponent<Rigidbody>();
            rigidBody.velocity = Random.insideUnitSphere.normalized * Random.Range(this.minVelocity, this.maxVelocity);
        }
    }
}
