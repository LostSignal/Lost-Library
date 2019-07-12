//-----------------------------------------------------------------------
// <copyright file="PF.Do.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System.Collections.Generic;
    using PlayFab;
    using PlayFab.ClientModels;

    public class CatalogHelper
    {
        private GetCatalogItemsRequest getCatalogRequest = new GetCatalogItemsRequest();
        private GetStoreItemsRequest getStoreRequest = new GetStoreItemsRequest();

        private Dictionary<string, CatalogItem> catalogItemDictionary = new Dictionary<string, CatalogItem>();
        private Dictionary<string, List<StoreItem>> cachedStores = new Dictionary<string, List<StoreItem>>();
        private List<CatalogItem> cachedCatalog;

        public UnityTask<List<CatalogItem>> GetCatalog()
        {
            if (this.cachedCatalog != null)
            {
                return UnityTask<List<CatalogItem>>.Run(this.GetCachedCatalog());
            }
            else
            {
                return UnityTask<List<CatalogItem>>.Run(this.FetchCatalog());
            }
        }

        public CatalogItem GetCatalogItem(string itemId)
        {
            if (catalogItemDictionary.TryGetValue(itemId, out CatalogItem item))
            {
                return item;
            }

            return null;
        }

        public UnityTask<List<StoreItem>> GetStore(string storeId, bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                this.cachedStores.Remove(storeId);
            }

            if (this.cachedStores.ContainsKey(storeId))
            {
                return UnityTask<List<StoreItem>>.Run(this.GetCachedStore(storeId));
            }
            else
            {
                return UnityTask<List<StoreItem>>.Run(this.FetchStore(storeId));
            }
        }

        private IEnumerator<List<CatalogItem>> GetCachedCatalog()
        {
            yield return this.cachedCatalog;
        }

        private IEnumerator<List<CatalogItem>> FetchCatalog()
        {
            if (string.IsNullOrEmpty(getCatalogRequest.CatalogVersion))
            {
                getCatalogRequest.CatalogVersion = PF.CatalogVersion;
            }

            var getCatalog = PF.Do<GetCatalogItemsRequest, GetCatalogItemsResult>(getCatalogRequest, PlayFabClientAPI.GetCatalogItems);

            while (getCatalog.IsDone == false)
            {
                yield return null;
            }

            // Caching off the store in case the user requests it again
            this.cachedCatalog = getCatalog.Value?.Catalog;

            if (this.cachedCatalog != null)
            {
                foreach (var item in this.cachedCatalog)
                {
                    this.catalogItemDictionary.Add(item.ItemId, item);
                }
            }

            yield return this.cachedCatalog;
        }

        private IEnumerator<List<StoreItem>> GetCachedStore(string storeId)
        {
            yield return this.cachedStores[storeId];
        }

        private IEnumerator<List<StoreItem>> FetchStore(string storeId)
        {
            if (string.IsNullOrEmpty(getStoreRequest.CatalogVersion))
            {
                getStoreRequest.CatalogVersion = PF.CatalogVersion;
            }

            getStoreRequest.StoreId = storeId;

            var getStore = PF.Do<GetStoreItemsRequest, GetStoreItemsResult>(getStoreRequest, PlayFabClientAPI.GetStoreItems);

            while (getStore.IsDone == false)
            {
                yield return null;
            }

            var store = getStore.Value?.Store;

            // Caching off the store in case the user requests it again
            if (store != null)
            {
                this.cachedStores.Add(storeId, store);
            }

            yield return store;
        }
    }
}

#endif
