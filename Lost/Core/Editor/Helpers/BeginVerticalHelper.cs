//-----------------------------------------------------------------------
// <copyright file="BeginVerticalHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class BeginVerticalHelper : IDisposable
    {
        public BeginVerticalHelper()
        {
            EditorGUILayout.BeginVertical();
        }

        public BeginVerticalHelper(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
        }

        public BeginVerticalHelper(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
        }

        public BeginVerticalHelper(out Rect rect)
        {
            rect = EditorGUILayout.BeginVertical();
        }

        public BeginVerticalHelper(out Rect rect, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginVertical(options);
        }

        public BeginVerticalHelper(out Rect rect, GUIStyle style, params GUILayoutOption[] options)
        {
            rect = EditorGUILayout.BeginVertical(style, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }
    }
}
