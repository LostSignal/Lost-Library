//-----------------------------------------------------------------------
// <copyright file="Store.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Store
    {
        #pragma warning disable 0649
        [SerializeField] private string id;
        [SerializeField] private List<StoreItem> storeItems = new List<StoreItem>();
        #pragma warning restore 0649

        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public List<StoreItem> StoreItems
        {
            get { return this.storeItems; }
            set { this.storeItems = value; }
        }
    }
}
