//-----------------------------------------------------------------------
// <copyright file="LazyScenePropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_UNITY_ADDRESSABLES

namespace Lost
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(LazyScene), true)]
    public class LazyScenePropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, UnityEngine.Object> objectCache = new Dictionary<string, UnityEngine.Object>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty assetGuid = property.FindPropertyRelative("assetGuid");

            var currentValue = this.GetAsset(assetGuid.stringValue);

            var newValue = EditorGUI.ObjectField(position, label, currentValue, typeof(SceneAsset), false);

            if (currentValue != newValue)
            {
                assetGuid.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newValue));
            }
        }

        private UnityEngine.Object GetAsset(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            UnityEngine.Object obj;
            if (objectCache.TryGetValue(guid, out obj) == false)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));
                objectCache.Add(guid, obj);
            }

            return obj;
        }
    }
}

#endif
