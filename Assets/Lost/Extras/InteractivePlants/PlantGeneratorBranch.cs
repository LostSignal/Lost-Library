//-----------------------------------------------------------------------
// <copyright file="PlantGeneratorBranch.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlantGenerator
{
    using UnityEngine;
 
    /// <summary>
    /// This class represents a branch of the plant generator.  You setup a mesh with an aim target and this 
    /// class will make sure the mesh points towards that aim target.  In order for everything to work 
    /// properly, the branch should be facing down the forward vector (0, 0, 1).
    /// </summary>
    public class PlantGeneratorBranch : MonoBehaviour
    {
        /// <summary>
        /// How fast the MeshRoot should lerp towards the mesh aim target.
        /// </summary>
        public float LerpSpeed = 5;

        /// <summary>
        /// The transform that the MeshRoot should always point at.
        /// </summary>
        public Transform AimTarget;

        /// <summary>
        /// The Mesh's parent that will be rotated to make sure this mesh always points to the MeshAimTarget.
        /// </summary>
        public Transform Mesh;

        /// <summary>
        /// 
        /// </summary>
        private Rigidbody aimTargetRigidbody;

        /// <summary>
        /// 
        /// </summary>
        public void Awake()
        {
            this.aimTargetRigidbody = this.AimTarget.GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Updates the MeshRoot rotation point towards the mesh aim target.
        /// </summary>
        private void Update()
        {
            if (this.aimTargetRigidbody.IsSleeping() == false)
            {
                this.Mesh.rotation = Quaternion.Lerp(this.Mesh.rotation, this.AimTarget.rotation, this.LerpSpeed * Time.deltaTime);
            }
        }
    }
}