//-----------------------------------------------------------------------
// <copyright file="SupportedPlatformsSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lost.EditorGrid;
    using Lost.AppConfig;
    using UnityEngine;

    // TODO [bgish]: Try adding all the button types to the grid and make sure they all still look good.
    //               May need to update the up and down buttons.

    [AppConfigSettingsOrder(500)]
    public class LostDefineSettings : AppConfigSettings
    {
        private static EditorGrid.EditorGrid definesGrid;
        private static EditorGridDefinition definesGridDefinition;

        #pragma warning disable 0649
        [SerializeField] private List<Define> defines = new List<Define>();
        #pragma warning restore 0649

        public override string DisplayName => "Lost Defines";
        public override bool IsInline => false;

        public override void DrawSettings(AppConfigSettings settings, float width)
        {
            var lostDefineSettings = settings as LostDefineSettings;

            // Adjusting the first columns width to fit the total size of the space
            definesGridDefinition[0].Width = (int)(width - definesGridDefinition[1].Width);

            using (new BeginGridScope(definesGrid))
            {
                foreach (var define in lostDefineSettings.defines)
                {
                    using (new BeginGridRowScope(definesGrid))
                    {
                        definesGrid.DrawLabel(define.Name);
                        define.IsEnabled = definesGrid.DrawBool(define.IsEnabled);
                    }
                }
            }
        }

        public override void InitializeOnLoad(Lost.AppConfig.AppConfig appConfig)
        {
            var lostDefineSettings = appConfig.GetSettings<LostDefineSettings>();

            HashSet<string> definesToAdd = new HashSet<string>();
            HashSet<string> definesToRemove = new HashSet<string>();

            foreach (var define in lostDefineSettings.defines)
            {
                if (define.IsEnabled)
                {
                    definesToAdd.Add(define.Name);
                }
                else
                {
                    definesToRemove.Add(define.Name);
                }
            }

            EditorAppConfigDefinesHelper.UpdateProjectDefines(definesToAdd, definesToRemove);
        }

        private void OnEnable()
        {
            if (definesGrid == null)
            {
                definesGridDefinition = new EditorGridDefinition();
                definesGridDefinition.AddColumn("Define", 200);
                definesGridDefinition.AddColumn("Enabled", 75);
                definesGridDefinition.RowButtons = GridButton.None;
                definesGridDefinition.DrawHeader = false;

                definesGrid = new EditorGrid.EditorGrid(definesGridDefinition);
            }

            this.ConfigureLostDefines();
        }

        private void ConfigureLostDefines()
        {
            this.AddLostDefine(new Define("UNITY", true));
            this.AddLostDefine(new Define("USING_PLAYFAB_SDK", false));
            this.AddLostDefine(new Define("USING_FACEBOOK_SDK", false));
            this.AddLostDefine(new Define("USING_ANDROID_FIREBASE_MESSAGING", false));
            this.AddLostDefine(new Define("USING_UNITY_ADS", false));
            this.AddLostDefine(new Define("USING_UNITY_ANALYTICS", false));
            this.AddLostDefine(new Define("USING_UNITY_PURCHASING", false));
            this.AddLostDefine(new Define("USING_UNITY_ADDRESSABLES", false));
            this.AddLostDefine(new Define("USING_UNITY_AR_FOUNDATION", false));
        }

        private void AddLostDefine(Define define)
        {
            if (this.defines.FirstOrDefault(x => x.Name == define.Name) == null)
            {
                this.defines.Add(define);
            }
        }

        [Serializable]
        public class Define
        {
            #pragma warning disable 0649
            [SerializeField] private string name;
            [SerializeField] private bool isEnabled;
            #pragma warning restore 0649

            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public bool IsEnabled
            {
                get { return this.isEnabled; }
                set { this.isEnabled = value; }
            }

            public Define(string name, bool isEnabled)
            {
                this.name = name;
                this.isEnabled = isEnabled;
            }
        }
    }
}
