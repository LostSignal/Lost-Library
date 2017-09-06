//-----------------------------------------------------------------------
// <copyright file="StoreItem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    public enum StoreItemType
    {
        CatalogItem,
        BundleItem,
    }

    [Serializable]
    public class StoreItem
    {
        #pragma warning disable 0649
        [SerializeField] private StoreItemType type;
        [SerializeField] private string itemId;
        [SerializeField] private string costCurrencyId;
        [SerializeField] private int cost;

        [Header("Purcasing Info")]
        [SerializeField] private string purchaseDescription;
        [SerializeField] private Sprite purchaseIcon;
        #pragma warning restore 0649
        
        public string StoreId { get; set; }
        
        public CatalogItem CatalogItem { get; set; }

        public BundleItem BundleItem { get; set; }

        public StoreItemType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public string ItemId
        {
            get { return this.itemId; }
            set { this.itemId = value; }
        }

        public string CostCurrencyId
        {
            get { return this.costCurrencyId; }
            set { this.costCurrencyId = value; }
        }

        public int Cost
        {
            get { return this.cost; }
            set { this.cost = value; }
        }

        public string PurchaseDescription
        {
            get { return this.purchaseDescription; }
            set { this.purchaseDescription = value; }
        }

        public Sprite PurchaseIcon
        {
            get { return this.purchaseIcon; }
            set { this.purchaseIcon = value; }
        }

        public bool IsIapItem
        {
            get { return this.CatalogItem != null ? this.CatalogItem.IsIapItem : this.BundleItem.IsIapItem; }
        }

        public string ItemClass
        {
            get { return this.CatalogItem != null ? this.CatalogItem.ItemClass : this.BundleItem.ItemClass; }
        }

        public bool IsFree
        {
            get { return this.costCurrencyId == "AD"; }
        }

        public string LocalizedString
        {
            get { return string.Format("${0}.{1:D2}", this.cost / 100, this.cost % 100); }
        }
    }
}
