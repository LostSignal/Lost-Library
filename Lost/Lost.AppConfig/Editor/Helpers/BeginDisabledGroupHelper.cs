//-----------------------------------------------------------------------
// <copyright file="BeginDisabledGroupHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System;
    using UnityEditor;

    public class BeginDisabledGroupHelper : IDisposable
    {
        public BeginDisabledGroupHelper(bool disabled)
        {
            EditorGUI.BeginDisabledGroup(disabled);
        }

        public void Dispose()
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}
