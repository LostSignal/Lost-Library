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
    public class LocalizedString : IValidate
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
            get { return this.Id; } // throw new NotImplementedException(); }
        }

        void IValidate.Validate()
        {
            // TODO [bgish]: Actually verify that localizedStringId is not null and exists in a localization table
            throw new NotImplementedException();
        }
    }
}
