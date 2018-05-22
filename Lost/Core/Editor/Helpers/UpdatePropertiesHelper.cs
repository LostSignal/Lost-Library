//-----------------------------------------------------------------------
// <copyright file="UpdatePropertiesHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEditor;

    public class UpdatePropertiesHelper : IDisposable
    {
        private SerializedObject serializedObject;
        private bool changed = false;

        public UpdatePropertiesHelper(SerializedObject serializedObj)
        {
            this.serializedObject = serializedObj;
            this.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
        }

        public bool Changed
        {
            get { return this.changed; }
        }

        public void Dispose()
        {
            if (EditorGUI.EndChangeCheck())
            {
                this.changed = true;
                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
