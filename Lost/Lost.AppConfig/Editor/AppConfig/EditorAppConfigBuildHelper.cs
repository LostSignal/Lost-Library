//-----------------------------------------------------------------------
// <copyright file="EditorAppConfig.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.AppConfig
{
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;

    public class EditorAppConfigBuildHelper : IPreprocessBuildWithReport, IPostprocessBuildWithReport
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
    }
}
