//--------------------------------------------------------------------s---
// <copyright file="PurchaseItem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using PlayFab.ClientModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    using Text = TMPro.TMP_Text;

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
        [SerializeField] private Button buyButton;
        [SerializeField] private Text buyButtonText;
        [SerializeField] private Image buyButtonIcon;
        [SerializeField] private Image storeItemIcon;
        [SerializeField] private Text storeItemDescription;

        [Header("Icons")]
        [SerializeField] private Color buyButtonTextColor = new Color32(87, 44, 43, 255);
        [SerializeField] private Color buyButtonTextColorInsufficient = new Color32(255, 0, 0, 255);
        [SerializeField] private VirtualCurrencyIcon[] virtualCurrencyIcons;
        #pragma warning restore 0649

        private PurchaseResult result;

        public UnityTask<PurchaseResult> ShowStoreItem(StoreItem storeItem, bool hasSufficientFunds)
        {
            // // resetting the result
            // this.result = PurchaseResult.Cancel;
            //
            // this.buyButtonText.text = storeItem.IsIapItem ? storeItem.LocalizedString : storeItem.Cost.ToString();
            // this.buyButtonText.color = hasSufficientFunds ? this.buyButtonTextColor : this.buyButtonTextColorInsufficient;
            // this.buyButtonIcon.gameObject.SetActive(storeItem.IsIapItem == false);
            //
            // if (storeItem.IsIapItem == false)
            // {
            //     this.buyButtonIcon.sprite = this.virtualCurrencyIcons.First(x => x.Id == storeItem.CostCurrencyId).Icon;
            // }
            //
            // this.storeItemIcon.enabled = storeItem.PurchaseIcon != null;
            // this.storeItemIcon.sprite = storeItem.PurchaseIcon;
            // this.storeItemDescription.text = storeItem.PurchaseDescription;
            //
            // return UnityTask<PurchaseResult>.Run(this.ShowInternal());

            return null;
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(this.cancelButton != null, "PurchaseItem didn't define cancel button", this);
            Debug.Assert(this.buyButton != null, "PurchaseItem didn't define buy button", this);
            Debug.Assert(this.buyButtonText != null, "PurchaseItem didn't define buy button text", this);
            Debug.Assert(this.buyButtonIcon != null, "PurchaseItem didn't define buy button icon", this);
            Debug.Assert(this.storeItemIcon != null, "PurchaseItem didn't define StoreItem icon", this);
            Debug.Assert(this.storeItemDescription != null, "PurchaseItem didn't define StoreITem description", this);

            this.buyButton.onClick.AddListener(this.BuyButtonClicked);
            this.cancelButton.onClick.AddListener(this.CancelButtonClicked);
        }

        protected override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            this.BuyButtonClicked();
        }

        private IEnumerator<PurchaseResult> ShowInternal()
        {
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

        private void CancelButtonClicked()
        {
            this.result = PurchaseResult.Cancel;
            this.Dialog.Hide();
        }

        private void BuyButtonClicked()
        {
            this.result = PurchaseResult.Buy;
            this.Dialog.Hide();
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
