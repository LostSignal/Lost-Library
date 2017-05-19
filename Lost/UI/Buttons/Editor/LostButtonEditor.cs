//-----------------------------------------------------------------------
// <copyright file="LostButtonEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEditor.UI;

    [CustomEditor(typeof(LostButton))]
    public class LostButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UIActionsEditor.DrawUIActionGUI(this.target);
        }
    }
}
