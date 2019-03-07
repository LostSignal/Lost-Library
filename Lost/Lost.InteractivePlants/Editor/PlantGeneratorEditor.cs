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
            base.OnInspectorGUI();

            EditorGUILayout.Space();

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

            EditorGUILayout.Space();
        }
    }
}
