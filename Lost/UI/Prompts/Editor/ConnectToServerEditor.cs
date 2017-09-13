//-----------------------------------------------------------------------
// <copyright file="ConnectToServerEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ConnectToServer))]
    public class ConnectToServerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label(string.Empty);

            if (GUILayout.Button("Create Default Setup"))
            {
                ((ConnectToServer)this.target).SetupDefault();
            }
        }
    }
}
