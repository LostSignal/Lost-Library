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
    [CustomEditor(typeof(PlantDefinition))]
    public class PlantDefinitionEditor : Editor
    {
        /// <summary>
        /// Used to draw the inspector GUI for the plant generator object.
        /// </summary>
        public override void OnInspectorGUI()
        {
            PlantDefinition plantDefinition = target as PlantDefinition;

            int minBranchCount;
            int maxBranchCount;
            plantDefinition.GetMinMaxBranchCount(out minBranchCount, out maxBranchCount);

            GUILayout.Label("Total Branches: " + minBranchCount + " - " + maxBranchCount);
            EditorGUILayout.Separator();

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("NAME", GUILayout.Width(100));
                GUILayout.Label("COUNT", GUILayout.Width(60));
                GUILayout.Label("%", GUILayout.Width(50));
            }

            for (int i = 0; i < plantDefinition.GroupParameters.Length; i++)
            {
                PlantDefinition.BranchGroupParameters branchParameters = plantDefinition.GroupParameters[i];

                using (new GUILayout.HorizontalScope())
                {
                    branchParameters.Name = GUILayout.TextField(branchParameters.Name ?? "", GUILayout.Width(100));

                    branchParameters.MinCount = this.IntegerTextField(branchParameters.MinCount);
                    GUILayout.Label("-", GUILayout.Width(7));
                    branchParameters.MaxCount = this.IntegerTextField(branchParameters.MaxCount);

                    int minPercent = Mathf.RoundToInt(100 * ((float)branchParameters.MinCount) / ((float)maxBranchCount));
                    int maxPercent = Mathf.RoundToInt(100 * ((float)branchParameters.MaxCount) / ((float)minBranchCount));
                    GUILayout.Label(minPercent + "% - " + maxPercent + "%", GUILayout.Width(50));
                }
            }

            EditorGUILayout.Separator();

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
