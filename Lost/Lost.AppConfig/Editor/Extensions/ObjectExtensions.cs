//-----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using UnityEditor;

    public static class ObjectExtensions
    {
        public static string GetPath(this UnityEngine.Object obj)
        {
            return AssetDatabase.GetAssetPath(obj.GetInstanceID());
        }

        public static string GetGuid(this UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string path = obj.GetPath();

            if (string.IsNullOrEmpty(path) == false)
            {
                return AssetDatabase.AssetPathToGUID(path);
            }

            return null;
        }
    }
}
