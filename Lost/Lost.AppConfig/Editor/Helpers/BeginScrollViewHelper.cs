//-----------------------------------------------------------------------
// <copyright file="BeginScrollViewHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class BeginScrollViewHelper : IDisposable
    {
        public BeginScrollViewHelper(Vector2 scrollPosition, out Vector2 newScrollPosition)
        {
            newScrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        }

        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }
    }
}
