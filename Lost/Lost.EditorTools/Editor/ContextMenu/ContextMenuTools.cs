//-----------------------------------------------------------------------
// <copyright file="ContextMenuTools.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEditor.VersionControl;

    public static class ContextMenuTools
    {
        [MenuItem("Assets/Lost/Copy GUID")]
        public static void PrintGuid()
        {
            if (Selection.assetGUIDs.Length == 1)
            {
                EditorGUIUtility.systemCopyBuffer = Selection.assetGUIDs[0];
            }
        }

        [MenuItem("Assets/Lost/Regenerate All Folder Guids")]
        public static void DeleteAllFolderMetaFiles()
        {
            if (Selection.activeObject == null)
            {
                UnityEngine.Debug.LogError("You must select a directory for this option to work.");
                return;
            }

            string rootDirectoryAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

            if (Directory.Exists(rootDirectoryAssetPath) == false)
            {
                UnityEngine.Debug.LogError("You must select a directory for this option to work.");
                return;
            }

            foreach (var directory in Directory.GetDirectories(rootDirectoryAssetPath, "*", SearchOption.AllDirectories))
            {
                var assetPath = directory.Replace("\\", "/");
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
                var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
                var newGuid = Guid.NewGuid().ToString("N");

                string metaFilePath = assetPath + ".meta";

                if (File.Exists(metaFilePath))
                {
                    // NOTE [bgish]: Not sure why, but this only seems to get half the folders
                    // Provider.Checkout(asset, CheckoutMode.Both).Wait();

                    File.WriteAllText(metaFilePath, File.ReadAllText(metaFilePath).Replace(assetGuid, newGuid));
                }
            }
        }
    }
}
