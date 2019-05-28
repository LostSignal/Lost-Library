//-----------------------------------------------------------------------
// <copyright file="PurchasingHelper.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Lost.AppConfig;
    using PlayFab;
    using PlayFab.ClientModels;
    using UnityEngine;

    #if USING_UNITY_PURCHASING && !UNITY_XBOXONE
    using UnityEngine.Purchasing;
    #endif

    using UnityEngine.SceneManagement;

    public class PurchasingHelper
    {
        private bool isInitializationRunning;

        public bool IsInitialized { get; private set; }

        public UnityTask<bool> Initialize()
        {
            return UnityTask<bool>.Run(this.InitializeCoroutine());
        }

        private IEnumerator<bool> InitializeCoroutine()
        {
            if (this.IsInitialized)
            {
                yield return true;
                yield break;
            }

            // If it's already running, then wait for it to finish and leave
            if (this.isInitializationRunning)
            {
                while (this.isInitializationRunning)
                {
                    yield return default(bool);
                }

                yield return this.IsInitialized;
                yield break;
            }

            this.isInitializationRunning = true;

            // TODO [bgish]: Run initialization code
            var getCatalog = PF.Catalog.GetCatalog();

            while (getCatalog.IsDone == false)
            {
                yield return default(bool);
            }

            #if USING_UNITY_PURCHASING && !UNITY_XBOXONE
            // initializing purchasing, but no need to wait on it
            if (getCatalog.HasError == false && IAP.UnityPurchasingManager.Instance.IsIAPInitialized == false)
            {
                var initializeUnityIap = IAP.UnityPurchasingManager.Instance.InitializeUnityPurchasing((store, builder) =>
                {
                    foreach (var catalogItem in getCatalog.Value)
                    {
                        if (catalogItem.GetVirtualCurrenyPrice("RM") > 0)
                        {
                            builder.AddProduct(catalogItem.ItemId, this.GetProductType(catalogItem));
                        }
                    }
                });

                // Waiting for initialization to finish
                while (initializeUnityIap.IsDone == false)
                {
                    yield return default(bool);
                }
            }
            #endif

            this.isInitializationRunning = false;

            #if USING_UNITY_PURCHASING && !UNITY_XBOXONE
            this.IsInitialized = getCatalog.HasError == false && Lost.IAP.UnityPurchasingManager.IsInitialized;
            #else
            this.IsInitialized = getCatalog.HasError == false;
            #endif

            yield return this.IsInitialized;
        }

        #if USING_UNITY_PURCHASING && !UNITY_XBOXONE
        private ProductType GetProductType(CatalogItem catalogItem)
        {
            return catalogItem.Consumable != null ?
                UnityEngine.Purchasing.ProductType.Consumable :
                UnityEngine.Purchasing.ProductType.NonConsumable;
        }
        #endif

        public UnityTask<bool> PurchaseStoreItem(string storeId, StoreItem storeItem, bool showPurchaseItemDialog, Action showStore = null)
        {
            return UnityTask<bool>.Run(this.PurchaseStoreItemInternal(storeId, storeItem, showPurchaseItemDialog, showStore));
        }

        private IEnumerator<bool> PurchaseStoreItemInternal(string storeId, StoreItem storeItem, bool showPurchaseItemDialog, Action showStore)
        {
            bool isIapItem = storeItem.GetVirtualCurrenyPrice("RM") > 0;

            if (isIapItem)
            {
                #if USING_UNITY_PURCHASING && !UNITY_XBOXONE

                // Making sure we're properly initialized
                if (this.IsInitialized == false)
                {
                    var initialize = this.Initialize();

                    while (initialize.IsDone == false)
                    {
                        yield return default(bool);
                    }

                    // Early out if initialization failed
                    if (initialize.HasError || this.IsInitialized == false)
                    {
                        if (initialize.HasError)
                        {
                            PlayFabMessages.HandleError(initialize.Exception);
                        }

                        yield return false;
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

                var iapPurchaseItem = Lost.IAP.UnityPurchasingManager.Instance.PurchaseProduct(storeItem.ItemId);

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
                        var purchase = UnityTask<bool>.Run(this.DebugPurchaseStoreItem(iapPurchaseItem.Value));

                        while (purchase.IsDone == false)
                        {
                            yield return default(bool);
                        }

                        yield return purchase.Value;
                    }
                    else
                    {
                        var purchase = UnityTask<bool>.Run(this.ProcessPurchaseCoroutine(iapPurchaseItem.Value, storeId));

                        while (purchase.IsDone == false)
                        {
                            yield return default(bool);
                        }

                        yield return purchase.Value;
                    }
                }

                #else

                throw new NotImplementedException("Trying to buy IAP Store Item when USING_UNITY_PURCHASING is not defined!");

                #endif
            }
            else
            {
                string virtualCurrency = storeItem.GetVirtualCurrencyId();
                int storeItemCost = storeItem.GetCost(virtualCurrency);

                bool hasSufficientFunds = PF.VC[virtualCurrency] >= storeItemCost;

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

                var coroutine = this.Do(new PurchaseItemRequest
                {
                    ItemId = storeItem.ItemId,
                    Price = storeItemCost,
                    VirtualCurrency = virtualCurrency,
                    StoreId = storeId,
                });

                while (coroutine.keepWaiting)
                {
                    yield return default(bool);
                }

                bool isSuccessful = coroutine.Value != null;

                if (isSuccessful)
                {
                    PF.Inventory.InternalAddStoreItemToInventory(storeItem);
                }

                yield return isSuccessful;
            }
        }

        #if USING_UNITY_PURCHASING && !UNITY_XBOXONE

        private int GetPurchasePrice(PurchaseEventArgs e)
        {
            int fractionalUnit;

            switch (e.purchasedProduct.metadata.isoCurrencyCode)
            {
                // https://en.wikipedia.org/wiki/List_of_circulating_currencies
                //
                // case "BHD": // Bahrain
                // case "IQD": // Iraq
                // case "KWD": // Kuwait
                // case "LYD": // Libya
                // case "OMR": // Oman
                // case "TND": // Tunisia
                //     {
                //         fractionalUnit = 1000;
                //         break;
                //     }
                //
                // case "MGA": // Madagascar
                // case "MRU": // Mauritania, Sahrawi Republic (Mauritanian ouguiya)
                //     {
                //         fractionalUnit = 5;
                //         break;
                //     }
                //
                // case "VUV": // Vanuatu
                //     {
                //         fractionalUnit = 1;
                //         break;
                //     }
                //
                // NOTE [bgish]: May need this for IAP to work in Vietname
                // case "VND": // Vietname
                //     {
                //         fractionalUnit = 10;
                //         break;
                //     }

                default:
                    {
                        fractionalUnit = 100;
                        break;
                    }
            }

            return (int)(e.purchasedProduct.metadata.localizedPrice * fractionalUnit);
        }

        private IEnumerator<bool> ProcessPurchaseCoroutine(PurchaseEventArgs e, string storeId)
        {
            var catalogItem = PF.Catalog.GetCatalogItem(e.purchasedProduct.definition.id);
            Debug.AssertFormat(catalogItem != null, "Couln't find CatalogItem {0}", e.purchasedProduct.definition.id);

            Debug.AssertFormat(e.purchasedProduct.hasReceipt, "Purchased item {0} doesn't have a receipt", e.purchasedProduct.definition.id);
            var receipt = (Dictionary<string, object>)Lost.AppConfig.MiniJSON.Json.Deserialize(e.purchasedProduct.receipt);
            Debug.AssertFormat(receipt != null, "Unable to parse receipt {0}", e.purchasedProduct.receipt);

            var store = (string)receipt["Store"];
            var transactionId = (string)receipt["TransactionID"];
            var payload = (string)receipt["Payload"];

            Debug.AssertFormat(string.IsNullOrEmpty(store) == false, "Unable to parse store from {0}", e.purchasedProduct.receipt);
            Debug.AssertFormat(string.IsNullOrEmpty(transactionId) == false, "Unable to parse transactionID from {0}", e.purchasedProduct.receipt);
            Debug.AssertFormat(string.IsNullOrEmpty(payload) == false, "Unable to parse payload from {0}", e.purchasedProduct.receipt);

            var currencyCode = e.purchasedProduct.metadata.isoCurrencyCode;
            var purchasePrice = this.GetPurchasePrice(e);
            bool wasSuccessfull = false;

            if (Platform.CurrentDevicePlatform == DevicePlatform.iOS)
            {
                var validate = PF.Do(new ValidateIOSReceiptRequest()
                {
                    CurrencyCode = currencyCode,
                    PurchasePrice = purchasePrice,
                    ReceiptData = payload,
                });

                while (validate.IsDone == false)
                {
                    yield return default(bool);
                }

                if (validate.HasError)
                {
                    Debug.LogErrorFormat("Unable to validate iOS IAP Purchase: {0}", validate?.Exception?.ToString());
                    this.LogErrorReceiptFailedInfo(catalogItem, e);
                    Debug.LogErrorFormat("ReceiptData = " + payload);

                    PlayFabMessages.HandleError(validate.Exception);
                }
                else
                {
                    wasSuccessfull = true;
                }
            }
            else if (Platform.CurrentDevicePlatform == DevicePlatform.Android)
            {
                var details = (Dictionary<string, object>)Lost.AppConfig.MiniJSON.Json.Deserialize(payload);
                Debug.AssertFormat(details != null, "Unable to parse Receipt Payload {0}", payload);

                Debug.AssertFormat(details.ContainsKey("json"), "Receipt Payload doesn't have \"json\" key: {0}", payload);
                Debug.AssertFormat(details.ContainsKey("signature"), "Receipt Payload doesn't have \"signature\" key: {0}", payload);

                var receiptJson = (string)details["json"];
                var signature = (string)details["signature"];

                var validate = PF.Do(new ValidateGooglePlayPurchaseRequest()
                {
                    CurrencyCode = currencyCode,
                    PurchasePrice = (uint)purchasePrice,
                    ReceiptJson = receiptJson,
                    Signature = signature,
                });

                while (validate.IsDone == false)
                {
                    yield return default(bool);
                }

                if (validate.HasError)
                {
                    Debug.LogErrorFormat("Unable to validate Google Play IAP Purchase: {0}", validate?.Exception?.ToString());
                    this.LogErrorReceiptFailedInfo(catalogItem, e);
                    Debug.LogErrorFormat("Receipt Json = " + receiptJson);
                    Debug.LogErrorFormat("Signature = " + signature);

                    PlayFabMessages.HandleError(validate.Exception);
                }
                else
                {
                    wasSuccessfull = true;
                }
            }
            else
            {
                Debug.LogErrorFormat("Tried to make IAP Purchase on Unknown Platform {0}", Application.platform.ToString());
            }

            if (wasSuccessfull == false)
            {
                yield break;
            }

            float price = catalogItem.GetVirtualCurrenyPrice("RM") / 100.0f;
            string itemId = e.purchasedProduct.definition.id;
            string itemType = catalogItem.ItemClass;
            string level = SceneManager.GetActiveScene().name;

            Analytics.AnalyticsEvent.IAPTransaction(storeId, price, itemId, itemType, level, transactionId, new Dictionary<string, object>
            {
                { "localized_price", e.purchasedProduct.metadata.localizedPrice },
                { "iso_currency_code", e.purchasedProduct.metadata.isoCurrencyCode },
                { "store", store },
            });

            PF.Inventory.InternalAddCatalogItemToInventory(catalogItem);

            yield return true;
        }

        private void LogErrorReceiptFailedInfo(CatalogItem catalogItem, PurchaseEventArgs e)
        {
            StringBuilder itemDataBuilder = new StringBuilder();
            itemDataBuilder.Append("Item Id = ");
            itemDataBuilder.Append(catalogItem?.ItemId);
            itemDataBuilder.AppendLine();
            itemDataBuilder.Append("Item RM = ");
            itemDataBuilder.Append(catalogItem?.GetVirtualCurrenyPrice("RM"));
            itemDataBuilder.AppendLine();
            itemDataBuilder.Append("Currency Code = ");
            itemDataBuilder.Append(e.purchasedProduct.metadata.isoCurrencyCode);
            itemDataBuilder.AppendLine();
            itemDataBuilder.Append("Localized Price = ");
            itemDataBuilder.Append(e.purchasedProduct.metadata.localizedPrice);
            itemDataBuilder.AppendLine();
            itemDataBuilder.Append("Purchase Price = ");
            itemDataBuilder.Append(this.GetPurchasePrice(e));

            Debug.LogError(itemDataBuilder.ToString());
        }

        private IEnumerator<bool> DebugPurchaseStoreItem(PurchaseEventArgs e)
        {
            var coroutine = PF.CloudScript.Execute("DebugPurchaseItem", new
            {
                itemId = e.purchasedProduct.definition.id,
                catalogVersion = PF.CatalogVersion,
            });

            while (coroutine.keepWaiting)
            {
                yield return default(bool);
            }

            if (coroutine.Value != null)
            {
                var catalogItem = PF.Catalog.GetCatalogItem(e.purchasedProduct.definition.id);

                if (catalogItem != null)
                {
                    PF.Inventory.InternalAddCatalogItemToInventory(catalogItem);
                    yield return true;
                }
                else
                {
                    Debug.LogErrorFormat("DebugPurchaseStoreItem found unknown ItemId {0}", e.purchasedProduct.definition.id);
                }
            }
        }

        #endif

        private UnityTask<PurchaseItemResult> Do(PurchaseItemRequest request)
        {
            if (string.IsNullOrEmpty(request.CatalogVersion))
            {
                request.CatalogVersion = PF.CatalogVersion;
            }

            return PF.Do<PurchaseItemRequest, PurchaseItemResult>(request, PlayFabClientAPI.PurchaseItem);
        }
    }
}

#endif
