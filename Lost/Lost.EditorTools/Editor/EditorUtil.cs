//-----------------------------------------------------------------------
// <copyright file="EditorUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    public class EditorUtil : MonoBehaviour
    {
        public static T GetAssetByGuid<T>(string guid) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}
