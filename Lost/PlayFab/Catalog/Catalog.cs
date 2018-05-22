//-----------------------------------------------------------------------
// <copyright file="Catalog.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Catalog
    {
        #pragma warning disable 0649
        [SerializeField] private string version;
        [SerializeField] private List<string> itemClasses = new List<string>();
        [SerializeField] private List<VirtualCurrency> virtualCurrencies = new List<VirtualCurrency>();
        [SerializeField] private List<CatalogItem> catalogItems = new List<CatalogItem>();
        [SerializeField] private List<BundleItem> bundleItems = new List<BundleItem>();
        [SerializeField] private List<Store> stores = new List<Store>();
        #pragma warning restore 0649

        public string Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        public List<string> ItemClasses
        {
            get { return this.itemClasses; }
            set { this.itemClasses = value; }
        }

        public List<VirtualCurrency> VirtualCurrencies
        {
            get { return this.virtualCurrencies; }
            set { this.virtualCurrencies = value; }
        }

        public List<CatalogItem> CatalogItems
        {
            get { return this.catalogItems; }
            set { this.catalogItems = value; }
        }

        public List<BundleItem> BundleItems
        {
            get { return this.bundleItems; }
            set { this.bundleItems = value; }
        }

        public List<Store> Stores
        {
            get { return this.stores; }
            set { this.stores = value; }
        }
    }
}
