//-----------------------------------------------------------------------
// <copyright file="EditorAppConfig.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEngine.SceneManagement;

    public class EditorAppConfigBuildHelper : IPreprocessBuildWithReport, IPostprocessBuildWithReport, IProcessSceneWithReport
        #if UNITY_ANDROID
        , UnityEditor.Android.IPostGenerateGradleAndroidProject
        #endif
    {
        int IOrderedCallback.callbackOrder => 1000;

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            var activeConfig = EditorAppConfig.ActiveAppConfig;

            foreach (var settings in EditorAppConfig.GetActiveConfigSettings())
            {
                settings.OnPostprocessBuild(activeConfig, report);
            }
        }

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            var activeConfig = EditorAppConfig.ActiveAppConfig;

            foreach (var settings in EditorAppConfig.GetActiveConfigSettings())
            {
                settings.OnPreproccessBuild(activeConfig, report);
            }
        }

        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
            var activeConfig = EditorAppConfig.ActiveAppConfig;

            foreach (var settings in EditorAppConfig.GetActiveConfigSettings())
            {
                settings.OnProcessScene(activeConfig, scene, report);
            }
        }

        #if UNITY_ANDROID
        void UnityEditor.Android.IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string gradlePath)
        {
            var activeConfig = EditorAppConfig.ActiveAppConfig;

            foreach (var settings in EditorAppConfig.GetActiveConfigSettings())
            {
                settings.OnPostGenerateGradleAndroidProject(activeConfig, gradlePath);
            }
        }
        #endif
    }
}
