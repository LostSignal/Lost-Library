//-----------------------------------------------------------------------
// <copyright file="TemplateScriptAssetProcessor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.IO;
    using Lost.AppConfig;
    using UnityEditor;
    using UnityEngine;

    public class TemplateScriptAssetProcessor : AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string assetPath)
        {
            if (assetPath.EndsWith(".cs.meta") == false)
            {
                return;
            }

            var settings = EditorAppConfig.ActiveAppConfig?.GetSettings<OverrideTemplatesSettings>();

            if (settings == null)
            {
                return;
            }

            // removing the ".meta" extension
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf("."));

            // Determining the company name and namespace
            bool isLostFolder = assetPath.Contains("/Lost/");
            string companyName = "Lost Signal LLC";
            string nameSpace = "Lost";

            if (isLostFolder == false)
            {
                companyName = string.IsNullOrWhiteSpace(PlayerSettings.companyName) ? "Player Settings Company Not Defined" : PlayerSettings.companyName;
                nameSpace = string.IsNullOrWhiteSpace(EditorSettings.projectGenerationRootNamespace) ? "EditorSettingsRootNamespaceNotDefined" : EditorSettings.projectGenerationRootNamespace;
            }

            // getting the script name and the template file to use
            string scriptName = Path.GetFileNameWithoutExtension(assetPath);
            TextAsset templateFile = GetTemplateTextAsset(settings, assetPath);

            WriteCSharpFile(assetPath, templateFile, companyName, nameSpace, scriptName);
        }

        private static TextAsset GetTemplateTextAsset(OverrideTemplatesSettings settings, string assetPath)
        {
            string fileContents = File.ReadAllText(assetPath);

            if (fileContents.Contains(": MonoBehaviour"))
            {
                return settings.MonoBehavoiur;
            }
            else if (fileContents.Contains(": PlayableAsset"))
            {
                return settings.PlayableAsset;
            }
            else if (fileContents.Contains(": PlayableBehaviour"))
            {
                return settings.PlayableBehaviour;
            }
            else if (fileContents.Contains(": StateMachineBehaviour"))
            {
                if (fileContents.Contains("OnStateMachineEnter"))
                {
                    return settings.SubStateMachineBehaviour;
                }
                else
                {
                    return settings.StateMachineBehaviour;
                }
            }
            else if (fileContents.Contains("[Test]"))
            {
                return settings.EditorTestScript;
            }
            else if (fileContents.Contains(": MonoBehaviour"))
            {
                return settings.MonoBehavoiur;
            }

            return null;
        }

        private static void WriteCSharpFile(string assetPath, TextAsset template, string companyName, string nameSpace, string scriptName)
        {
            string fileContents = template == null ? File.ReadAllText(assetPath) : template.text;

            fileContents = fileContents.Replace("#COMPANY_NAME#", companyName)
                                       .Replace("#NAMESPACE#", nameSpace)
                                       .Replace("#SCRIPTNAME#", scriptName)
                                       .Replace("#NOTRIM#", string.Empty);

            File.WriteAllText(assetPath, FileUtil.ConvertLineEndings(fileContents, EditorSettings.lineEndingsForNewScripts));
            AssetDatabase.Refresh();
        }
    }
}
