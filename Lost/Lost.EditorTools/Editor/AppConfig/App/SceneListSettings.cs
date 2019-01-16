//-----------------------------------------------------------------------
// <copyright file="SceneListSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [AppConfigSettingsOrder(30)]
    public class SceneListSettings : AppConfigSettings
    {
        #pragma warning disable 0649
        [SerializeField] private bool overrideSceneList;
        [SerializeField] private Scene sceneList;
        #pragma warning restore 0649

        public override string DisplayName => "Scene List Override";
        public override bool IsInline => false;
    }
}
