//-----------------------------------------------------------------------
// <copyright file="PlantGeneratorEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.PlantGenerator
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Simple inspector editor for the plant generator MonoBehaviour.
    /// </summary>
    [CanEditMultipleObjects()]
    [CustomEditor(typeof(PlantGenerator))]
    public class PlantGeneratorEditor : Editor
    {
        /// <summary>
        /// Used to draw the inspector GUI for the plant generator object.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Generate New Seed"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    var plant = targets[i] as PlantGenerator;
                    plant.GenerateNewRandomSeed();
                    EditorUtility.SetDirty(plant);
                }
            }

            if (GUILayout.Button("Refresh"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    var plant = targets[i] as PlantGenerator;
                    plant.Refresh();
                    EditorUtility.SetDirty(plant);
                }
            }

            if (GUILayout.Button("Clear"))
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    var plant = targets[i] as PlantGenerator;
                    plant.ClearChildren();
                    EditorUtility.SetDirty(plant);
                }
            }

            if (targets.Length == 1)
            {
                PlantGenerator plant = target as PlantGenerator;

                int minBranchCount;
                int maxBranchCount;
                plant.GetMinMaxBranchCount(out minBranchCount, out maxBranchCount);

                GUILayout.Label("Total Branches: " + minBranchCount + " - " + maxBranchCount);
                EditorGUILayout.Separator();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("NAME", GUILayout.Width(100));
                    GUILayout.Label("COUNT", GUILayout.Width(60));
                    GUILayout.Label("%", GUILayout.Width(50));
                }
                GUILayout.EndHorizontal();

                for (int i = 0; i < plant.GroupParameters.Length; i++)
                {
                    PlantGenerator.BranchGroupParameters branchParameters = plant.GroupParameters[i];

                    GUILayout.BeginHorizontal();
                    {
                        branchParameters.Name = GUILayout.TextField(branchParameters.Name ?? "", GUILayout.Width(100));

                        branchParameters.MinCount = this.IntegerTextField(branchParameters.MinCount);
                        GUILayout.Label("-", GUILayout.Width(7));
                        branchParameters.MaxCount = this.IntegerTextField(branchParameters.MaxCount);

                        int minPercent = Mathf.RoundToInt(100 * ((float)branchParameters.MinCount) / ((float)maxBranchCount));
                        int maxPercent = Mathf.RoundToInt(100 * ((float)branchParameters.MaxCount) / ((float)minBranchCount));
                        GUILayout.Label(minPercent + "% - " + maxPercent + "%", GUILayout.Width(50));
                    }
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.Separator();
            }

            base.OnInspectorGUI();
        }

        /// <summary>
        /// Simple text field wrapper making sure only integers values are inputted.
        /// </summary>
        /// <param name="value">The current integer value.</param>
        /// <returns>The integer the user inputted.</returns>
        private int IntegerTextField(int value)
        {
            int returnValue = value;
            string enteredValue = GUILayout. TextField(value.ToString(), GUILayout.Width(20));
            int newValue;

            if (int.TryParse(enteredValue, out newValue))
            {
                returnValue = newValue;
            }

            return returnValue;
        }
    }
}