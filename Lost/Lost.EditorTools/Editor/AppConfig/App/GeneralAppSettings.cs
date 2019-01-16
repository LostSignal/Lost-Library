//-----------------------------------------------------------------------
// <copyright file="GeneralAppSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEngine;

    [AppConfigSettingsOrder(20)]
    public class GeneralAppSettings : AppConfigSettings
    {
        public enum LineEndings
        {
            Unix,
            Windows,
        }

        #pragma warning disable 0649
        [SerializeField] private string appVersion;
        [SerializeField] private string productName;
        [SerializeField] private string companyName;
        [SerializeField] private string rootNamespace;
        [SerializeField] private LineEndings lineEndings;
        [SerializeField] private ExternalVersionControl versionContorl;
        [SerializeField] private SerializationMode serializationMode = SerializationMode.ForceText;
        #pragma warning restore 0649

        public override string DisplayName => "General App Settings";
        public override bool IsInline => false;

        public override void InitializeOnLoad(AppConfig.AppConfig buildConfig)
        {
            var settings = buildConfig.GetSettings<GeneralAppSettings>();

            if (settings == null)
            {
                return;
            }

            if (PlayerSettings.bundleVersion != settings.appVersion)
            {
                PlayerSettings.bundleVersion = settings.appVersion;
            }

            if (PlayerSettings.productName != settings.productName)
            {
                PlayerSettings.productName = settings.productName;
            }

            if (PlayerSettings.companyName != settings.companyName)
            {
                PlayerSettings.companyName = settings.companyName;
            }

            if (EditorSettings.projectGenerationRootNamespace != settings.rootNamespace)
            {
                EditorSettings.projectGenerationRootNamespace = settings.rootNamespace;
            }

            if (EditorSettings.lineEndingsForNewScripts != this.Convert(settings.lineEndings))
            {
                EditorSettings.lineEndingsForNewScripts = this.Convert(settings.lineEndings);
            }

            if (EditorSettings.serializationMode != settings.serializationMode)
            {
                EditorSettings.serializationMode = settings.serializationMode;
            }
        }

        private LineEndingsMode Convert(LineEndings lineEndings)
        {
            switch (lineEndings)
            {
                case LineEndings.Unix:
                    return LineEndingsMode.Unix;

                case LineEndings.Windows:
                    return LineEndingsMode.Windows;

                default:
                    Debug.LogErrorFormat("Found unknown line endings type {0}", lineEndings);
                    return LineEndingsMode.Unix;
            }
        }
    }
}
