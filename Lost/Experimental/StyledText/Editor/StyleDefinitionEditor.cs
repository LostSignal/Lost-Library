//-----------------------------------------------------------------------
// <copyright file="StyleDefinitionEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    [CustomEditor(typeof(StyleDefinition))]
    public class StyleDefinitionEditor : Editor
    {
        /// <summary>
        /// 
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUI.changed)
            {
                foreach (StyledText lostText in ObjectTracker.GetObjects<StyledText>())
                {
                    lostText.UpdateFontStyleValues();
                }
            }
        }
    }
}
