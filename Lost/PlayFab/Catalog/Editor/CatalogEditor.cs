//-----------------------------------------------------------------------
// <copyright file="CatalogEditor.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class CatalogEditor : EditorWindow
    {
        private static int foldoutId = 38565739;

        // grids
        private static Grid itemClassesGrid;
        private static Grid virtualCurrencyGrid;
        private static Grid catalogItemGrid;
        private static Grid bundleItemGrid;
        private static Grid storeGrid;

        // caching
        private string[] allVirtualCurrenciesCache;
        private string[] virtualCurrenciesCache;
        private string[] catalogItemsCache;
        private string[] bundleItemsCache;

        // scroll view
        private Vector2 scrollViewPosition = Vector2.zero;

        // store
        private int currentStoreIndex = -1;

        public Catalogs ActiveCatalogs { get; private set; }

        public Catalog ActiveCatalog { get; private set; }

        public void SetActiveCatalog(Catalogs catalogs, Catalog activeCatalog)
        {
            if (this.ActiveCatalog != activeCatalog)
            {
                this.ActiveCatalogs = catalogs;
                this.ActiveCatalog = activeCatalog;
                this.UpdateCache();
            }
        }

        private void OnEnable()
        {
            this.scrollViewPosition = Vector2.zero;

            if (itemClassesGrid == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Item Class Name", 250);
                gridDefinition.RowButtons = GridButton.All;
                gridDefinition.DrawHeader = true;

                itemClassesGrid = new Grid(gridDefinition);
            }

            if (virtualCurrencyGrid == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Id", 80);
                gridDefinition.AddColumn("Name", 100);
                gridDefinition.AddColumn("Initial Deposit", 100);
                gridDefinition.AddColumn("Recharge Rate", 120);
                gridDefinition.AddColumn("Recharge Max", 120);
                gridDefinition.RowButtons = GridButton.All;
                gridDefinition.DrawHeader = true;

                virtualCurrencyGrid = new Grid(gridDefinition);
            }

            if (catalogItemGrid == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Id", 300);
                gridDefinition.AddColumn("Item Class", 100);
                gridDefinition.AddColumn("Item Type", 100);
                gridDefinition.AddColumn("Display Name", 120);
                gridDefinition.AddColumn("Description", 100);
                gridDefinition.AddColumn("RM Cost", 80);
                gridDefinition.AddColumn("IsStackable", 80);
                gridDefinition.RowButtons = GridButton.All;
                gridDefinition.DrawHeader = true;

                catalogItemGrid = new Grid(gridDefinition);
            }

            if (bundleItemGrid == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Id", 300);
                gridDefinition.AddColumn("Item Class", 100);
                gridDefinition.AddColumn("Display Name", 120);
                gridDefinition.AddColumn("Description", 100);
                gridDefinition.AddColumn("First Item Type", 120);
                gridDefinition.AddColumn("First Item", 100);
                gridDefinition.AddColumn("First Item Count", 100);
                gridDefinition.AddColumn("RM Cost", 80);
                gridDefinition.RowButtons = GridButton.All;
                gridDefinition.DrawHeader = true;

                bundleItemGrid = new Grid(gridDefinition);
            }

            if (storeGrid == null)
            {
                var gridDefinition = new GridDefinition();
                gridDefinition.AddColumn("Item Type", 120);
                gridDefinition.AddColumn("Item Id", 300);
                gridDefinition.AddColumn("Virtual Currency", 120);
                gridDefinition.AddColumn("Cost", 80);
                gridDefinition.AddColumn("Purchase Description", 250);
                gridDefinition.AddColumn("Purchase Icon", 120);
                gridDefinition.RowButtons = GridButton.All;
                gridDefinition.DrawHeader = true;

                storeGrid = new Grid(gridDefinition);
            }
        }

        private void OnGUI()
        {
            this.scrollViewPosition = GUILayout.BeginScrollView(this.scrollViewPosition, GUILayout.Width(1500));

            {
                GUILayout.Label("");

                DrawCatalog(ActiveCatalog);

                GUILayout.Label("");

                // using (new GuiBackgroundHelper(Color.red))
                // {
                //     // only showing the update defines button if they actually need updating
                //     if (UpdateProjectDefines(appSettings, false) && GUILayout.Button("Update Defines"))
                //     {
                //         UpdateProjectDefines(appSettings, true);
                //     }
                // }

                // making sure we mark the data dirty if it changed
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(ActiveCatalogs);
                }
            }

            GUILayout.EndScrollView();
        }

        private void DrawCatalog(Catalog catalog)
        {
            if (catalog == null)
            {
                return;
            }

            // Version
            using (new BeginHorizontalHelper())
            {
                GUILayout.Label("Catalog Version", GUILayout.Width(100));
                catalog.Version = EditorGUILayout.TextField(GUIContent.none, catalog.Version, GUILayout.Width(200));
            }

            GUILayout.Label("");

            this.DrawItemClasses(foldoutId + 0);
            this.DrawVirtualCurrencies(foldoutId + 1);
            this.DrawCatalogItems(foldoutId + 2);
            this.DrawBundleItems(foldoutId + 3);
            this.DrawStores(foldoutId + 4);

            if (GUI.changed)
            {
                Debug.Log("GUI Changed");
                this.UpdateCache();
            }
        }

        private void DrawItemClasses(int foldoutId)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Item Classes", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                GUILayout.Label("");

                var grid = itemClassesGrid;
                var gridItems = ActiveCatalog.ItemClasses;

                using (new BeginGridHelper(grid))
                {
                    int index = 0;

                    foreach (var item in gridItems)
                    {
                        using (new BeginGridRowHelper(grid))
                        {
                            gridItems[index] = grid.DrawString(item);
                        }

                        if (Grid.UpdateList<string>(grid.RowButtonPressed, gridItems, index))
                        {
                            break;
                        }

                        index++;
                    }

                    if (grid.DrawAddButton())
                    {
                        gridItems.Add(string.Empty);
                    }

                    GUILayout.Label("");
                }
            }
        }

        private void DrawVirtualCurrencies(int foldoutId)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Virtual Currencies", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                GUILayout.Label("");

                var grid = virtualCurrencyGrid;
                var gridItems = ActiveCatalog.VirtualCurrencies;

                using (new BeginGridHelper(grid))
                {
                    int index = 0;

                    foreach (var item in gridItems)
                    {
                        using (new BeginGridRowHelper(grid))
                        {
                            item.Id = grid.DrawString(item.Id);
                            item.Name = grid.DrawString(item.Name);
                            item.InitialDeposit = grid.DrawInt(item.InitialDeposit);
                            item.RechargeRate = grid.DrawInt(item.RechargeRate);
                            item.RechargeMax = grid.DrawInt(item.RechargeMax);
                        }

                        if (Grid.UpdateList<VirtualCurrency>(grid.RowButtonPressed, gridItems, index))
                        {
                            break;
                        }

                        index++;
                    }

                    if (grid.DrawAddButton())
                    {
                        gridItems.Add(new VirtualCurrency());
                    }

                    GUILayout.Label("");
                }
            }
        }

        private void DrawCatalogItems(int foldoutId)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Catalog Items", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                GUILayout.Label("");

                var grid = catalogItemGrid;
                var gridItems = ActiveCatalog.CatalogItems;

                using (new BeginGridHelper(grid))
                {
                    int index = 0;

                    foreach (var catalogItem in gridItems)
                    {
                        using (new BeginGridRowHelper(grid))
                        {
                            catalogItem.Id = grid.DrawString(catalogItem.Id);
                            catalogItem.ItemClass = grid.DrawPopup(ActiveCatalog.ItemClasses, catalogItem.ItemClass);
                            catalogItem.UsageType = grid.DrawEnum<UsageType>(catalogItem.UsageType);
                            catalogItem.DisplayName.Id = grid.DrawString(catalogItem.DisplayName.Id);
                            catalogItem.Description.Id = grid.DrawString(catalogItem.Description.Id);
                            catalogItem.RealMoneyCost = grid.DrawInt(catalogItem.RealMoneyCost);
                            catalogItem.IsStackable = grid.DrawBool(catalogItem.IsStackable);
                        }

                        if (Grid.UpdateList<CatalogItem>(grid.RowButtonPressed, gridItems, index))
                        {
                            break;
                        }

                        index++;
                    }

                    if (grid.DrawAddButton())
                    {
                        gridItems.Add(new CatalogItem());
                    }

                    GUILayout.Label("");
                }
            }
        }

        private void DrawBundleItems(int foldoutId)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Bundle Items", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                var grid = bundleItemGrid;
                var gridItems = ActiveCatalog.BundleItems;

                GUILayout.Label("");

                using (new BeginGridHelper(grid))
                {
                    int index = 0;

                    foreach (var gridItem in gridItems)
                    {
                        using (new BeginGridRowHelper(grid))
                        {
                            gridItem.Id = grid.DrawString(gridItem.Id);
                            gridItem.ItemClass = grid.DrawPopup(ActiveCatalog.ItemClasses, gridItem.ItemClass);
                            gridItem.DisplayName.Id = grid.DrawString(gridItem.DisplayName.Id);
                            gridItem.Description.Id = grid.DrawString(gridItem.Description.Id);
                            gridItem.Items[0].Type = grid.DrawEnum<BundleItemType>(gridItem.Items[0].Type);

                            if (gridItem.Items[0].Type == BundleItemType.CatalogItem)
                            {
                                gridItem.Items[0].Id = grid.DrawPopup(this.catalogItemsCache, gridItem.Items[0].Id);
                            }
                            else if (gridItem.Items[0].Type == BundleItemType.VirtualCurrency)
                            {
                                gridItem.Items[0].Id = grid.DrawPopup(this.virtualCurrenciesCache, gridItem.Items[0].Id);
                            }
                            else
                            {
                                Debug.LogErrorFormat("Unknown BundleItemType {0}", gridItem.Items[0].Type);
                                grid.DrawLabel("?");
                            }

                            gridItem.Items[0].Count = grid.DrawInt(gridItem.Items[0].Count);
                            gridItem.RealMoneyCost = grid.DrawInt(gridItem.RealMoneyCost);
                        }

                        if (Grid.UpdateList<BundleItem>(grid.RowButtonPressed, gridItems, index))
                        {
                            break;
                        }

                        index++;
                    }

                    if (grid.DrawAddButton())
                    {
                        gridItems.Add(new BundleItem());
                    }

                    GUILayout.Label("");
                }
            }

            // TODO [bgish]:  If a BundleItem is Added/Removed/Renamed, then call this.UpdateCache()
        }

        private void DrawStores(int foldoutId)
        {
            bool isVisible = false;

            using (new FoldoutHelper(foldoutId, "Store Items", out isVisible))
            {
                if (isVisible == false)
                {
                    return;
                }

                GUILayout.Label("");

                if (this.currentStoreIndex == -1)
                {
                    if (this.ActiveCatalog.Stores.Count > 0)
                    {
                        this.currentStoreIndex = 0;
                    }
                    else if (GUILayout.Button("Create Catalog", GUILayout.Width(200)))
                    {
                        this.ActiveCatalog.Stores.Add(new Store() { Id = "New Store 0" });
                        this.currentStoreIndex = 0;
                    }

                    return;
                }

                if (GUILayout.Button("Add New Store", GUILayout.Width(200)))
                {
                    this.ActiveCatalog.Stores.Add(new Store() { Id = "New Store " + this.ActiveCatalog.Stores.Count });
                    this.currentStoreIndex = this.ActiveCatalog.Stores.Count - 1;
                }

                if (GUILayout.Button("Delete Selected Store", GUILayout.Width(200)))
                {

                    this.ActiveCatalog.Stores.RemoveAt(this.currentStoreIndex);

                    if (this.ActiveCatalog.Stores.Count == 0)
                    {
                        this.currentStoreIndex = -1;
                        return;
                    }
                    else
                    {
                        this.currentStoreIndex = Mathf.Max(0, this.currentStoreIndex - 1);
                    }
                }

                GUILayout.Label("");

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("Store", GUILayout.Width(40));

                    var storeIds = this.ActiveCatalog.Stores.Select(x => x.Id).ToArray();
                    this.currentStoreIndex = EditorGUILayout.Popup(this.currentStoreIndex, storeIds, GUILayout.Width(120));
                }

                GUILayout.Label("");

                using (new BeginHorizontalHelper())
                {
                    GUILayout.Label("Name", GUILayout.Width(40));
                    this.ActiveCatalog.Stores[this.currentStoreIndex].Id = EditorGUILayout.TextField(this.ActiveCatalog.Stores[this.currentStoreIndex].Id, GUILayout.Width(120));
                }

                GUILayout.Label("");

                this.DrawStore(foldoutId, this.ActiveCatalog.Stores[this.currentStoreIndex]);
            }
        }

        private void DrawStore(int foldoutId, Store store)
        {
            var grid = storeGrid;
            var gridItems = store.StoreItems;

            GUILayout.Label("");

            using (new BeginGridHelper(grid))
            {
                int index = 0;

                foreach (var gridItem in gridItems)
                {
                    using (new BeginGridRowHelper(grid))
                    {
                        gridItem.Type = grid.DrawEnum<StoreItemType>(gridItem.Type);

                        bool isIap = false;

                        if (gridItem.Type == StoreItemType.BundleItem)
                        {
                            gridItem.ItemId = grid.DrawPopup(this.bundleItemsCache, gridItem.ItemId);

                            var bundleItem = this.ActiveCatalog.BundleItems.FirstOrDefault(x => x.Id == gridItem.ItemId);

                            if (bundleItem != null && bundleItem.IsIapItem)
                            {
                                isIap = true;
                                gridItem.CostCurrencyId = "RM";
                                gridItem.Cost = bundleItem.RealMoneyCost;
                            }
                        }
                        else if (gridItem.Type == StoreItemType.CatalogItem)
                        {
                            gridItem.ItemId = grid.DrawPopup(this.catalogItemsCache, gridItem.ItemId);

                            var catalogItem = this.ActiveCatalog.CatalogItems.FirstOrDefault(x => x.Id == gridItem.ItemId);

                            if (catalogItem != null && catalogItem.IsIapItem)
                            {
                                isIap = true;
                                gridItem.CostCurrencyId = "RM";
                                gridItem.Cost = catalogItem.RealMoneyCost;
                            }
                        }
                        else
                        {
                            Debug.LogErrorFormat("Unknown StoreItemType {0}", gridItem.Type);
                            grid.DrawLabel("?");
                        }

                        if (isIap)
                        {
                            grid.DrawLabel(gridItem.CostCurrencyId);
                            grid.DrawLabel(gridItem.Cost.ToString());
                        }
                        else
                        {
                            gridItem.CostCurrencyId = grid.DrawPopup(this.allVirtualCurrenciesCache, gridItem.CostCurrencyId);
                            gridItem.Cost = grid.DrawInt(gridItem.Cost);
                        }

                        gridItem.PurchaseDescription = grid.DrawString(gridItem.PurchaseDescription);
                        gridItem.PurchaseIcon = grid.DrawSprite(gridItem.PurchaseIcon, false);
                    }

                    if (Grid.UpdateList<StoreItem>(grid.RowButtonPressed, gridItems, index))
                    {
                        break;
                    }

                    index++;
                }

                if (grid.DrawAddButton())
                {
                    gridItems.Add(new StoreItem { ItemId = this.catalogItemsCache.First() });
                }

                GUILayout.Label("");
            }
        }

        private void UpdateCache()
        {
            this.virtualCurrenciesCache = this.ActiveCatalog.VirtualCurrencies.Select(x => x.Id).ToArray();
            this.catalogItemsCache = this.ActiveCatalog.CatalogItems.Select(x => x.Id).ToArray();
            this.bundleItemsCache = this.ActiveCatalog.BundleItems.Select(x => x.Id).ToArray();

            // all virtual currencies
            List<string> allVirtualCurrencies = new List<string> { "RM", "AD" };
            allVirtualCurrencies.AddRange(virtualCurrenciesCache);
            this.allVirtualCurrenciesCache = allVirtualCurrencies.ToArray();
        }
    }
}
