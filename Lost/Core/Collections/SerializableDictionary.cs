//-----------------------------------------------------------------------
// <copyright file="SerializableDictionary.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class SerializableDictionary<T, V> : Dictionary<T, V>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] private List<T> keys;
        [SerializeField, HideInInspector] private List<V> values;

        public void OnBeforeSerialize()
        {
            this.keys = new List<T>(this.Count);
            this.values = new List<V>(this.Count);

            foreach (var keyValuePair in this)
            {
                this.keys.Add(keyValuePair.Key);
                this.values.Add(keyValuePair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            int count = Mathf.Min(this.keys.Count, this.values.Count);

            for (int i = 0; i < count; i++)
            {
                this.Add(this.keys[i], this.values[i]);
            }
        }
    }
}
