//-----------------------------------------------------------------------
// <copyright file="FolderFinder.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public static class FolderFinder
    {
        [MenuItem("Lost/Logs/Show Editor Logs")]
        public static void OpenEditorLogs()
        {
            if (Platform.EditorPlatform == Platform.UnityEditorPlatform.Windows)
            {
                EditorUtility.RevealInFinder(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Unity", "Editor", "Editor.log"));
            }
            else if (Platform.EditorPlatform == Platform.UnityEditorPlatform.Mac)
            {
                EditorUtility.RevealInFinder("~/Library/Logs/Unity/Editor.log");
            }
            else
            {
                Debug.LogError("Unable to open Editor Log...  Unknown Platform.");
            }
        }

        [MenuItem("Lost/Logs/Show Player Log")]
        public static void OpenPlayerLog()
        {
            if (Platform.EditorPlatform == Platform.UnityEditorPlatform.Windows)
            {
                string appDataRootPath = Path.GetDirectoryName(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
                EditorUtility.RevealInFinder(Path.Combine(appDataRootPath, "LocalLow", PlayerSettings.companyName, PlayerSettings.productName, "Player.log"));
            }
            else if (Platform.EditorPlatform == Platform.UnityEditorPlatform.Mac)
            {
                EditorUtility.RevealInFinder("~/Library/Logs/Unity/Player.log");
            }
            else if (Platform.EditorPlatform == Platform.UnityEditorPlatform.Linux)
            {
                EditorUtility.RevealInFinder(Path.Combine("~", ".config", "unity3d", PlayerSettings.companyName, PlayerSettings.productName, "Player.log"));
            }
            else
            {
                Debug.LogError("Unable to open Editor Log...  Unknown Platform.");
            }
        }
    }
}
