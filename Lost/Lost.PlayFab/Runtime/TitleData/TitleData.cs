//-----------------------------------------------------------------------
// <copyright file="TitleData.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if USING_PLAYFAB_SDK

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public class TitleData<T> : ScriptableObject where T : new()
    {
        #if UNITY_EDITOR
        [SerializeField] private T data = new T();
        #endif

        [SerializeField] private string titleDataKeyName = "";
        [SerializeField] private bool serializeWithUnity = true;
        [SerializeField] private bool compressData = false;

        #if UNITY_EDITOR
        public T Data
        {
            get { return this.data; }
        }
        #endif

        public string TitleDataKeyName
        {
            get { return this.titleDataKeyName; }
            set { this.titleDataKeyName = value; }
        }

        public bool SerializeWithUnity
        {
            get { return this.serializeWithUnity; }
            set { this.serializeWithUnity = value; }
        }

        public bool CompressData
        {
            get { return this.compressData; }
            set { this.compressData = value; }
        }

        public Lost.UnityTask<T> Load()
        {
            // Take key name and the verison and load from title data using PF class
            // If it's compressed, then decompress it
            return null;
        }
    }
}

#endif
