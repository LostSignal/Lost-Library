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
            
            // removing the ".meta" extension
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf("."));
            
            bool isLostFolder = assetPath.StartsWith("Assets/Code/Lost/");
            
            // special casing the lost folder
            string companyName = isLostFolder ? "Lost Signal LLC" : PlayerSettings.companyName;
            string nameSpace = isLostFolder ? "Lost" : PlayerSettings.productName.Replace(" ", "");

            string fileContents = File.ReadAllText(assetPath);
            fileContents = fileContents.Replace("#COMPANY_NAME#", companyName);
            fileContents = fileContents.Replace("#NAMESPACE#", nameSpace);
            
            File.WriteAllText(assetPath, fileContents);
            AssetDatabase.Refresh();
        }
    }
}
