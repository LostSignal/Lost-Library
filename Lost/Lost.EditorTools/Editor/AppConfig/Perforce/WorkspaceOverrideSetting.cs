//-----------------------------------------------------------------------
// <copyright file="WorkspaceOverrideSetting.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using Lost.AppConfig;

    [AppConfigSettingsOrder(210)]
    public class WorkspaceOverrideSetting : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private string workspace;
        #pragma warning restore 0649

        public override string DisplayName => "Perforce Workspace Override";
        public override bool IsInline => true;
    }
}
