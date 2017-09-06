//-----------------------------------------------------------------------
// <copyright file="ScriptAssetProcessor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using UnityEditor;
    
    public class TemplateScriptAssetProcessor : AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string assetPath)
        {
            if (assetPath.EndsWith(".cs.meta") == false)
            {
                return;
            }

            AppSettings appSettings = AppSettingsHelper.GetAppSettings();

            // removing the ".meta" extension
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf("."));

            bool isLostFolder = assetPath.Contains("/Lost/");
            string companyName = "Lost Signal LLC";
            string nameSpace = "Lost";
            
            if (isLostFolder == false)
            {
                companyName = PlayerSettings.companyName.IsNullOrWhitespace() ? "Player Settings Company Not Defined" : PlayerSettings.companyName;
                nameSpace = EditorSettings.projectGenerationRootNamespace.IsNullOrWhitespace() ? "EditorSettingsRootNamespaceNotDefined" : EditorSettings.projectGenerationRootNamespace;
            }

            string fileContents = File.ReadAllText(assetPath);
            fileContents = fileContents.Replace("#COMPANY_NAME#", companyName);
            fileContents = fileContents.Replace("#NAMESPACE#", nameSpace);
            
            File.WriteAllText(assetPath, AppSettingsHelper.ConvertLineEndings(fileContents, appSettings.ProjectLineEndings));
            AssetDatabase.Refresh();
        }
    }
}
