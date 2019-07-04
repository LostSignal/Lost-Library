//--------------------------------------------------------------------s---
// <copyright file="PurchaseItem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using PlayFab.ClientModels;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    
    public enum PurchaseResult
    {
        Cancel,
        Buy,
    }

    public class PurchaseItem : SingletonDialogResource<PurchaseItem>
    {
        #pragma warning disable 0649
        [Header("Purchase Item")]
        [SerializeField] private Button cancelButton;

        [Header("Item")]
        [SerializeField] private Image storeItemIcon;
        [SerializeField] private TMP_Text storeItemTitle;
        [SerializeField] private TMP_Text storeItemDescription;

        [Header("Virtual Currency")]
        [SerializeField] private Button virtualCurrencyBuyButton;
        [SerializeField] private TMP_Text virtualCurrencyBuyButtonText;
        [SerializeField] private Image virtualCurrencyBuyButtonIcon;

        [Header("IAP")]
        [SerializeField] private Button iapBuyButton;
        [SerializeField] private TMP_Text iapBuyButtonText;

        [Header("Virtual Currencies")]
        [SerializeField] private VirtualCurrencyIcon[] virtualCurrencyIcons;
        #pragma warning restore 0649

        private string storeId;
        private StoreItem storeItem;
        private PurchaseResult result;
        private bool isCoroutineRunning;
        private bool automaticallyPerformPurchase;

        public UnityTask<PurchaseResult> ShowStoreItem(bool automaticallyPerformPurchase, string storeId, StoreItem storeItem, Sprite icon, string title, string description, Action insufficientFundsStore = null)
        {
            this.automaticallyPerformPurchase = automaticallyPerformPurchase;

            return UnityTask<PurchaseResult>.Run(Coroutine());

            IEnumerator<PurchaseResult> Coroutine()
            {
                // resetting the result, and caching the items
                this.result = PurchaseResult.Cancel;
                this.storeId = storeId;
                this.storeItem = storeItem;

                // Figuring out which currecy this item costs
                string virtualCurrencyId = null;

                foreach (var virtualCurrencyPrice in storeItem.VirtualCurrencyPrices)
                {
                    if (virtualCurrencyPrice.Value > 0)
                    {
                        virtualCurrencyId = virtualCurrencyPrice.Key;
                    }
                }

                if (virtualCurrencyId == null)
                {
                    Debug.LogErrorFormat("StoreItem {0} has unknown currency.", storeItem.ItemId);
                    yield return PurchaseResult.Cancel;
                    yield break;
                }

                bool isIapItem = virtualCurrencyId == "RM";

                // Turning on the correct button
                this.iapBuyButton.gameObject.SafeSetActive(isIapItem);
                this.virtualCurrencyBuyButton.gameObject.SafeSetActive(!isIapItem);

                if (isIapItem)
                {
                    string purchasePrice = string.Empty;

                    #if USING_UNITY_PURCHASING && !UNITY_XBOXONE
                    purchasePrice = IAP.UnityPurchasingManager.Instance.GetLocalizedPrice(storeItem.ItemId);
                    #else
                    purchasePrice = string.Format("${0}.{1:D2}", currencyPrice / 100, currencyPrice % 100);
                    #endif

                    this.iapBuyButtonText.text = purchasePrice;
                }
                else
                {
                    int virtualCurrencyPrice = storeItem.GetVirtualCurrenyPrice(virtualCurrencyId);
                    bool hasSufficientFunds = PF.VC[virtualCurrencyId] >= virtualCurrencyPrice;

                    if (hasSufficientFunds == false && insufficientFundsStore != null)
                    {
                        var insufficientFundsMessage = PlayFabMessages.ShowInsufficientCurrency();

                        while (insufficientFundsMessage.IsDone == false)
                        {
                            yield return default(PurchaseResult);
                        }

                        if (insufficientFundsMessage.Value == YesNoResult.Yes)
                        {
                            insufficientFundsStore.Invoke();
                        }
                        else
                        {
                            yield return PurchaseResult.Cancel;
                            yield break;
                        }
                    }

                    this.virtualCurrencyBuyButton.interactable = hasSufficientFunds;
                    this.virtualCurrencyBuyButtonIcon.sprite = this.GetSprite(virtualCurrencyId);
                    this.virtualCurrencyBuyButtonText.text = virtualCurrencyPrice.ToString();
                }

                // Setting the item image/texts
                this.storeItemIcon.sprite = icon;
                this.storeItemTitle.text = title;
                this.storeItemDescription.text = description;

                this.Dialog.Show();

                // waiting for it to start showing
                while (this.Dialog.IsShowing == false)
                {
                    yield return default(PurchaseResult);
                }

                // waiting for it to return to the hidden state
                while (this.Dialog.IsHidden == false)
                {
                    yield return default(PurchaseResult);
                }

                yield return this.result;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(this.cancelButton != null, "PurchaseItem didn't define cancel button", this);
            Debug.Assert(this.storeItemIcon != null, "PurchaseItem didn't define StoreItem icon", this);
            Debug.Assert(this.storeItemTitle != null, "PurchaseItem didn't define StoreItem title", this);
            Debug.Assert(this.storeItemDescription != null, "PurchaseItem didn't define StoreItem description", this);

            Debug.Assert(this.virtualCurrencyBuyButton != null, "PurchaseItem didn't define virtual currency buy button", this);
            Debug.Assert(this.virtualCurrencyBuyButtonText != null, "PurchaseItem didn't define virtual currency buy button text", this);
            Debug.Assert(this.virtualCurrencyBuyButtonIcon != null, "PurchaseItem didn't define buy virtual currency button icon", this);

            Debug.Assert(this.iapBuyButton != null, "PurchaseItem didn't define iap buy button", this);
            Debug.Assert(this.iapBuyButtonText != null, "PurchaseItem didn't define iap buy button text", this);

            this.virtualCurrencyBuyButton.onClick.AddListener(this.BuyButtonClicked);
            this.iapBuyButton.onClick.AddListener(this.BuyButtonClicked);
            this.cancelButton.onClick.AddListener(this.CancelButtonClicked);
        }

        protected override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            this.BuyButtonClicked();
        }

        private void CancelButtonClicked()
        {
            this.result = PurchaseResult.Cancel;
            this.Dialog.Hide();
        }

        private void BuyButtonClicked()
        {
            this.result = PurchaseResult.Buy;

            if (this.automaticallyPerformPurchase)
            {
                PF.Purchasing.PurchaseStoreItem(this.storeId, this.storeItem);
            }            

            this.Dialog.Hide();
        }

        private Sprite GetSprite(string virtualCurrencyId)
        {
            return this.virtualCurrencyIcons.First(x => x.Id == virtualCurrencyId).Icon;
        }

        [Serializable]
        private class VirtualCurrencyIcon
        {
            #pragma warning disable 0649
            [SerializeField] private string id;
            [SerializeField] private Sprite icon;
            #pragma warning restore 0649

            public string Id
            {
                get { return this.id; }
            }

            public Sprite Icon
            {
                get { return this.icon; }
            }
        }
    }
}

#endif
