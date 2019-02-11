//-----------------------------------------------------------------------
// <copyright file="VersionedTitleDataEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System.Collections.Generic;
    using Lost.EditorGrid;
    using UnityEditor;
    using UnityEngine;

    public abstract class VersionedTitleDataEditor<EditorWindowType> : Editor
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

    public abstract class VersionedTitleDataEditorWindow<BaseType, TitleDataType> : EditorWindow, ITitleDataEditorWindow
        where BaseType : new()
        where TitleDataType : VersionedTitleData<BaseType>
    {
        private TitleDataType titleDataObject;
        private SerializedObject serializedObject;
        private SerializedProperty serializedProperty;

        private Vector2 scrollBarPosition = Vector2.zero;
        private int activeDataVersionIndex = -1;

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

            if (this.activeDataVersionIndex < 0 || this.activeDataVersionIndex >= this.titleDataObject.Versions.Count)
            {
                this.activeDataVersionIndex = this.titleDataObject.Versions.Count - 1;
            }


            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                using (new GUILayout.VerticalScope("box", GUILayout.ExpandWidth(true)))
                {
                    using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
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
                                json = JsonUtility.ToJson(this.titleDataObject.Data[this.activeDataVersionIndex]);
                            }
                            else
                            {
                                json = PF.SerializerPlugin.SerializeObject(this.titleDataObject.Data[this.activeDataVersionIndex]);
                            }

                            if (this.titleDataObject.CompressData)
                            {
                                json = LZString.CompressToBase64(json);
                            }

                            string titleDataKey = string.Format("{0}_{1}", this.titleDataObject.TitleDataKeyName, this.titleDataObject.Versions[this.activeDataVersionIndex]);
                            PlayFabEditorAdmin.SetTitleDataAndPrintErrorOrSuccess(titleDataKey, json);
                        }
                    }

                    using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(true)))
                    {
                        GUILayout.Label("Version", GUILayout.Width(60));
                        this.activeDataVersionIndex = EditorGUILayout.Popup(this.activeDataVersionIndex, this.GetVersions(), GUILayout.Width(80));

                        // Drawing Add Button
                        if (ButtonUtil.DrawAddButton(new Rect(new Vector2(155, 24), new Vector2(15, 15))))
                        {
                            this.serializedObject.FindProperty("data").arraySize++;
                            this.serializedObject.FindProperty("versions").arraySize++;
                            this.serializedObject.ApplyModifiedProperties();

                            // Making sure we select the newly created version
                            this.activeDataVersionIndex = this.titleDataObject.Versions.Count - 1;
                        }

                        // Drawing Delete Button
                        if (this.titleDataObject.Versions.Count > 1)
                        {
                            if (ButtonUtil.DrawDeleteButton(new Rect(new Vector2(175, 24), new Vector2(15, 15))))
                            {
                                this.serializedObject.FindProperty("data").DeleteArrayElementAtIndex(this.activeDataVersionIndex);
                                this.serializedObject.FindProperty("versions").DeleteArrayElementAtIndex(this.activeDataVersionIndex);
                                this.serializedObject.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }

                using (var horizontalScope = new GUILayout.HorizontalScope("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollBarPosition))
                    {
                        scrollBarPosition = scrollViewScope.scrollPosition;

                        EditorGUILayout.Space();

                        // Drawing the Version String
                        using (new LabelWidthScope(60))
                        {
                            string version = this.titleDataObject.Versions[this.activeDataVersionIndex];
                            string newVersion = EditorGUILayout.TextField("Version", version);

                            if (version != newVersion)
                            {
                                this.titleDataObject.Versions[this.activeDataVersionIndex] = newVersion;
                            }
                        }

                        // Drawing everything else (user defined)
                        this.DrawData(this.titleDataObject.Data[this.activeDataVersionIndex], this.serializedObject, this.serializedProperty.GetArrayElementAtIndex(this.activeDataVersionIndex));
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

        private string[] GetVersions()
        {
            List<string> versions = new List<string>();

            for (int i = 0; i < this.titleDataObject.Versions.Count; i++)
            {
                versions.Add(i + " - " + this.titleDataObject.Versions[i]);
            }

            return versions.ToArray();
        }

        protected abstract void DrawData(BaseType item, SerializedObject serializedObject, SerializedProperty serializedProperty);
    }
}

#endif
