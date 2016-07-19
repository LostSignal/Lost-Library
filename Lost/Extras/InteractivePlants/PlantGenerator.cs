//-----------------------------------------------------------------------
// <copyright file="PlantGenerator.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlantGenerator
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A game object that takes a bunch of parameters like materials and branch prefabs and generates 
    /// a nice looking random plants.
    /// </summary>
    public class PlantGenerator : MonoBehaviour
    {
        /// <summary>
        /// The name of the Layer the plants use for their physics.
        /// </summary>
        private static readonly string LayerName = "InteractivePlants";

        /// <summary>
        /// Stores the layer id the branches will live on.
        /// </summary>
        private static int layerId = -1;
        
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

        /// <summary>
        /// Set this if you always want the plant to look different when you run the game.
        /// </summary>
        public bool RandomizeOnStart = false;

        /// <summary>
        /// All the branch parameters associated with this plant generator.
        /// </summary>
        public BranchGroupParameters[] GroupParameters = new BranchGroupParameters[1] { new BranchGroupParameters() };
        
        /// <summary>
        /// The random seed used to generate the looks of this generated plant.
        /// </summary>
        [HideInInspector]
        public int Seed;

        /// <summary>
        /// Flag specifying whether or not the plant has been generated yet.
        /// </summary>
        [HideInInspector]
        public bool IsGenerated = false;

        /// <summary>
        /// Used to keep track if this object created it's children itself and thus needs to 
        /// destroy them when this object is destroyed.
        /// </summary>
        private bool destroyChildren = false;

        /// <summary>
        /// Makes sure the plant is generated when enabled.
        /// </summary>
        public void Awake()
        {
            // making sure group parameters are never null
            if (this.GroupParameters == null)
            {
                this.GroupParameters = new PlantGenerator.BranchGroupParameters[0];
            }

            if (this.RandomizeOnStart)
            {
                this.GenerateNewRandomSeed();
            }
            else
            {
                this.Generate();
            }
        }

        /// <summary>
        /// Creates a new seed value and regenerates the plant.
        /// </summary>
        public void GenerateNewRandomSeed()
        {
            this.ClearChildren();
            this.Seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            this.Generate();
        }

        /// <summary>
        /// Destroys all child branches and resets the plant so it can be regenerated.
        /// </summary>
        public void ClearChildren()
        {
            PlantGeneratorBranch[] branches = this.GetComponentsInChildren<PlantGeneratorBranch>();

            if (branches != null)
            {
                for (int i = 0; i < branches.Length; i++)
                {
                    DestroyImmediate(branches[i].gameObject);
                }
            }

            this.IsGenerated = false;
        }

        /// <summary>
        /// Clears the children and regenerates them.  Useful if you already generated 
        /// the plant but changed the branch prefab, or plant parameters.
        /// </summary>
        public void Refresh()
        {
            this.ClearChildren();
            this.Generate();
        }

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
        /// Uses the the plant parameters to generates a new plant.
        /// </summary>
        private void Generate()
        {
            // making sure it hasn't already been generated
            if (this.IsGenerated)
            {
                return;
            }

            // making sure layerId gets initialized
            if (layerId == -1)
            {
                layerId = LayerMask.NameToLayer(LayerName);

                if (layerId == -1)
                {
                    string message = string.Format("PlantGenerator couldn't find layer \"{0}\".  To stop the plants from jiggling, go to the menu \"Edit -> Project Settings -> Tags and Layers\" and make sure the layer \"{0}\" exists.", LayerName);
                    Debug.LogError(message, this.gameObject);
                }
                else
                {
                    Physics.IgnoreLayerCollision(layerId, layerId);
                }
            }

            System.Random rand = new System.Random(this.Seed);
            SuitableRotationSystem suitableRotationSystem = new SuitableRotationSystem(rand);

            if (this.GroupParameters.Length == 0)
            {
                Debug.LogError("Plant doesn't have any Group Parameters specified", this.gameObject);
                return;
            }

            for (int i = 0; i < this.GroupParameters.Length; i++)
            {
                BranchGroupParameters groupParameters = this.GroupParameters[i];

                if (groupParameters.MinCount < 0 || groupParameters.MaxCount < 0)
                {
                    Debug.LogError("Plant has invalid Min/Max count.  Min and Max must be greater than or equal to 0", this.gameObject);
                    return;
                }

                if (groupParameters.MinCount > groupParameters.MaxCount)
                {
                    Debug.LogError("Plant has invalid Min/Max count.  Min must be less than or equal to Max", this.gameObject);
                    return;
                }

                if (groupParameters.MinCount == 0 && groupParameters.MaxCount == 0)
                {
                    Debug.LogError("Plant have Min/Max both equal to 0.  That group will never be created.", this.gameObject);
                    return;
                }

                int branchTypeCount = Mathf.RoundToInt(RandomUtil.IntRangeInclusive(rand, groupParameters.MinCount, groupParameters.MaxCount));

                for (int j = 0; j < branchTypeCount; j++)
                {
                    if (groupParameters.Variations == null || groupParameters.Variations.Length == 0)
                    {
                        Debug.LogError("Plant has invalid number of Variations.  Must have at least 1.", this.gameObject);
                        return;
                    }
                    
                    // creating the new branch
                    int variationIndex = rand.Next(groupParameters.Variations.Length);
                    GameObject prototypePlant = groupParameters.Variations[variationIndex];

                    if (prototypePlant == null)
                    {
                        Debug.LogError(string.Format("Plant has null GameObject in Variation Element {0}.", variationIndex), this.gameObject);
                        return;
                    }

                    GameObject newBranch = (GameObject)Instantiate(prototypePlant);

                    // setting up the base rotation
                    float baseRotation = 360 * ((float)j / (float)branchTypeCount);
                    baseRotation = suitableRotationSystem.GetSuitibleRotation(baseRotation);
                    suitableRotationSystem.AddDeadSpot(baseRotation, groupParameters.RotationalWidth);

                    // generating height offset
                    float height = RandomUtil.FloatRangeExclusive(rand, -groupParameters.VerticalOffset, groupParameters.VerticalOffset);

                    // setting the new plants transform values
                    newBranch.transform.parent = transform;
                    newBranch.transform.localPosition = new Vector3(0, height, 0);
                    newBranch.transform.localEulerAngles = new Vector3(0, baseRotation, 0);
                    newBranch.transform.localScale = Vector3.one;

                    // setting the layer
                    if (layerId != -1)
                    {
                        newBranch.layer = layerId;
                    }

                    // setting the branch rotation (off of the base)
                    PlantGeneratorBranch plantBranch = newBranch.GetComponent<PlantGeneratorBranch>();
                    float addedRotation = RandomUtil.FloatRangeExclusive(rand, groupParameters.RandomRotationOffset, -groupParameters.RandomRotationOffset);
                    plantBranch.transform.localEulerAngles += new Vector3(0, addedRotation, 0);

                    // setting a random material on all renderers of this branch
                    if (groupParameters.Materials != null && groupParameters.Materials.Length > 0)
                    {
                        int index = rand.Next(groupParameters.Materials.Length);
                        Material newMaterial = groupParameters.Materials[index];

                        if (newMaterial != null)
                        {
                            var renderers = plantBranch.GetComponentsInChildren<Renderer>();

                            for (int r = 0; r < renderers.Length; r++)
                            {
                                renderers[r].sharedMaterial = newMaterial;
                            }
                        }
                    }
                }

                // if we got here then everything generated properly
                this.IsGenerated = true;
                this.destroyChildren = true;
            }
        }

        /// <summary>
        /// Called when the engine destroys this object.  Makes sure that all 
        /// children that this object instance are destroyed as well.
        /// </summary>
        private void OnDestroy()
        {
            if (this.destroyChildren)
            {
                this.ClearChildren();
            }
        }
    }
}
