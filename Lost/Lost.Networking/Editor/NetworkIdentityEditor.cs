//-----------------------------------------------------------------------
// <copyright file="NetworkIdentityEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.Networking
{
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(NetworkIdentity))]
    public class NetworkIdentityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate New Id"))
            {
                if (this.target != null)
                {
                    this.GenerateNewId(this.target);
                }

                if (this.targets != null)
                {
                    for (int i = 0; i < this.targets.Length; i++)
                    {
                        this.GenerateNewId(this.targets[i]);
                    }
                }
            }
        }

        private void GenerateNewId(UnityEngine.Object target)
        {
            var serializedObject = new SerializedObject(target);
            var networkId = serializedObject.FindProperty("networkId");
            networkId.longValue = NetworkIdentity.NewId();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
