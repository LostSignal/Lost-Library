//-----------------------------------------------------------------------
// <copyright file="BundleItem.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum BundleItemType
    {
        VirtualCurrency,
        CatalogItem,
    }

    [Serializable]
    public class BundleItem
    {
        #pragma warning disable 0649
        [SerializeField] private string id;
        [SerializeField] private string itemClass;
        [SerializeField] private LocalizedString displayName = new LocalizedString();
        [SerializeField] private LocalizedString description = new LocalizedString();
        [SerializeField] private List<Item> items = new List<Item> { new Item() };
        [SerializeField] private int realMoneyCost;
        #pragma warning restore 0649

        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public string ItemClass
        {
            get { return this.itemClass; }
            set { this.itemClass = value; }
        }

        public LocalizedString DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; }
        }

        public LocalizedString Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        public List<Item> Items
        {
            get { return this.items; }
            set { this.items = value; }
        }

        public int RealMoneyCost
        {
            get { return this.realMoneyCost; }
            set { this.realMoneyCost = value; }
        }

        public bool IsIapItem
        {
            get { return this.RealMoneyCost > 0; }
        }

        [Serializable]
        public class Item
        {
            #pragma warning disable 0649
            [SerializeField] private string id;
            [SerializeField] private BundleItemType type;
            [SerializeField] private int count;
            #pragma warning restore 0649

            public string Id
            {
                get { return this.id; }
                set { this.id = value; }
            }

            public BundleItemType Type
            {
                get { return this.type; }
                set { this.type = value; }
            }

            public int Count
            {
                get { return this.count; }
                set { this.count = value; }
            }
        }
    }
}
