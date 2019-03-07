//-----------------------------------------------------------------------
// <copyright file="PlantGenerator.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlantGenerator
{
    using System;
    using UnityEngine;

    /// <summary>
    /// A game object that takes a bunch of parameters like materials and branch prefabs and generates
    /// a nice looking random plants.
    /// </summary>
    [CreateAssetMenu]
    public class PlantDefinition : ScriptableObject
    {
        /// <summary>
        /// The name of the Layer the plants use for their physics.
        /// </summary>
        public static readonly string LayerName = "InteractivePlants";

        /// <summary>
        /// All the branch parameters associated with this plant generator.
        /// </summary>
        public BranchGroupParameters[] GroupParameters = new BranchGroupParameters[1] { new BranchGroupParameters() };

        /// <summary>
        /// Returns the min and max branch count the generated plant can have.
        /// </summary>
        /// <param name="minBranchCount">Min branches possible.</param>
        /// <param name="maxBranchCount">Max branches possible.</param>
        public void GetMinMaxBranchCount(out int minBranchCount, out int maxBranchCount)
        {
            minBranchCount = 0;
            maxBranchCount = 0;

            if (this.GroupParameters != null)
            {
                for (int i = 0; i < this.GroupParameters.Length; i++)
                {
                    minBranchCount += this.GroupParameters[i].MinCount;
                    maxBranchCount += this.GroupParameters[i].MaxCount;
                }
            }
        }

        /// <summary>
        /// Parameters associated with creating a group of branches.
        /// </summary>
        [Serializable]
        public class BranchGroupParameters
        {
            /// <summary>
            /// Name of the group.
            /// </summary>
            public string Name;

            /// <summary>
            /// Collection of all the different branch prefabs to spawn from.
            /// </summary>
            public GameObject[] Variations = new GameObject[0];

            /// <summary>
            /// The minimum number of branches to spawn in this group.
            /// </summary>
            public int MinCount = 1;

            /// <summary>
            /// The maximum number of branches to spawn in this group.
            /// </summary>
            public int MaxCount = 5;

            /// <summary>
            /// Branches are evenly rotated, but this adds randomness +/- this value to the rotation.
            /// </summary>
            public float RandomRotationOffset = 10;

            /// <summary>
            /// the desired minimum space between each of the branches in this group.
            /// </summary>
            public int RotationalWidth = 10;

            /// <summary>
            /// The materials that a branch can be randomly assigned.  All renderers of the
            /// branch prefab will be set to the random material.
            /// </summary>
            public Material[] Materials;

            /// <summary>
            /// Varies the vertical height of the branch by +/- this offset.
            /// </summary>
            public float VerticalOffset = 0f;
        }
    }
}
