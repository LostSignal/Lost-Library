//-----------------------------------------------------------------------
// <copyright file="UnityDateTime.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct UnityDateTime : ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private long dateTimeLong;
        private DateTime dateTime;

        public static implicit operator UnityDateTime(System.DateTime systemDateTime)
        {
            var unityDateTime = new UnityDateTime();
            unityDateTime.dateTime = systemDateTime;
            return unityDateTime;
        }

        public static implicit operator System.DateTime(UnityDateTime unityDateTime)
        {
            return unityDateTime.dateTime;
        }

        public void OnAfterDeserialize()
        {
            this.dateTime = DateTime.FromBinary(this.dateTimeLong);
        }

        public void OnBeforeSerialize()
        {
            this.dateTimeLong = this.dateTime.ToBinary();
        }

        public override bool Equals(object obj)
        {
            return this.dateTime.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.dateTime.GetHashCode();
        }

        public override string ToString()
        {
            return this.dateTime.ToString();
        }
    }
}
