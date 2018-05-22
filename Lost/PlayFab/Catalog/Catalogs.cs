//-----------------------------------------------------------------------
// <copyright file="Catalogs.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu]
    public class Catalogs : SingletonScriptableObjectResource<Catalogs>
    {
        #pragma warning disable 0649
        [SerializeField] private List<Catalog> catalogs =new List<Catalog> { new Catalog() };
        #pragma warning restore 0649

        public List<Catalog> AllCatalogs
        {
            get { return this.catalogs; }
        }

        public Catalog GetCatalog(string version)
        {
            return this.catalogs.FirstOrDefault(x => x.Version == version);
        }
    }
}
