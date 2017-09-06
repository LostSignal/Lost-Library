//-----------------------------------------------------------------------
// <copyright file="LocalizedString.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class LocalizedString
    {
        #pragma warning disable 0649
        [SerializeField] private string localizedStringId;
        #pragma warning restore 0649

        public string Id
        {
            get { return this.localizedStringId; }
            set { this.localizedStringId = value; }
        }

        public string Value
        {
            // TODO [bgish]: Actually query localizatin system to get this info
            get { throw new NotImplementedException(); }
        }
    }
}
