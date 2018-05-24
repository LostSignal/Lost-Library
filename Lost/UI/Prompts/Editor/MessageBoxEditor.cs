//-----------------------------------------------------------------------
// <copyright file="MessageBoxEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(MessageBox))]
    public class MessageBoxEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label(string.Empty);

            if (GUILayout.Button("Create Default Setup"))
            {
                ((MessageBox)this.target).SetupDefault();
            }
        }
    }
}
