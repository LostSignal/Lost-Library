//-----------------------------------------------------------------------
// <copyright file="LocTextPropertyDrawer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;

    [CustomPropertyDrawer(typeof(LocText))]
    public class LocTextPropertyDrawer : PropertyDrawer
    {
        // Here you must define the height of your property drawer. Called by Unity.
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return base.GetPropertyHeight(prop, label);
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            
            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            float locTableRectWidth = position.width - 55;
            Rect locTableRect = new Rect(position.x, position.y, locTableRectWidth, 16);
            Rect locTextIdRect = new Rect(position.x + locTableRectWidth + 5, position.y, 50, 16);
            
            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            var locTableProp = property.FindPropertyRelative("localizationTable");
            var locTextIdProp = property.FindPropertyRelative("localizationTextId");

            EditorGUI.PropertyField(locTableRect, locTableProp, GUIContent.none);
            EditorGUI.PropertyField(locTextIdRect, locTextIdProp, GUIContent.none);

            //TODO not sure if we want a drop down
            //var locTable = locTableProp.objectReferenceValue as LocTable;
            //
            //if (locTable != null)
            //{
            //    var strings = new List<string>(locTable.Count);
            //    
            //    for (int i = 0; i < locTable.Count; i++)
            //    {
            //        strings.Add(locTable[i].Text);
            //    }
            //    
            //    position.y += 16;
            //    EditorGUI.Popup(position, 0, strings.ToArray());
            //}
            
            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
                    
            EditorGUI.EndProperty();
        }
    }
}