//-----------------------------------------------------------------------
// <copyright file="PurchaseItemEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(PurchaseItem))]
    public class PurchaseItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label(string.Empty);

            if (GUILayout.Button("Create Default Setup"))
            {
                ((PurchaseItem)this.target).SetupDefault();
            }
        }
    }
}
