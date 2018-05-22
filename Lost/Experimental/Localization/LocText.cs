//-----------------------------------------------------------------------
// <copyright file="LocText.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class LocText
    {
        #pragma warning disable 0649
        [SerializeField] private LocalizationTable localizationTable;
        [SerializeField] private ushort localizationTextId;
        #pragma warning restore 0649

        private string textCache;

        public LocalizationTable LocalizationTable
        {
            get { return this.localizationTable; }
        }

        public ushort Id
        {
            get { return this.localizationTextId; }
        }

        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(this.textCache) && this.localizationTable != null)
                {
                    this.textCache = this.localizationTable.GetText(this.localizationTextId);
                }

                return this.textCache;
            }
        }

        public string FormatValue(params object[] args)
        {
            return string.Format(this.Text, args);
        }

        public void InvalidateCache()
        {
            this.textCache = null;
        }
    }
}
