//-----------------------------------------------------------------------
// <copyright file="CatalogsEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    #if ENABLE_PLAYFABADMIN_API
    using PlayFab.AdminModels;
    using PlayFab.PfEditor;
    #endif

    [CustomEditor(typeof(Catalogs))]
    public class CatalogsEditor : Editor
    {
        private string[] catalogVersions;
        private int activeCatalogIndex;

        public void OnEnable()
        {
            var catalogs = this.target as Catalogs;
            this.catalogVersions = catalogs.AllCatalogs.Select(x => x.Version).ToArray();
            this.activeCatalogIndex = 0;
        }

        public override void OnInspectorGUI()
        {
            this.DrawDefaultInspector();

            GUILayout.Label("");

            var catalogs = target as Catalogs;

            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("Selected Catalog", GUILayout.Width(120));
                this.activeCatalogIndex = EditorGUILayout.Popup(this.activeCatalogIndex, this.catalogVersions, GUILayout.Width(80));
            }

            GUILayout.Label("");

            if (GUILayout.Button("Edit Catalog", GUILayout.Width(200)))
            {
                var window = EditorWindow.GetWindow<CatalogEditor>();
                window.SetActiveCatalog(catalogs, catalogs.GetCatalog(this.catalogVersions[this.activeCatalogIndex]));
                window.Show();
            }

            #if ENABLE_PLAYFABADMIN_API
            if (GUILayout.Button("Upload Catalog", GUILayout.Width(200)))
            {
                this.UploadCatalog(catalogs.GetCatalog(this.catalogVersions[this.activeCatalogIndex]));
            }
            #endif

            GUILayout.Label("");
        }

        #if ENABLE_PLAYFABADMIN_API

        // Assets\PlayFabEditorExtensions\Editor\Scripts\PlayFabEditorSDK\PlayFabEditorApi.cs
        private void UploadCatalog(Catalog catalog)
        {
            // virtual currencies
            var addVirtualCurrenciesRequest = this.GetAddVirtualCurrenciesRequest(catalog);
            this.AddVirtualCurrencyTypes(addVirtualCurrenciesRequest, (result) => Debug.Log("Add Virtual Currencies Successful"), (error) => Debug.LogErrorFormat("Add Virtual Currencies Failed: {0}", error.ToString()));

            // catalog items
            var catalogItemsRequest = this.GetUpdateCatalogItemsRequest(catalog);
            this.UpdateCatalogItems(catalogItemsRequest, (result) => Debug.Log("Upload Catalog Items Successful"), (error) => Debug.LogErrorFormat("Upload Catalog Items Failed: {0}", error.ToString()));
             
            // adding stores
            foreach (var store in catalog.Stores)
            {
                var updateStoreItemRequest = this.GetUpdateStoreItemsRequest(catalog, store);
                this.SetStoreItems(updateStoreItemRequest, (result) => Debug.LogFormat("Upload Store Items {0} Successful", store.Id), (error) => Debug.LogErrorFormat("Upload Store Items {0} Failed: {1}", store.Id, error.ToString()));
            }
        }
        
        private AddVirtualCurrencyTypesRequest GetAddVirtualCurrenciesRequest(Catalog catalog)
        {
            var request = new AddVirtualCurrencyTypesRequest();
            request.VirtualCurrencies = new List<VirtualCurrencyData>();

            foreach (var vc in catalog.VirtualCurrencies)
            {
                request.VirtualCurrencies.Add(new VirtualCurrencyData
                {
                    CurrencyCode = vc.Id,
                    DisplayName = vc.Name,
                    InitialDeposit = vc.InitialDeposit,
                    RechargeRate = vc.RechargeRate,
                    RechargeMax = vc.RechargeMax,
                });
            }

            return request;
        }
        
        private UpdateCatalogItemsRequest GetUpdateCatalogItemsRequest(Catalog catalog)
        {
            var request = new UpdateCatalogItemsRequest
            {
                CatalogVersion = catalog.Version,
                Catalog = new List<PlayFab.AdminModels.CatalogItem>(),
            };
            
            foreach (var catalogItem in catalog.CatalogItems)
            {
                request.Catalog.Add(new PlayFab.AdminModels.CatalogItem
                {
                    ItemId = catalogItem.Id,
                    ItemClass = catalogItem.ItemClass,
                    Description = catalogItem.Description.Value,
                    DisplayName = catalogItem.DisplayName.Value,
                    IsStackable = catalogItem.IsStackable,
                    Consumable = catalogItem.UsageType == UsageType.Consumable ? new CatalogItemConsumableInfo { UsageCount = 1 } : null,
                });
            }

            foreach (var bundleItem in catalog.BundleItems)
            {
                var bundleInfo = new CatalogItemBundleInfo();

                foreach (var item in bundleItem.Items)
                {
                    if (item.Type == BundleItemType.VirtualCurrency)
                    {
                        if (bundleInfo.BundledVirtualCurrencies == null)
                        {
                            bundleInfo.BundledVirtualCurrencies = new Dictionary<string, uint>();
                        }

                        bundleInfo.BundledVirtualCurrencies.Add(item.Id, (uint)item.Count);
                    }
                    else if (item.Type == BundleItemType.CatalogItem)
                    {
                        if (bundleInfo.BundledItems == null)
                        {
                            bundleInfo.BundledItems = new List<string>();
                        }

                        for (int i = 0; i < item.Count; i++)
                        {
                            bundleInfo.BundledItems.Add(item.Id);
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("Found Unknown BundleItemType {0}", item.Type.ToString());
                    }
                }

                request.Catalog.Add(new PlayFab.AdminModels.CatalogItem
                {
                    ItemId = bundleItem.Id,
                    ItemClass = bundleItem.ItemClass,
                    Description = bundleItem.Description.Value,
                    DisplayName = bundleItem.DisplayName.Value,
                    Consumable = new CatalogItemConsumableInfo { UsagePeriod = 3 },
                    Bundle = bundleInfo,
                });
            }
            
            return request;
        }

        private UpdateStoreItemsRequest GetUpdateStoreItemsRequest(Catalog catalog, Store store)
        {
            var request = new UpdateStoreItemsRequest
            {
                CatalogVersion = catalog.Version,
                StoreId = store.Id,
                Store = new List<PlayFab.AdminModels.StoreItem>(),
            };

            uint displayPosition = 0;
            foreach (var storeItem in store.StoreItems)
            {
                request.Store.Add(new PlayFab.AdminModels.StoreItem
                {
                    ItemId = storeItem.ItemId,
                    DisplayPosition = displayPosition++,
                    VirtualCurrencyPrices = new Dictionary<string, uint> { { storeItem.CostCurrencyId, (uint)storeItem.Cost } },
                });
            }

            return request;
        }

        private void AddVirtualCurrencyTypes(AddVirtualCurrencyTypesRequest req, Action<BlankResult> resultCb, Action<PlayFab.PfEditor.EditorModels.PlayFabError> errorCb)
        {
            PlayFabEditorHttp.MakeApiCall("/Admin/AddVirtualCurrencyTypes", this.GetEndpoint(), req, resultCb, errorCb);
        }

        private void UpdateCatalogItems(UpdateCatalogItemsRequest req, Action<UpdateCatalogItemsResult> resultCb, Action<PlayFab.PfEditor.EditorModels.PlayFabError> errorCb)
        {
            PlayFabEditorHttp.MakeApiCall("/Admin/UpdateCatalogItems", this.GetEndpoint(), req, resultCb, errorCb);
        }
        
        private void SetStoreItems(UpdateStoreItemsRequest req, Action<UpdateStoreItemsResult> resultCb, Action<PlayFab.PfEditor.EditorModels.PlayFabError> errorCb)
        {
            this.GetEndpoint();
            PlayFabEditorHttp.MakeApiCall("/Admin/SetStoreItems", this.GetEndpoint(), req, resultCb, errorCb);
        }

        private string GetEndpoint()
        {
            PlayFabEditorDataService.SharedSettings.TitleId = AppSettings.ActiveConfig.PlayfabTitleId;
            PlayFabEditorDataService.SharedSettings.DeveloperSecretKey = AppSettings.ActiveConfig.PlayfabSecretId;
            return "https://" + PlayFabEditorDataService.ActiveTitle.Id + PlayFabEditorHelper.TITLE_ENDPOINT;
        }

        #endif
    }
}
