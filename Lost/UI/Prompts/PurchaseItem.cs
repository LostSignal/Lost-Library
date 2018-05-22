//--------------------------------------------------------------------s---
// <copyright file="PurchaseItem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    #if USE_TEXTMESH_PRO
    using Text = TMPro.TextMeshProUGUI;
    #else
    using Text = UnityEngine.UI.Text;
    #endif

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

        public void SetupDefault()
        {
            // creating objects
            var content = this.transform.Find("Content").gameObject;
            content.GetOrAddComponent<Image>();
            content.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            content.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 800);
            content.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            this.cancelButton = this.DebugCreateButton(content, "CancelButton", "CancelButtonText", "Cancel", new Vector3(300, 300));
            this.buyButton = this.DebugCreateButton(content, "BuyButton", "BuyButtonText", "Buy", new Vector3(0, -300));
            this.buyButtonText = this.buyButton.GetComponentInChildren<Text>();
            this.buyButtonIcon = this.DebugCreateImage(this.buyButton.gameObject, "Icon", Color.red, new Vector3(75, 0));
            this.storeItemIcon = this.DebugCreateImage(content, "StoreItemIcon", Color.blue, new Vector3(0, 400));
            this.storeItemDescription = this.DebugCreateText(content, "ItemDescription", "Description", Vector3.zero);
        }

        public override void OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            this.BuyButtonClicked();
        }

        public UnityTask<PurchaseResult> ShowStoreItem(StoreItem storeItem, bool hasSufficientFunds)
        {
            // resetting the result
            this.result = PurchaseResult.Cancel;

            this.buyButtonText.text = storeItem.IsIapItem ? storeItem.LocalizedString : storeItem.Cost.ToString();
            this.buyButtonText.color = hasSufficientFunds ? this.buyButtonTextColor : this.buyButtonTextColorInsufficient;
            this.buyButtonIcon.gameObject.SetActive(storeItem.IsIapItem == false);

            if (storeItem.IsIapItem == false)
            {
                this.buyButtonIcon.sprite = this.virtualCurrencyIcons.First(x => x.Id == storeItem.CostCurrencyId).Icon;
            }

            this.storeItemIcon.enabled = storeItem.PurchaseIcon != null;
            this.storeItemIcon.sprite = storeItem.PurchaseIcon;
            this.storeItemDescription.text = storeItem.PurchaseDescription;

            return UnityTask<PurchaseResult>.Run(this.ShowInternal());
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

        private IEnumerator<PurchaseResult> ShowInternal()
        {
            this.Show();

            // waiting for it to start showing
            while (this.IsShowing == false)
            {
                yield return default(PurchaseResult);
            }

            // waiting for it to return to the hidden state
            while (this.IsHidden == false)
            {
                yield return default(PurchaseResult);
            }

            yield return this.result;
        }

        private void CancelButtonClicked()
        {
            this.result = PurchaseResult.Cancel;
            this.Hide();
        }

        private void BuyButtonClicked()
        {
            this.result = PurchaseResult.Buy;
            this.Hide();
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
