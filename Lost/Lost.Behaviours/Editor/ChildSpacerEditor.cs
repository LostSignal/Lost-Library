//-----------------------------------------------------------------------
// <copyright file="ChildSpacerEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ChildSpacer))]
    public class ChildSpacerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Space Children"))
            {
                var childSpacer = this.target as ChildSpacer;

                for (int i = 0; i < childSpacer.transform.childCount; i++)
                {
                    childSpacer.transform.GetChild(i).localPosition = childSpacer.StartPosition + (childSpacer.Spacing * i);
                }
            }
        }
    }
}
