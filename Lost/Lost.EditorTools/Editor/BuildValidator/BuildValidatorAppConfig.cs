//-----------------------------------------------------------------------
// <copyright file="BuildValidatorAppConfig.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [AppConfigSettingsOrder(2000)]
    public class BuildValidatorAppConfig : AppConfigSettings
    {
#pragma warning disable 0649
        [SerializeField] private bool warningsAsErrors;
#pragma warning restore 0649

        public override string DisplayName => "Build Validator";
        public override bool IsInline => true;

        public override void OnProcessScene(AppConfig.AppConfig appConfig, Scene scene, BuildReport report)
        {
            base.OnProcessScene(appConfig, scene, report);

            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                this.ValidateComponents(rootGameObject.GetComponentsInChildren(typeof(Component), true));
            }
        }

        public override void OnPreproccessBuild(AppConfig.AppConfig appConfig, BuildReport buildReport)
        {
            // Validating Prefabs
            foreach (var prefabPath in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.prefab", SearchOption.AllDirectories))
            {
                var prefab = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(prefabPath);

                this.ValidateComponents(prefab.GetComponentsInChildren(typeof(Component), true));
            }

            // Scriptable Objects
            foreach (var scriptableObjectPath in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.asset", SearchOption.AllDirectories))
            {
                var scriptableObject = AssetDatabase.LoadAssetAtPath(scriptableObjectPath, typeof(UnityEngine.ScriptableObject));
                var serializedObject = new SerializedObject(scriptableObject);

                this.ValidateSerializedObject(serializedObject);
            }
        }

        private void ValidateComponents(Component[] components)
        {
            foreach (var component in components)
            {
                SerializedObject serializedObject = new SerializedObject(component);

                this.ValidateSerializedObject(serializedObject);
            }
        }

        private void ValidateSerializedObject(SerializedObject serializedObject)
        {
            // Validate this object
            (serializedObject.targetObject as IValidate)?.Validate();

            // Validate all of it's properties
            // Recursively Iterate over all properties (make sure to check if it's an array and iterate over every element if it is)
        }
    }
}
