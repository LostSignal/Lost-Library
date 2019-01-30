//-----------------------------------------------------------------------
// <copyright file="StrictModeSettings.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;
    using UnityEditor;

    public class StrictModeSettings : AppConfigSettings
    {
        public override string DisplayName => "Build Strict Mode";
        public override bool IsInline => true;

        public bool buildInStrictMode = true;

        public override BuildPlayerOptions ChangeBuildPlayerOptions(AppConfig.AppConfig appConfig, BuildPlayerOptions options)
        {
            if (this.buildInStrictMode)
            {
                options.options |= BuildOptions.StrictMode;
            }
            else
            {
                options.options &= ~BuildOptions.StrictMode;
            }

            return options;
        }
    }
}
