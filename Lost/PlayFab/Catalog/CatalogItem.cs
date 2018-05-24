//-----------------------------------------------------------------------
// <copyright file="InventoryDefinition.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    public enum UsageType
    {
        Consumable,
        Durable,
    }

    [Serializable]
    public class CatalogItem
    {
        #pragma warning disable 0649
        [SerializeField] private string id;
        [SerializeField] private string itemClass;
        [SerializeField] private UsageType usageType;
        [SerializeField] private LocalizedString displayName = new LocalizedString();
        [SerializeField] private LocalizedString description = new LocalizedString();
        [SerializeField] private bool isStackable;
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

        public UsageType UsageType
        {
            get { return this.usageType; }
            set { this.usageType = value; }
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

        public int RealMoneyCost
        {
            get { return this.realMoneyCost; }
            set { this.realMoneyCost = value; }
        }

        public bool IsStackable
        {
            get { return this.isStackable; }
            set { this.isStackable = value; }
        }

        public bool IsIapItem
        {
            get { return this.RealMoneyCost > 0; }
        }
    }
}
