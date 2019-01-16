//-----------------------------------------------------------------------
// <copyright file="BuildConfigSettings.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    public abstract class AppConfigSettings : ScriptableObject
    {
        public abstract string DisplayName { get; }
        public abstract bool IsInline { get; }

        public virtual void GetRuntimeConfigSettings(AppConfig appConfig, Dictionary<string, string> runtimeConfigSettings)
        {
        }

        public virtual void OnPreproccessBuild(AppConfig appConfig, BuildReport buildReport)
        {
        }

        public virtual void OnPostprocessBuild(AppConfig appConfig, BuildReport buildReport)
        {
        }

        public virtual void InitializeOnLoad(AppConfig appConfig)
        {
        }

        public virtual void OnUserBuildInitiated(AppConfig appConfig)
        {
        }

        public virtual void OnUnityCloudBuildInitiated(AppConfig appConfig)
        {
        }

        public virtual BuildPlayerOptions ChangeBuildPlayerOptions(AppConfig appConfig, BuildPlayerOptions options)
        {
            return options;
        }

        public virtual void DrawSettings(AppConfigSettings settings, float width)
        {
            SerializedObject serializedObject = new SerializedObject(settings);
            SerializedProperty prop = serializedObject.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.name == "m_Script")
                {
                    continue;
                }

                using (new BeginHorizontalHelper())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true, GUILayout.Width(width));
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
