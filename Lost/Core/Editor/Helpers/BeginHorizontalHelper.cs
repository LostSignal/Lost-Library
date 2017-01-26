//-----------------------------------------------------------------------
// <copyright file="BeginHorizontalHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEditor;
    using UnityEngine;
    
    public class BeginHorizontalHelper : IDisposable
    {
        public BeginHorizontalHelper()
        {
            EditorGUILayout.BeginHorizontal();
        }
        
        public BeginHorizontalHelper(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
        }
        
        public BeginHorizontalHelper(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
        }
        
        public BeginHorizontalHelper(out Rect rect)
        {
            rect = EditorGUILayout.BeginHorizontal();
        }

        public BeginHorizontalHelper(out Rect rect, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginHorizontal(options);
        }
        
        public BeginHorizontalHelper(out Rect rect, GUIStyle style, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginHorizontal(style, options);
        }
        
        public void Dispose()
        {
            EditorGUILayout.EndHorizontal();
        }
    }
}
