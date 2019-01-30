//-----------------------------------------------------------------------
// <copyright file="LabelWidthScope.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.EditorGrid
{
    using System;
    using UnityEditor;

    public class LabelWidthScope : IDisposable
    {
        private readonly float originalWidth;

        public LabelWidthScope(float labelWidth)
        {
            this.originalWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        void IDisposable.Dispose()
        {
            EditorGUIUtility.labelWidth = this.originalWidth;
        }
    }
}
