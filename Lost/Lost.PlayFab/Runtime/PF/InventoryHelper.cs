//-----------------------------------------------------------------------
// <copyright file="InventoryHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System.Collections.Generic;
    using PlayFab.ClientModels;

    public class InventoryHelper
    {
        private List<ItemInstance> usersInventory = null;
        private bool getInventoryCoroutineRunning;

        public delegate void OnInventoryChangedDelegate();
        public event OnInventoryChangedDelegate InventoryChanged;

        public InventoryHelper()
        {
            PF.PlayfabEvents.OnLoginResultEvent += this.PlayfabEvents_OnLoginResultEvent;
            PF.PlayfabEvents.OnGetUserInventoryResultEvent += this.PlayfabEvents_OnGetUserInventoryResultEvent;
            PF.PlayfabEvents.OnGetPlayerCombinedInfoResultEvent += PlayfabEvents_OnGetPlayerCombinedInfoResultEvent;
        }

        public void InvalidateUserInventory()
        {
            this.usersInventory = null;
            this.InventoryChanged?.Invoke();
        }

        public UnityTask<List<ItemInstance>> GetInventoryItems()
        {
            if (this.usersInventory != null)
            {
                return UnityTask<List<ItemInstance>>.Empty(this.usersInventory);
            }
            else
            {
                return UnityTask<List<ItemInstance>>.Run(GetInventoryItemsCoroutine());
            }            

            IEnumerator <List<ItemInstance>> GetInventoryItemsCoroutine()
            {
                // If it's already running, then wait for it to finish
                if (this.getInventoryCoroutineRunning)
                {
                    while (this.getInventoryCoroutineRunning)
                    {
                        yield return default(List<ItemInstance>);
                    }

                    yield return this.usersInventory;
                    yield break;
                }

                this.getInventoryCoroutineRunning = true;

                var playfabGetInventory = PF.Do(new GetUserInventoryRequest());

                while (playfabGetInventory.IsDone == false)
                {
                    yield return default(List<ItemInstance>);
                }

                this.getInventoryCoroutineRunning = false;

                yield return this.usersInventory;
            }
        }

        public UnityTask<int> GetInventoryCount(string itemId)
        {
            return UnityTask<int>.Run(GetInventoryCountCoroutine());

            IEnumerator<int> GetInventoryCountCoroutine()
            {
                var inventory = this.GetInventoryItems();

                while (inventory.IsDone == false)
                {
                    yield return default(int);
                }

                List<ItemInstance> items = inventory.Value;
                int count = 0;

                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ItemId == itemId)
                    {
                        int? remainingUses = items[i].RemainingUses;

                        if (remainingUses.HasValue)
                        {
                            count += remainingUses.Value;
                        }
                    }
                }

                yield return count;
            }
        }

        private void PlayfabEvents_OnLoginResultEvent(LoginResult result)
        {
            this.UpdateInventory(result.InfoResultPayload?.UserInventory);
        }

        private void PlayfabEvents_OnGetUserInventoryResultEvent(GetUserInventoryResult result)
        {
            this.UpdateInventory(result?.Inventory);
        }

        private void PlayfabEvents_OnGetPlayerCombinedInfoResultEvent(GetPlayerCombinedInfoResult result)
        {
            this.UpdateInventory(result.InfoResultPayload?.UserInventory);
        }

        private void UpdateInventory(List<ItemInstance> inventory)
        {
            if (this.usersInventory == null)
            {
                this.usersInventory = new List<ItemInstance>();
            }
            else
            {
                this.usersInventory.Clear();
            }

            if (inventory.IsNullOrEmpty() == false)
            {
                this.usersInventory.AddRange(inventory);
                this.InventoryChanged?.Invoke();
            }
        }

        // public delegate void OnUserInventoryChagnedDelegate();
        //// public event OnUserInventoryChagnedDelegate UserInventoryChanged;
        ////
        //// private void InternalAddCatalogItemToInventory(string catalogItemId, int count)
        //// {
        ////     var inventoryItem = this.userInventory.FirstOrDefault(x => x.Id == catalogItemId);
        ////
        ////     if (inventoryItem != null)
        ////     {
        ////         inventoryItem.Count += count;
        ////     }
        ////     else
        ////     {
        ////         this.userInventory.Add(new InventoryItem() { Id = catalogItemId, Count = count });
        ////     }
        ////
        ////     this.OnVirtualCurrenciesChanged();
        //// }
        ////
        //// public void InternalAddBundleItemToInventory(BundleItem bundleItem)
        //// {
        ////     foreach (var item in bundleItem.Items)
        ////     {
        ////         if (item.Type == BundleItemType.VirtualCurrency)
        ////         {
        ////             this.InternalAddVirtualCurrencyToInventory(item.Id, item.Count);
        ////         }
        ////         else if (item.Type == BundleItemType.CatalogItem)
        ////         {
        ////             this.InternalAddCatalogItemToInventory(item.Id, item.Count);
        ////         }
        ////         else
        ////         {
        ////             Debug.LogErrorFormat("PlayFabServer.InternalAddBundleItemToInventory found unknown BundleItemType {0} on bundle item id {1}", item.Type, bundleItem.Id);
        ////         }
        ////     }
        //// }
        ////
        //// private void UpdateUserInventory(List<ItemInstance> userInventory)
        //// {
        ////     this.userInventory.Clear();
        ////
        ////     foreach (ItemInstance item in userInventory)
        ////     {
        ////         this.userInventory.Add(new InventoryItem()
        ////         {
        ////             Id = item.ItemId,
        ////             Count = item.RemainingUses.HasValue ? item.RemainingUses.Value : 1,
        ////             Data = item.CustomData,
        ////             InstanceId = item.ItemInstanceId,
        ////         });
        ////     }
        ////
        ////     this.OnUserInventoryChanged();
        //// }

        ////
        //// public int GetInventoryCount(string itemId)
        //// {
        ////     var inventoryItem = this.userInventory.FirstOrDefault(x => x.Id == itemId);
        ////     return inventoryItem != null ? inventoryItem.Count : 0;
        //// }

        //// protected void InternalAddStoreItemToInventory(StoreItem storeItem)
        //// {
        ////     // deducting the virtual currency
        ////     this.InternalAddVirtualCurrencyToInventory(storeItem.CostCurrencyId, -storeItem.Cost);
        ////
        ////     // adding the item to the inventory
        ////     if (storeItem.Type == StoreItemType.BundleItem)
        ////     {
        ////         this.InternalAddBundleItemToInventory(storeItem.BundleItem);
        ////     }
        ////     else if (storeItem.Type == StoreItemType.CatalogItem)
        ////     {
        ////         this.InternalAddCatalogItemToInventory(storeItem.CatalogItem.Id, 1);
        ////     }
        ////     else
        ////     {
        ////         Debug.LogErrorFormat("PlayFabServer.InternalAddStoreItemToInventory found unknown StoreItemType {0} on store item id {1}", storeItem.Type, storeItem.ItemId);
        ////     }
        //// }
        ////
        //// protected void OnUserInventoryChanged()
        //// {
        ////     if (this.UserInventoryChanged != null)
        ////     {
        ////         this.UserInventoryChanged();
        ////     }
        //// }

        //// // inventory
        //// private List<InventoryItem> userInventory = new List<InventoryItem>();
        //// public IEnumerable<IInventoryItem> UserInventory
        //// {
        ////     get
        ////     {
        ////         for (int i = 0; i < this.userInventory.Count; i++)
        ////         {
        ////             yield return this.userInventory[i];
        ////         }
        ////     }
        //// }
        ////
        //// public bool HasInventoryItem(string itemId)
        //// {
        ////     return this.GetInventoryCount(itemId) > 0;
        //// }
    }
}

#endif
