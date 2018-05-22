//-----------------------------------------------------------------------
// <copyright file="BuildLightingHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class BuildLightingHelper : EditorWindow
    {
        private Dictionary<string, bool> scenes;

        [MenuItem("Lost/Tools/Build Lighting Helper")]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow<BuildLightingHelper>(false, "Build Lighting");
        }

        private void OnGUI()
        {
            if (this.scenes == null)
            {
                this.RefreshSceneList();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scenes To Build Lighting");

            foreach (string key in this.scenes.Keys.OrderBy(x => x))
            {
                this.scenes[key] = this.DrawCheckbox(key, this.scenes[key]);
            }

            using (new BeginHorizontalHelper())
            {
                if (GUILayout.Button("Build Lighting"))
                {
                    var scenes = this.scenes.Select(x => x.Key).Where(x => this.scenes[x]).ToArray();
                    Lightmapping.BakeMultipleScenes(scenes);
                }
            }

            using (new BeginHorizontalHelper())
            {
                if (GUILayout.Button("Refresh Scene List"))
                {
                    this.RefreshSceneList();
                }
            }
        }

        private void RefreshSceneList()
        {
            this.scenes = new Dictionary<string, bool>();

            var currentDir = Directory.GetCurrentDirectory();

            foreach (var file in Directory.GetFiles(currentDir, "*.unity", SearchOption.AllDirectories))
            {
                var assetPath = file.Substring(currentDir.Length + 1).Replace('\\', '/');
                this.scenes.Add(assetPath, false);
            }
        }

        private bool DrawCheckbox(string label, bool value)
        {
            using (new BeginHorizontalHelper())
            {
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(10));
                bool newValue = EditorGUILayout.Toggle(value, GUILayout.Width(20));
                EditorGUILayout.LabelField(label);
                return newValue;
            }
        }
    }
}
