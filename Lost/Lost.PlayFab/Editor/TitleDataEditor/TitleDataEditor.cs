//-----------------------------------------------------------------------
// <copyright file="TitleDataEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEditor;
    using UnityEngine;

    public abstract class TitleDataEditor<EditorWindowType> : Editor
        where EditorWindowType : EditorWindow, ITitleDataEditorWindow
    {
        public abstract string WindowName { get; }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor Window", GUILayout.ExpandWidth(true)))
            {
                var window = EditorWindow.GetWindow<EditorWindowType>(this.WindowName);
                window.SetData(this.target);
                window.Show();
            }
        }
    }

    public abstract class TitleDataEditorWindow<BaseType, TitleDataType> : EditorWindow, ITitleDataEditorWindow
            where BaseType : new()
            where TitleDataType : TitleData<BaseType>
    {
        private TitleDataType titleDataObject;
        private SerializedObject serializedObject;
        private SerializedProperty serializedProperty;
        private Vector2 scrollBarPosition = Vector2.zero;

        public void SetData(UnityEngine.Object obj)
        {
            this.titleDataObject = (obj as TitleDataType);
            this.serializedObject = new SerializedObject(this.titleDataObject);
            this.serializedProperty = this.serializedObject.FindProperty("data");
        }

        public virtual void OnGUI()
        {
            if (this.titleDataObject == null || this.serializedObject == null || this.serializedProperty == null)
            {
                GUILayout.Label("Please Re-open the editor.");
                return;
            }

            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                using (new GUILayout.HorizontalScope("box", GUILayout.ExpandWidth(true)))
                {
                    // TitleDataKeyName
                    GUILayout.Label("TitleData Key", GUILayout.Width(80));
                    this.titleDataObject.TitleDataKeyName = GUILayout.TextField(this.titleDataObject.TitleDataKeyName, GUILayout.Width(200));
                    GUILayout.Space(10);

                    // SerializeWithUnity
                    this.titleDataObject.SerializeWithUnity = GUILayout.Toggle(this.titleDataObject.SerializeWithUnity, "Serialize With Unity", GUILayout.Width(140));

                    // CompressData
                    this.titleDataObject.CompressData = GUILayout.Toggle(this.titleDataObject.CompressData, "Compress Data", GUILayout.Width(150));

                    GUILayout.FlexibleSpace();

                    // Upload Button
                    if (GUILayout.Button("Upload", GUILayout.Width(100)))
                    {
                        string json = string.Empty;

                        if (this.titleDataObject.SerializeWithUnity)
                        {
                            json = JsonUtility.ToJson(this.titleDataObject.Data);
                        }
                        else
                        {
                            json = PF.SerializerPlugin.SerializeObject(this.titleDataObject.Data);
                        }

                        if (this.titleDataObject.CompressData)
                        {
                            json = LZString.CompressToBase64(json);
                        }

                        PlayFabEditorAdmin.SetTitleDataAndPrintErrorOrSuccess(this.titleDataObject.TitleDataKeyName, json);
                    }
                }

                using (var horizontalScope = new GUILayout.HorizontalScope("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollBarPosition))
                    {
                        scrollBarPosition = scrollViewScope.scrollPosition;

                        this.DrawData(this.titleDataObject.Data, this.serializedObject, this.serializedProperty);
                    }
                }

                // Making sure we mark the data dirty if it changed
                if (changeCheck.changed)
                {
                    EditorUtility.SetDirty(this.serializedObject.targetObject);
                    this.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        protected abstract void DrawData(BaseType item, SerializedObject serializedObject, SerializedProperty serializedProperty);
    }
}
