//-----------------------------------------------------------------------
// <copyright file="BoxHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEditor;
    using UnityEngine;

    // TODO BoxHelper and FoldoutHelper are nearly identicle, probably should combine somehow
    public class BoxHelper : IDisposable
    {
        private static GUIStyle titleGuiStyle = null;

        public BoxHelper(float width)
        {
            Rect position = EditorGUILayout.BeginVertical("box", GUILayout.Width(width));
            this.DrawTitle(position, string.Empty);
        }

        public BoxHelper(float width, string title)
        {
            Rect position = EditorGUILayout.BeginVertical("box", GUILayout.Width(width));
            this.DrawTitle(position, title);
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
        }

        private void DrawTitle(Rect position, string title)
        {
            if (titleGuiStyle == null)
            {
                titleGuiStyle = new GUIStyle(GUI.skin.label);
                titleGuiStyle.alignment = TextAnchor.LowerCenter;
                titleGuiStyle.stretchWidth = true;
                titleGuiStyle.border = new RectOffset();

                if (LostEditorUtil.IsProTheme())
                {
                    titleGuiStyle.normal.textColor = Color.white;
                }
                else
                {
                    titleGuiStyle.normal.textColor = Color.black;
                }
            }

            if (string.IsNullOrEmpty(title) == false)
            {
                EditorGUILayout.LabelField(title, titleGuiStyle);
            }
            else
            {
                EditorGUILayout.LabelField(string.Empty, titleGuiStyle);
            }
        }
    }
}
