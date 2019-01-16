//-----------------------------------------------------------------------
// <copyright file="CodeAnalysisSetting.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;

    [AppConfigSettingsOrder(25)]
    public class CodeAnalysisSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool runUnitTests;
        [SerializeField] private bool runCodeAnalysis;
        [SerializeField] private bool runStyleCop;
        [SerializeField] private bool warningsAsErrors;
        #pragma warning restore 0649

        public override string DisplayName => "Code Analysis";
        public override bool IsInline => false;
    }
}
