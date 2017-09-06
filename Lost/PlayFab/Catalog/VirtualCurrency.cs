//-----------------------------------------------------------------------
// <copyright file="VirtualCurrency.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class VirtualCurrency
    {
        #pragma warning disable 0649
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private int initialDeposit;
        [SerializeField] private int rechargeRate;
        [SerializeField] private int rechargeMax;
        [SerializeField] private LazySprite icon;
        #pragma warning restore 0649

        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public int InitialDeposit
        {
            get { return this.initialDeposit; }
            set { this.initialDeposit = value; }
        }

        public int RechargeRate
        {
            get { return this.rechargeRate; }
            set { this.rechargeRate = value; }
        }

        public int RechargeMax
        {
            get { return this.rechargeMax; }
            set { this.rechargeMax = value; }
        }

        public LazySprite Icon
        {
            get { return this.icon; }
            set { this.icon = value; }
        }
    }
}
