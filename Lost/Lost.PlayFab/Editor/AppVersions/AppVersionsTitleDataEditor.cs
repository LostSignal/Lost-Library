//-----------------------------------------------------------------------
// <copyright file="AppVersionsTitleDataEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using UnityEditor;

    [CustomEditor(typeof(AppVersionsTitleData))]
    public class AppVersionsTitleDataEditor : TitleDataEditor<AppVersionsTitleDataEditorWindow>
    {
        public override string WindowName => "App Verions";
    }

    public class AppVersionsTitleDataEditorWindow : TitleDataEditorWindow<AppVersionsData, AppVersionsTitleData>
    {
        protected override void DrawData(AppVersionsData item, SerializedObject serializedObject, SerializedProperty serializedProperty)
        {
            using (new EditorGrid.LabelWidthScope(200.0f))
            {
                EditorGUILayout.PropertyField(serializedProperty, true);
            }
        }
    }
}

#endif
