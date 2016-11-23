//-----------------------------------------------------------------------
// <copyright file="LazyAssetPropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public class LazyAssetPropertyDrawer<T> : PropertyDrawer where T : UnityEngine.Object
    {
        private static readonly Dictionary<string, T> objectCache = new Dictionary<string, T>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assetGuid = property.FindPropertyRelative("assetGuid");
            
            var currentValue = this.GetAsset(assetGuid.stringValue);
            var newValue = EditorGUI.ObjectField(position, label, currentValue, typeof(T), false);
            
            if (currentValue != newValue)
            {
                assetGuid.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newValue));
                
                SerializedProperty assetPath = property.FindPropertyRelative("assetPath");
                assetPath.stringValue = AssetDatabase.GUIDToAssetPath(assetGuid.stringValue);
                
                SerializedProperty assetAssetBundleName = property.FindPropertyRelative("assetBundleName");
                assetAssetBundleName.stringValue = LazyAsset<Object>.FindAssetBundleName(assetPath.stringValue);
            }
        }

        private T GetAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            T obj;
            if (objectCache.TryGetValue(guid, out obj) == false)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
                objectCache.Add(guid, obj);
            }

            return obj;
        }
    }
}
