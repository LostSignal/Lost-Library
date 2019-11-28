//-----------------------------------------------------------------------
// <copyright file="PerforceSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;

    // NOTE [bgish]: Possibly look into EditorUserSettings.semanticMergeMode more in the future

    [AppConfigSettingsOrder(200)]
    public class PerforceSettings : AppConfigSettings
    {
        public enum BuildNumberType
        {
            None,
            CommitNumber,
            BuildNumber,
        }

        #pragma warning disable 0649
        [SerializeField] private string server;
        [SerializeField] private BuildNumberType buildNumberType;
        [SerializeField] private int incrementBuildNumberBy;
        [SerializeField] private bool autosetVersionControlToPerforce;

        [Header("Build User Info")]
        [SerializeField] private string user;
        [SerializeField] private string password;
        [SerializeField] private string workspace;
        [SerializeField] private string unityFolder;
        #pragma warning restore 0649

        public override string DisplayName => "Perforce";
        public override bool IsInline => false;

        public override void OnPreproccessBuild(AppConfig.AppConfig appConfig, BuildReport buildReport)
        {
            var settings = appConfig.GetSettings<PerforceSettings>();

            if (settings == null)
            {
                return;
            }

            int buildNumber = this.GetBuildNumber(settings);

            if (buildNumber != -1)
            {
                PlayerSettings.iOS.buildNumber = buildNumber.ToString();
                PlayerSettings.Android.bundleVersionCode = buildNumber;
            }
        }

        public override void InitializeOnLoad(AppConfig.AppConfig buildConfig)
        {
            var settings = buildConfig.GetSettings<PerforceSettings>();

            if (settings == null)
            {
                return;
            }

            if (settings.autosetVersionControlToPerforce)
            {
                if (EditorSettings.externalVersionControl != "Perforce")
                {
                    EditorSettings.externalVersionControl = "Perforce";
                }

                EditorUserSettings.SetConfigValue("vcPerforceServer", settings.server);
            }

            //// // testing if we should set the p4ignore variable
            //// if (appSettings.SourceControl == SourceControlType.Perforce && appSettings.UseP4IgnoreFile && appSettings.SetP4IgnoreVariableAtStartup && appSettings.P4IgnoreFileName != GetCurrentP4IgnoreVariable())
            //// {
            ////     SetP4IgnoreFileVariable(appSettings.P4IgnoreFileName);
            //// }

            // TODO [bgish]: if source control is set to P4, but server isn't set, then set it here
            // TODO [bgish]: if autosetP4IgnoreVariable, then set p4ignore via command line

            // NOTE [bgish]: Future settings we could possible alter?
            // EditorUserSettings.GetConfigValue("vcPerforceUsername");
            // EditorUserSettings.GetConfigValue("vcPerforcePassword");
            // EditorUserSettings.GetConfigValue("vcPerforceWorkspace");
            // EditorUserSettings.GetConfigValue("vcPerforceHost");
            // EditorUserSettings.GetConfigValue("vcSharedLogLevel");
        }

        private int GetBuildNumber(PerforceSettings settings)
        {
            if (Platform.IsUnityCloudBuild == false)
            {
                // NOTE [bgish]: Gradle Build will fail if build number is 0, so returning 1
                // android.defaultConfig.versionCode is set to 0, but it should be a positive integer.
                return 1; 
            }

            var cloudBuildManifest = CloudBuildManifest.Find();

            if (cloudBuildManifest == null)
            {
                Debug.LogError("PerforceSettings couldn't find CloudBuildManifest!");
            }
            else if (settings.buildNumberType == BuildNumberType.None)
            {
                Debug.Log("PerforceSettings skipping setting application build number");
            }
            else if (settings.buildNumberType == BuildNumberType.BuildNumber)
            {
                Debug.LogFormat("PerforceSettings setting application build number to unity cloud BuildNumber {0}!", cloudBuildManifest.BuildNumber);
                return cloudBuildManifest.BuildNumber + settings.incrementBuildNumberBy;
            }
            else if (settings.buildNumberType == BuildNumberType.CommitNumber)
            {
                string commitId = cloudBuildManifest.ScmCommitId;

                if (int.TryParse(commitId, out int commitNumber))
                {
                    Debug.LogFormat("PerforceSettings setting application build number to ScmCommitId {0}!", commitId);
                    return commitNumber + settings.incrementBuildNumberBy;
                }
                else
                {
                    Debug.LogErrorFormat("PerforceSettings couldn't parse ScmCommitId {0}.  It is not a valid integer!", commitId);
                }
            }
            else
            {
                Debug.LogErrorFormat("Found unknown BuildNumberType {0}", settings.buildNumberType);
            }

            return -1;
        }
    }
}
