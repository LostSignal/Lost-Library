//-----------------------------------------------------------------------
// <copyright file="DictionaryEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Dictionary))]
    public class DictionaryEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var dictionary = this.target as Dictionary;

            if (dictionary == null)
            {
                return;
            }
            
            EditorGUILayout.LabelField("Words in Dictionary", dictionary.WordCount.ToString());

            if (GUILayout.Button("Import Dictionary Text File"))
            {
                var path = EditorUtility.OpenFilePanel("Dictionary Text File", string.Empty, "txt");

                if (path != null && path.Length > 0)
                {
                    List<string> words = new List<string>(300000);

                    foreach (var line in File.ReadAllLines(path))
                    {
                        if (string.IsNullOrEmpty(line) == false)
                        {
                            words.Add(line.ToLower());
                        }
                    }

                    words.Sort();

                    dictionary.SetWords(words.ToArray());
                    EditorUtility.SetDirty(dictionary);
                }
            }
        }
    }
}
