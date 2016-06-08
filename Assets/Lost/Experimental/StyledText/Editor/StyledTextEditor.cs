//-----------------------------------------------------------------------
// <copyright file="StyledTextEditor.cs" company="Lost Signal LLC">
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
    [CustomEditor(typeof(StyledText))]
    public class StyledTextEditor : Editor
    {
        [MenuItem("GameObject/UI/Styled Text")]
        static void CreateStyledText(MenuCommand command)
        {
            UIUtil.InsureCanvasExists();

            // finding the canvas
            var canvas = Object.FindObjectOfType<Canvas>();
            
            // creating the styled text object
            GameObject go = new GameObject("StyledText");
            go.AddComponent<RectTransform>().sizeDelta = new Vector2(200f, 50f);
            go.AddComponent<StyledText>();

            // setting the parent
            var parent = command.context as GameObject ?? canvas.gameObject;
            GameObjectUtility.SetParentAndAlign(go, parent);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUI.changed)
            {
                var styledText = this.target as StyledText;
                styledText.UpdateFontStyleValues();
                styledText.UpdateTextValue();
            }
        }
    }
}
