//-----------------------------------------------------------------------
// <copyright file="IAPCatalogExporter.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public static class IAPCatalogExporter
    {
        // https://help.yudu.com/hc/en-us/articles/201498096-Bulk-Upload-In-App-Purchases-Google-Play
        [MenuItem("Lost/PlayFab/Export Google Play CSV")]
        public static void ExportGooglePlayCatalog()
        {
            PlayFabSettingsHelper.Initialize();

            StringBuilder outputFile = new StringBuilder();
            outputFile.AppendLine("Product Id, Published State, Purchase Type, Auto Translate, Locale; Title; Description, Auto Fill Prices, Price");

            // Getting the CatalogItems
            var catalogItems = PlayFabEditorAdmin.GetCatalogItems("1.0");

            Debug.Log("Catalog Count = " + catalogItems?.Result?.Catalog?.Count);

            // // Foreach Catalog Item...
            // string productId = string.Empty;
            // string purchaseType = string.Empty;
            // string autoTranslate = string.Empty;
            // string autoFillPrices = string.Empty;
            // StringBuilder localeTitleDescription = new StringBuilder();
            // StringBuilder price = new StringBuilder();
            // AppendRow(outputFile, productId, purchaseType, autoTranslate, localeTitleDescription.ToString(), autoFillPrices, price.ToString());
            //
            // // Exporint out the file to the desktop
            // string desktop = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // string outputPath = System.IO.Path.Combine(desktop, "GooglePlay.csv");
            // System.IO.File.WriteAllText(outputPath, outputFile.ToString());
        }

        private static void AppendRow(StringBuilder outputFile, string productId, string purchaseType, string autoTranslate, string localeTitleDescription, string autoFillPrices, string price)
        {
            // Product Id
            outputFile.Append(productId);
            outputFile.Append(",");

            // Published State
            outputFile.Append("published,");

            // Purchase Type
            outputFile.Append(purchaseType);
            outputFile.Append(",");

            // Auto Translate
            outputFile.Append(autoTranslate);
            outputFile.Append(",");

            // Locale; Title; Description
            outputFile.Append(localeTitleDescription);
            outputFile.Append(",");

            // Auto Fill Prices
            outputFile.Append(autoFillPrices);
            outputFile.Append(",");

            // Price
            outputFile.Append(price);
            outputFile.Append(",");
        }
    }
}
