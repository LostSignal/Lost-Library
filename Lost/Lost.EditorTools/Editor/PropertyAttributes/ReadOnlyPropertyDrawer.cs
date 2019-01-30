//-----------------------------------------------------------------------
// <copyright file="ReadOnlyPropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;

            try
            {
                EditorGUI.PropertyField(position, property, label);
            }
            finally
            {
                GUI.enabled = true;
            }
        }
    }
}
