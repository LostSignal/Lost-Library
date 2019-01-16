//-----------------------------------------------------------------------
// <copyright file="PlayFabRuntimBuildConfigExtensions.cs" company="DefaultCompany">
//     Copyright (c) DefaultCompany. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using Lost.AppConfig;

    public static class PlayFabConfigExtensions
    {
        public static readonly string TitleId = "PlayFab.TitleId";
        public static readonly string CatalogVersion = "PlayFab.CatalogVersion";
        public static readonly string CloudScriptRevision = "PlayFab.CloudScriptRevision";

        public static string GetTitleId(this RuntimeAppConfig runtimeConfig)
        {
            return runtimeConfig.GetString(TitleId);
        }

        public static string GetCatalogVersion(this RuntimeAppConfig runtimeConfig)
        {
            return runtimeConfig.GetString(CatalogVersion);
        }

        public static int GetCloudScriptRevision(this RuntimeAppConfig runtimeConfig)
        {
            int revision = 0;
            if (int.TryParse(runtimeConfig.GetString(CloudScriptRevision), out revision))
            {
                return revision;
            }

            return 0;
        }
    }
}
