//-----------------------------------------------------------------------
// <copyright file="PlayFabServer.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USE_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using PlayFab;
    using PlayFab.ClientModels;
    using PlayFab.SharedModels;
    using UnityEngine;
    using UnityEngine.Purchasing;

    public delegate void OnVirtualCurrencyChangedDelegate();
    public delegate void OnUserInventoryChagnedDelegate();
    public delegate void OnServerNeedsReloginDelegate();

    public abstract class PlayFabServer<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Singleton Related Code

        private static object instanceLock = new object();
        private static T instance = null;

        public static T Instance
        {
            get
            {
                if (IsInitialized == false)
                {
                    Initialize();
                }

                return instance;
            }
        }

        public static bool IsInitialized
        {
            get { return instance != null; }
        }

        public static void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            // highly unlikely this would need a lock since all unity calls need to happen on the main thread, but hey, being safe
            lock (instanceLock)
            {
                if (IsInitialized)
                {
                    return;
                }

                T[] objects = GameObject.FindObjectsOfType<T>();
                string className = typeof(T).Name;

                if (objects != null && objects.Length != 0)
                {
                    Debug.LogErrorFormat(objects[0], "An object of type {0} already exists and wasn't created with Initialize function.", className);
                }
                else
                {
                    // constructing the singleton object
                    GameObject singleton = new GameObject(className, typeof(T));
                    singleton.transform.SetParent(SingletonUtil.GetSingletonContainer());
                    singleton.transform.Reset();

                    instance = singleton.GetComponent<T>();
                }
            }
        }

        #endregion

        // tracking server time
        private DateTime serverTime = DateTime.MinValue;
        private float serverRealtimeSinceStartup = 0.0f;

        // device id
        private const string DeviceIdKey = "DeviceId";
        private static string deviceId;

        // tracking time outs
        private bool regainFocusCoroutineRunning;
        private DateTime lostFocusTime = DateTime.UtcNow;

        // login related info
        private UserAccountInfo userAccountInfo;
        private bool forceRelogin;

        // inventory
        private List<InventoryItem> userInventory = new List<InventoryItem>();
        private Catalog activeCatalog;

        private Catalog ActiveCatalog
        {
            get
            {
                if (this.activeCatalog == null)
                {
                    this.activeCatalog = Catalogs.Instance.GetCatalog(AppSettings.ActiveConfig.CatalogVersion);
                    Debug.AssertFormat(this.activeCatalog != null, "PlayFabServer.ActiveCatalog Cloudn't find Catalog Version {0}", AppSettings.ActiveConfig.CatalogVersion);
                }

                return this.activeCatalog;
            }
        }

        // virtual currencies
        private Dictionary<string, int> virtualCurrencies = new Dictionary<string, int>();
        private Dictionary<string, int> virtualCurrencyRechargeTimes = new Dictionary<string, int>();

        public event OnVirtualCurrencyChangedDelegate VirtualCurrencyChanged;

        public event OnUserInventoryChagnedDelegate UserInventoryChanged;

        public event OnServerNeedsReloginDelegate ServerNeedsRelogin;

        public virtual GetPlayerCombinedInfoRequestParams LoginCombinedInfoRequestParams
        {
            get
            {
                return new GetPlayerCombinedInfoRequestParams()
                {
                    GetUserVirtualCurrency = true,
                    GetUserInventory = true,
                    GetUserAccountInfo = true,
                };
            }
        }

        public LoginResult LoginResult { get; private set; }

        public DateTime ServerUtcTime
        {
            get { return serverTime != DateTime.MinValue ? serverTime.AddSeconds(Time.realtimeSinceStartup - serverRealtimeSinceStartup) : serverTime; }
        }

        public string PlayerId
        {
            get { return this.userAccountInfo != null ? this.userAccountInfo.PlayFabId : null; }
        }

        public string FacebookId
        {
            get { return this.userAccountInfo != null && this.userAccountInfo.FacebookInfo != null ? this.userAccountInfo.FacebookInfo.FacebookId : null; }
        }

        public bool IsLoggedIn
        {
            get { return forceRelogin == false && PlayFabClientAPI.IsClientLoggedIn(); }
        }

        public bool IsFacebookLinked
        {
            get { return string.IsNullOrEmpty(FacebookId) == false; }
        }

        public bool IsDeviceLinked
        {
            get
            {
                if (Application.isEditor || Platform.IsIosOrAndroid == false)
                {
                    return this.userAccountInfo != null && this.userAccountInfo.CustomIdInfo != null && this.userAccountInfo.CustomIdInfo.CustomId == DeviceId;
                }
                else if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
                {
                    return this.userAccountInfo != null && this.userAccountInfo.IosDeviceInfo != null && this.userAccountInfo.IosDeviceInfo.IosDeviceId == DeviceId;
                }
                else if (Platform.CurrentDevicePlatform == DevicePlatform.Android)
                {
                    return this.userAccountInfo != null && this.userAccountInfo.AndroidDeviceInfo != null && this.userAccountInfo.AndroidDeviceInfo.AndroidDeviceId == DeviceId;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public string DeviceId
        {
            get
            {
                if (string.IsNullOrEmpty(deviceId))
                {
                    string playerPrefsDeviceId = LostPlayerPrefs.GetString(DeviceIdKey, null);

                    if (string.IsNullOrEmpty(playerPrefsDeviceId))
                    {
                        deviceId = Guid.NewGuid().ToString();
                        LostPlayerPrefs.SetString(DeviceIdKey, deviceId);
                        LostPlayerPrefs.Save();
                    }
                    else
                    {
                        deviceId = playerPrefsDeviceId;
                    }
                }

                return deviceId;
            }
        }

        public IEnumerable<IInventoryItem> UserInventory
        {
            get
            {
                for (int i = 0; i < this.userInventory.Count; i++)
                {
                    yield return this.userInventory[i];
                }
            }
        }

        public void ResetDeviceId()
        {
            LostPlayerPrefs.DeleteKey(DeviceIdKey);
            LostPlayerPrefs.Save();
        }
        
        public bool HasInventoryItem(string itemId)
        {
            return this.GetInventoryCount(itemId) > 0;
        }

        public int GetInventoryCount(string itemId)
        {
            var inventoryItem = this.userInventory.FirstOrDefault(x => x.Id == itemId);
            return inventoryItem != null ? inventoryItem.Count : 0;
        }
        
        public int GetVirtualCurrency(string virtualCurrencyId)
        {
            int value;
            if (this.virtualCurrencies.TryGetValue(virtualCurrencyId, out value))
            {
                return value;
            }

            return -1;
        }
        
        public UnityTask<GetUserInventoryResult> RefreshVirtualCurrency()
        {
            return PF.Do(new GetUserInventoryRequest());
        }

        public UnityTask<List<StoreItem>> GetStoreItems(string storeId)
        {
            return UnityTask<List<StoreItem>>.Run(this.GetStoreItemsInternal(storeId));
        }

        public UnityTask<bool> PurchaseStoreItem(StoreItem storeItem, bool showPurchaseItemDialog, Action showStore = null)
        {
            return UnityTask<bool>.Run(this.PurchaseStoreItemInternal(storeItem, showPurchaseItemDialog, showStore));
        }

        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Debug.LogErrorFormat(instance, "Singleton GameObject {0} has been created multiple times!", typeof(T).Name);
            }

            PF.PlayfabEvents.OnLoginResultEvent += PlayfabEvents_OnLoginResultEvent;
            PF.PlayfabEvents.OnGetTimeResultEvent += PlayfabEvents_OnGetTimeResultEvent;
            PF.PlayfabEvents.OnGlobalErrorEvent += PlayfabEvents_OnGlobalErrorEvent;
            PF.PlayfabEvents.OnGetUserInventoryResultEvent += PlayfabEvents_OnGetUserInventoryResultEvent;
        }

        protected void InternalAddVirtualCurrencyToInventory(string virtualCurrencyId, int amountToAdd)
        {
            if (virtualCurrencyId == "RM" || virtualCurrencyId == "AD")
            {
                return;
            }

            if (this.virtualCurrencies.ContainsKey(virtualCurrencyId) == false)
            {
                Debug.LogErrorFormat("Tried to add unknown virtual currency to inventory {0}", virtualCurrencyId);
                return;
            }
            
            this.virtualCurrencies[virtualCurrencyId] += amountToAdd;

            this.OnVirtualCurrencyChanged();
        }

        protected void InternalSetVirtualCurrencyToInventory(string virtualCurrencyId, int neValue)
        {
            if (virtualCurrencyId == "RM" || virtualCurrencyId == "AD")
            {
                return;
            }

            if (this.virtualCurrencies.ContainsKey(virtualCurrencyId) == false)
            {
                Debug.LogErrorFormat("Tried to add unknown virtual currency to inventory {0}", virtualCurrencyId);
                return;
            }

            this.virtualCurrencies[virtualCurrencyId] = neValue;

            this.OnVirtualCurrencyChanged();
        }
        
        protected void InternalAddCatalogItemToInventory(string catalogItemId, int count)
        {
            var inventoryItem = this.userInventory.FirstOrDefault(x => x.Id == catalogItemId);

            if (inventoryItem != null)
            {
                inventoryItem.Count += count;
            }
            else
            {
                this.userInventory.Add(new InventoryItem() { Id = catalogItemId, Count = count });
            }

            this.OnUserInventoryChanged();
        }

        protected void InternalAddBundleItemToInventory(BundleItem bundleItem)
        {
            foreach (var item in bundleItem.Items)
            {
                if (item.Type == BundleItemType.VirtualCurrency)
                {
                    this.InternalAddVirtualCurrencyToInventory(item.Id, item.Count);
                }
                else if (item.Type == BundleItemType.CatalogItem)
                {
                    this.InternalAddCatalogItemToInventory(item.Id, item.Count);
                }
                else
                {
                    Debug.LogErrorFormat("PlayFabServer.InternalAddBundleItemToInventory found unknown BundleItemType {0} on bundle item id {1}", item.Type, bundleItem.Id);
                }
            }
        }

        protected int GetSecondsToRecharge(string virtualCurrencyId)
        {
            if (this.virtualCurrencyRechargeTimes == null)
            {
                return 0;
            }

            int rechargeFinishedTime = 0;

            if (this.virtualCurrencyRechargeTimes.TryGetValue(virtualCurrencyId, out rechargeFinishedTime))
            {
                return Math.Max(0, rechargeFinishedTime - (int)Time.realtimeSinceStartup);
            }

            return 0;
        }

        protected void InternalAddStoreItemToInventory(StoreItem storeItem)
        {
            // deducting the virtual currency
            this.InternalAddVirtualCurrencyToInventory(storeItem.CostCurrencyId, -storeItem.Cost);

            // adding the item to the inventory
            if (storeItem.Type == StoreItemType.BundleItem)
            {
                this.InternalAddBundleItemToInventory(storeItem.BundleItem);
            }
            else if (storeItem.Type == StoreItemType.CatalogItem)
            {
                this.InternalAddCatalogItemToInventory(storeItem.CatalogItem.Id, 1);
            }
            else
            {
                Debug.LogErrorFormat("PlayFabServer.InternalAddStoreItemToInventory found unknown StoreItemType {0} on store item id {1}", storeItem.Type, storeItem.ItemId);
            }
        }
        
        protected void OnUserInventoryChanged()
        {
            if (this.UserInventoryChanged != null)
            {
                this.UserInventoryChanged();
            }
        }

        protected void OnVirtualCurrencyChanged()
        {
            if (this.VirtualCurrencyChanged != null)
            {
                this.VirtualCurrencyChanged();
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                double minutesSinceLostFocus = this.lostFocusTime.Subtract(DateTime.UtcNow).TotalMinutes;

                if (this.regainFocusCoroutineRunning == false && minutesSinceLostFocus > 1)
                {
                    this.regainFocusCoroutineRunning = true;
                    this.StartCoroutine(this.RegainedFocusCoroutine());
                }
            }
            else
            {
                this.lostFocusTime = DateTime.UtcNow;
            }
        }

        private IEnumerator RegainedFocusCoroutine()
        {
            var getServerTime = PF.GetServerTime();

            yield return getServerTime;

            if (this.IsLoggedIn == false)
            {
                this.OnServerNeedsRelogin();
            }

            this.regainFocusCoroutineRunning = false;
        }

        private IEnumerator<bool> PurchaseStoreItemInternal(StoreItem storeItem, bool showPurchaseItemDialog, Action showStore)
        {
            if (storeItem.IsFree)
            {
                var coroutine = PF.ExecuteCloudScript("GrantItemForWatchingAd", new
                {
                    itemId = storeItem.ItemId,
                    catalogVersion = PF.CatalogVersion,
                });

                while (coroutine.keepWaiting)
                {
                    yield return default(bool);
                }

                bool isSuccessful = coroutine.Value != null;

                if (isSuccessful)
                {
                    this.InternalAddStoreItemToInventory(storeItem);
                }

                yield return isSuccessful;
            }
            else if (storeItem.IsIapItem)
            {
                #if UNITY_PURCHASING

                if (UnityPurchasingManager.Instance.IsIAPInitialized == false)
                {
                    var initializeIap = UnityPurchasingManager.Instance.InitializeUnityPurchasing();
                    
                    PlayFabMessages.ShowConnectingToStoreSpinner();

                    while (initializeIap.IsDone == false)
                    {
                        yield return default(bool);
                    }

                    SpinnerBox.Instance.Hide();

                    if (initializeIap.HasError)
                    {
                        PlayFabMessages.HandleError(initializeIap.Exception);
                        yield break;
                    }
                }

                if (showPurchaseItemDialog)
                {
                    var purchaseItemDialog = PurchaseItem.Instance.ShowStoreItem(storeItem, true);

                    while (purchaseItemDialog.IsDone == false)
                    {
                        yield return default(bool);
                    }

                    if (purchaseItemDialog.Value == PurchaseResult.Cancel)
                    {
                        yield break;
                    }
                }

                var iapPurchaseItem = UnityPurchasingManager.Instance.PurchaseProduct(storeItem);

                while (iapPurchaseItem.IsDone == false)
                {
                    yield return default(bool);
                }

                if (iapPurchaseItem.HasError)
                {
                    PlayFabMessages.HandleError(iapPurchaseItem.Exception);
                }
                else
                {
                    if (Debug.isDebugBuild || Application.isEditor)
                    {
                        this.StartCoroutine(this.DebugPurchaseStoreItem(iapPurchaseItem.Value));
                    }
                    else
                    {
                        this.StartCoroutine(this.ProcessPurchaseCoroutine(iapPurchaseItem.Value));
                    }
                }

                #else
                
                throw new NotImplementedException("Trying to buy IAP Store Item when UNITY_PURCHASING is not defined!");
                
                #endif
            }
            else
            {
                bool hasSufficientFunds = this.virtualCurrencies[storeItem.CostCurrencyId] >= storeItem.Cost;
                
                if (showPurchaseItemDialog)
                {
                    var purchaseItemDialog = PurchaseItem.Instance.ShowStoreItem(storeItem, hasSufficientFunds);

                    while (purchaseItemDialog.IsDone == false)
                    {
                        yield return default(bool);
                    }

                    if (purchaseItemDialog.Value == PurchaseResult.Cancel)
                    {
                        yield break;
                    }
                }
                
                if (hasSufficientFunds == false)
                {
                    var messageBoxDialog = PlayFabMessages.ShowInsufficientCurrency();

                    while (messageBoxDialog.IsDone == false)
                    {
                        yield return default(bool);
                    }

                    if (messageBoxDialog.Value == YesNoResult.Yes)
                    {
                        if (showStore != null)
                        {
                            showStore.Invoke();
                        }
                    }
                    
                    yield break;
                }
                
                var coroutine = PF.Do(new PurchaseItemRequest
                {
                    ItemId = storeItem.ItemId,
                    Price = storeItem.Cost,
                    VirtualCurrency = storeItem.CostCurrencyId,
                    StoreId = storeItem.StoreId,
                });

                while (coroutine.keepWaiting)
                {
                    yield return default(bool);
                }

                bool isSuccessful = coroutine.Value != null;

                if (isSuccessful)
                {
                    this.InternalAddStoreItemToInventory(storeItem);
                }

                yield return isSuccessful;
            }
        }

        private IEnumerator<List<StoreItem>> GetStoreItemsInternal(string storeId)
        {
            var getStore = PF.Do(new GetStoreItemsRequest() { StoreId = storeId });

            while (getStore.IsDone == false)
            {
                yield return default(List<StoreItem>);
            }

            List<StoreItem> storeItems = new List<StoreItem>();

            foreach (var playfabStoreItem in getStore.Value.Store)
            {
                var store = this.ActiveCatalog.Stores.FirstOrDefault(x => x.Id == storeId);
                Debug.AssertFormat(store != null, "Store {0} is not defined in the Catalog", storeId);

                var storeItem = store.StoreItems.FirstOrDefault(x => x.ItemId == playfabStoreItem.ItemId);
                Debug.AssertFormat(storeItem != null, "StoreItem {0} is not in Store {1}", playfabStoreItem.ItemId, storeId);

                var catalogItem = this.ActiveCatalog.CatalogItems.FirstOrDefault(x => x.Id == playfabStoreItem.ItemId);
                var bundleItem = this.ActiveCatalog.BundleItems.FirstOrDefault(x => x.Id == playfabStoreItem.ItemId);
                Debug.AssertFormat(catalogItem != null | bundleItem != null, "StoreItem {0} in Store {1} Doesn't have a valid catalog/bundle id", playfabStoreItem.ItemId, storeId);

                // calculating teh store item cost
                int storeItemCost = 0;

                if (storeItem.CostCurrencyId != "AD")
                {
                    if (playfabStoreItem.VirtualCurrencyPrices.ContainsKey(storeItem.CostCurrencyId) == false)
                    {
                        Debug.LogErrorFormat("StoreItem {0} Should have Virtual Currency {0}, but it's not specified.", storeItem.ItemId, storeItem.CostCurrencyId);
                    }
                    else
                    {
                        storeItemCost = (int)playfabStoreItem.VirtualCurrencyPrices[storeItem.CostCurrencyId];
                    }
                }

                if (storeItem.IsFree == false)
                {
                    Debug.AssertFormat(playfabStoreItem.VirtualCurrencyPrices.ContainsKey(storeItem.CostCurrencyId), "StoreItem {0} in Store {1} doesn't contain Virtual Currency Id {2}", playfabStoreItem.ItemId, storeId, storeItem.CostCurrencyId);
                }

                if (storeItem.Type == StoreItemType.CatalogItem)
                {
                    Debug.AssertFormat(catalogItem != null, "");
                }
                else if (storeItem.Type == StoreItemType.BundleItem)
                {
                    Debug.AssertFormat(bundleItem != null, "");
                }

                storeItems.Add(new StoreItem()
                {
                    StoreId = storeId,
                    Type = storeItem.Type,
                    ItemId = playfabStoreItem.ItemId,
                    CostCurrencyId = storeItem.CostCurrencyId,
                    Cost = storeItemCost,
                    CatalogItem = catalogItem,
                    BundleItem = bundleItem,
                    PurchaseDescription = storeItem.PurchaseDescription,
                    PurchaseIcon = storeItem.PurchaseIcon,
                });
            }

            yield return storeItems;
        }

        private void PlayfabEvents_OnLoginResultEvent(LoginResult result)
        {
            var payload = result.InfoResultPayload;

            this.forceRelogin = false;
            this.LoginResult = result;
            this.userAccountInfo = payload.AccountInfo;
            this.UpdateUserInventory(payload.UserInventory);
            this.UpdateVirtualCurrencies(payload.UserVirtualCurrency, payload.UserVirtualCurrencyRechargeTimes);
        }

        private void PlayfabEvents_OnGetUserInventoryResultEvent(GetUserInventoryResult result)
        {
            this.UpdateUserInventory(result.Inventory);
            this.UpdateVirtualCurrencies(result.VirtualCurrency, result.VirtualCurrencyRechargeTimes);
        }
        
        private void PlayfabEvents_OnGetTimeResultEvent(GetTimeResult result)
        {
            serverRealtimeSinceStartup = Time.realtimeSinceStartup;
            serverTime = result.Time;
        }

        private void PlayfabEvents_OnGlobalErrorEvent(PlayFabRequestCommon request, PlayFabError error)
        {
            if (error.Error == PlayFabErrorCode.InvalidSessionTicket)
            {
                this.forceRelogin = true;
            }
        }

        private void UpdateVirtualCurrencies(Dictionary<string, int> virtualCurrencies, Dictionary<string, VirtualCurrencyRechargeTime> rechargeTimes)
        {
            this.virtualCurrencies = virtualCurrencies;
            this.virtualCurrencyRechargeTimes = new Dictionary<string, int>();

            foreach (var vc in rechargeTimes.Keys)
            {
                if (rechargeTimes.ContainsKey(vc))
                {
                    this.virtualCurrencyRechargeTimes.Add(vc, (int)(Time.realtimeSinceStartup + rechargeTimes[vc].SecondsToRecharge + 1));
                }
            }

            this.OnVirtualCurrencyChanged();
        }

        private void UpdateUserInventory(List<ItemInstance> userInventory)
        {
            this.userInventory.Clear();

            foreach (ItemInstance item in userInventory)
            {
                this.userInventory.Add(new InventoryItem()
                {
                    Id = item.ItemId,
                    Count = item.RemainingUses.HasValue ? item.RemainingUses.Value : 1,
                    Data = item.CustomData,
                    InstanceId = item.ItemInstanceId,
                });
            }

            this.OnUserInventoryChanged();
        }
        
        private void OnServerNeedsRelogin()
        {
            if (this.ServerNeedsRelogin != null)
            {
                this.ServerNeedsRelogin();
            }
        }
                
        private IEnumerator ProcessPurchaseCoroutine(PurchaseEventArgs e)
        {
            Debug.AssertFormat(e.purchasedProduct.hasReceipt, "Purchased item {0} doesn't have a receipt", e.purchasedProduct.definition.id);

            var catalogItem = this.ActiveCatalog.CatalogItems.FirstOrDefault(x => x.Id == e.purchasedProduct.definition.id);
            var bundleItem = this.ActiveCatalog.BundleItems.FirstOrDefault(x => x.Id == e.purchasedProduct.definition.id);

            Debug.AssertFormat(catalogItem != null || bundleItem != null, "Couln't find BundleItem or CatalogItem {0}", e.purchasedProduct.definition.id);

            var receipt = (Dictionary<string, object>)Lost.MiniJSON.Json.Deserialize(e.purchasedProduct.receipt);
            Debug.AssertFormat(receipt != null, "Unable to parse receipt {0}", e.purchasedProduct.receipt);

            var store = (string)receipt["Store"];
            var transactionID = (string)receipt["TransactionID"];
            var payload = (string)receipt["Payload"];
            
            Debug.AssertFormat(string.IsNullOrEmpty(store) == false, "Unable to parse store from {0}", e.purchasedProduct.receipt);
            Debug.AssertFormat(string.IsNullOrEmpty(transactionID) == false, "Unable to parse transactionID from {0}", e.purchasedProduct.receipt);
            Debug.AssertFormat(string.IsNullOrEmpty(payload) == false, "Unable to parse payload from {0}", e.purchasedProduct.receipt);

            bool wasSuccessfull = false;

            if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
            {
                var validate = PF.Do(new ValidateIOSReceiptRequest()
                {
                    CurrencyCode = e.purchasedProduct.metadata.isoCurrencyCode,
                    PurchasePrice = (int)(e.purchasedProduct.metadata.localizedPrice * 100),
                    ReceiptData = payload,
                });

                yield return validate;

                if (validate.HasError)
                {
                    Debug.LogErrorFormat("Unable to validate iOS IAP Purchase: {0}", validate.Exception.ToString());
                    PlayFabMessages.HandleError(validate.Exception);
                }
                else
                {
                    wasSuccessfull = true;
                }
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.Android)
            {
                var details = (Dictionary<string, object>)Lost.MiniJSON.Json.Deserialize(payload);
                Debug.AssertFormat(details != null, "Unable to parse Receipt Payload {0}", payload);

                Debug.AssertFormat(details.ContainsKey("json"), "Receipt Payload doesn't have \"json\" key: {0}", payload);
                Debug.AssertFormat(details.ContainsKey("signature"), "Receipt Payload doesn't have \"signature\" key: {0}", payload);

                var receiptJson = (string)details["json"];
                var signature = (string)details["signature"];
                
                var validate = PF.Do(new ValidateGooglePlayPurchaseRequest()
                {
                    CurrencyCode = e.purchasedProduct.metadata.isoCurrencyCode,
                    PurchasePrice = (uint)(e.purchasedProduct.metadata.localizedPrice * 100),
                    ReceiptJson = receiptJson,
                    Signature = signature,
                });

                yield return validate;

                if (validate.HasError)
                {
                    Debug.LogErrorFormat("Unable to validate Google Play IAP Purchase: {0}", validate.Exception.ToString());
                    PlayFabMessages.HandleError(validate.Exception);
                }
                else
                {
                    wasSuccessfull = true;
                }
            }
            else
            {
                Debug.LogErrorFormat(this, "Tried to make IAP Purchase on Unknown Platform {0}", Application.platform.ToString());
            }

            if (wasSuccessfull == false)
            {
                yield break;
            }

            AnalyticsManager.Instance.Transaction(e.purchasedProduct.definition.id, e.purchasedProduct.metadata.localizedPrice, e.purchasedProduct.metadata.isoCurrencyCode);

            if (bundleItem != null)
            {
                this.InternalAddBundleItemToInventory(bundleItem);
            }
            else if (catalogItem != null)
            {
                this.InternalAddCatalogItemToInventory(catalogItem.Id, 1);
            }
        }
        
        private IEnumerator DebugPurchaseStoreItem(PurchaseEventArgs e)
        {
            var coroutine = PF.ExecuteCloudScript("DebugPurchaseItem", new
            {
                itemId = e.purchasedProduct.definition.id,
                catalogVersion = PF.CatalogVersion,
            });
               
            while (coroutine.keepWaiting)
            {
                yield return null;
            }

            if (coroutine.Value != null)
            {
                var bundleItem = this.ActiveCatalog.BundleItems.FirstOrDefault(x => x.Id == e.purchasedProduct.definition.id);
                var catalogItem = this.ActiveCatalog.CatalogItems.FirstOrDefault(x => x.Id == e.purchasedProduct.definition.id);

                if (bundleItem != null)
                {
                    this.InternalAddBundleItemToInventory(bundleItem);
                }
                else if (catalogItem != null)
                {
                    this.InternalAddCatalogItemToInventory(catalogItem.Id, 1);
                }
                else
                {
                    Debug.LogErrorFormat("DebugPurchaseStoreItem found unknown bundle/catalog id {0}", e.purchasedProduct.definition.id);
                }
            }
        }
        
        private class InventoryItem : IInventoryItem
        {
            public string Id { get; set; }

            public int Count { get; set; }

            public Dictionary<string, string> Data { get; set; }

            public string InstanceId { get; set; }
        }
    }
}

#endif
