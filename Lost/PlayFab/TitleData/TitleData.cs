//-----------------------------------------------------------------------
// <copyright file="TitleData.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu]
    public abstract class TitleData<T> : ScriptableObject where T : IVersion, new()
    {
        #pragma warning disable 0649
        [SerializeField] private string key;
        [SerializeField] private List<T> data = new List<T> { new T() };
        #pragma warning restore 0649

        public string Key
        {
            get { return this.key; }
        }

        public List<T> Data
        {
            get { return this.data; }
        }

        public T GetDataVersion(string version)
        {
            return this.data.FirstOrDefault(x => x.Version == version);
        }
    }
}
