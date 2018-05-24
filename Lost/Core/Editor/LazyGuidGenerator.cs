//-----------------------------------------------------------------------
// <copyright file="LazyGuidGenerator.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class LazyGuidGenerator : MonoBehaviour
    {
        private const string assetGuidString = "assetGuid:";

        [MenuItem("Lost/Update Lazy GUIDs")]
        public static void FindLazyGuids()
        {
            var guids = new HashSet<string>();

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(".", "*.asset", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(".", "*.prefab", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(".", "*.unity", SearchOption.AllDirectories));

            foreach (var file in files)
            {
                foreach (var line in File.ReadAllLines(file))
                {
                    var trimmedLine = line.Trim();

                    if (trimmedLine.StartsWith(assetGuidString))
                    {
                        var guid = trimmedLine.Replace(assetGuidString, string.Empty).Trim();
                        if (string.IsNullOrEmpty(guid) == false)
                        {
                            guids.Add(guid);
                        }
                    }
                }
            }

            Debug.LogFormat("Found {0} GUIDs", guids.Count);

            foreach (var guid in guids)
            {
                Debug.Log("GUID: " + guid);
            }

            // TODO [bgish]:  Sort and save these out to a scriptable object.  Also print out Path and AssetBundle name
        }
    }
}
