//-----------------------------------------------------------------------
// <copyright file="ReferenceFinder.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public static class ReferenceFinder
    {
        [MenuItem("Lost/Tools/Find All Outside References")]
        private static void FindAllOutsideReferences()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (Directory.Exists(path) == false)
            {
                Debug.LogError("You must select a directory.");
                return;
            }

            HashSet<string> guidsToLookFor = GetMetaFileGuids(path);

            foreach (var dataFile in GetAllDataFiles(path))
            {
                HashSet<string> fileGuids = GetFileGuidReferences(dataFile);

                foreach (var fileGuid in fileGuids)
                {
                    if (guidsToLookFor.Contains(fileGuid))
                    {
                        Debug.LogFormat("Found Reference to Guid {0} ({1}) in {2}", fileGuid, AssetDatabase.GUIDToAssetPath(fileGuid), dataFile);
                    }
                }
            }
        }

        private static HashSet<string> GetMetaFileGuids(string path)
        {
            var result = new HashSet<string>();

            foreach (var metaFile in Directory.EnumerateFiles(path, "*.meta", SearchOption.AllDirectories))
            {
                string fileContents = File.ReadAllText(metaFile);
                int guidStartIndex = fileContents.IndexOf("guid: ") + 6;
                string guid = fileContents.Substring(guidStartIndex, 32);
                result.Add(guid);
            }

            return result;
        }

        private static HashSet<string> GetFileGuidReferences(string filePath)
        {
            var referencedGuids = new HashSet<string>();
            string fileContents = File.ReadAllText(filePath);

            int guidIndex = fileContents.IndexOf("guid: ");

            while (guidIndex != -1)
            {
                string guid = fileContents.Substring(guidIndex + 6, 32);
                referencedGuids.Add(guid);
                guidIndex = fileContents.IndexOf("guid: ", guidIndex + 1);
            }

            return referencedGuids;
        }

        private static IEnumerable<string> GetAllDataFiles(string pathToIgnore)
        {
            foreach (var file in Directory.EnumerateFiles(".", "*.*", SearchOption.AllDirectories))
            {
                var sanitizedFilePath = file.Substring(2).Replace("\\", "/");

                if (sanitizedFilePath.StartsWith(pathToIgnore))
                {
                    continue;
                }

                // Timeline or Signal?
                if (sanitizedFilePath.EndsWith(".unity") ||
                    sanitizedFilePath.EndsWith(".prefab") ||
                    sanitizedFilePath.EndsWith(".asset") ||
                    sanitizedFilePath.EndsWith(".controller") ||
                    sanitizedFilePath.EndsWith(".spriteatlas") ||
                    sanitizedFilePath.EndsWith(".anim"))
                {
                    yield return sanitizedFilePath;
                }
            }
        }
    }
}

/* Notes
--- !u!1 &795956156
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 795956157}
  - component: {fileID: 795956158}
  m_Layer: 0
  m_Name: AppStarter
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &795956157
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 795956156}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_Children: []
  m_Father: {fileID: 0}
  m_RootOrder: 1
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}

"--- !u!1 " -> GameObject
"--- !u!4 " -> Transform
"--- !u!224 " -> RectTransform
"  - component: {fileID: " -> Component

string lastFileId;

foreach (var line in File.ReadAllLines())
{
    if (line.StartsWith("--- !u"))
    {
        lastFileId = blah;
    }
    else if (line.StartsWith("GameObject:"))
    {
        // Create new GameObjectInfo
        // Set GameObjectInfo.FileId to lastFileId
        // Add GameObjectInfo to fileIdToGameObjectMap
        // Go through component list and add those ids to the componentIdToGameObjectMap
    }
    else if (line.StartsWith("m_Father") // Might want to make sure we're in a Transform or RectTransform
    {
        // Get the fileId
        // gameObjectId = componentIdToGameObjectMap[lastFileId];
        // parentFileId = componentIdToGameObjectMap[The parsed out "file:"];
        // fileIdToGameObjectMap[gameObjectId].ParentId = parentFileId;
    }
    else if (line.Contains("guid:"))
    {
        // Get the guid and see if it's in our guid map
        gameObjectId = componentIdToGameObjectMap[lastFileId];
        Debug.Log("Found Reference: " + GetFullName(gameObjectId));
    }
}

Dictionary<string, GameObject> fileIdToGameObjectMap;
Dictionary<string, string> componentIdToGameObjectMap;


GameObjectInfo
  FileId
  List<string> componentIds;
  Name
  ParentId

*/
