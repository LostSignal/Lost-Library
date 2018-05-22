//-----------------------------------------------------------------------
// <copyright file="IdBag.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class IdBag : ISerializationCallbackReceiver
    {
        public static readonly int NullId = int.MaxValue;
        private const int NumberOfBits = sizeof(int) * 8;

        [SerializeField]
        private List<int> bits = new List<int>();
        private int idCount;

        public int Count
        {
            get { return this.idCount; }
        }

        #if UNITY_EDITOR

        public int CalculatedBitCount
        {
            get { return this.CalculateBitCount(); }
        }

        public int BitsListCount
        {
            get { return this.bits != null ? this.bits.Count : 0; }
        }

        #endif

        public bool ContainsId(int id)
        {
            int index = id / NumberOfBits;

            if (index >= this.bits.Count)
            {
                return false;
            }
            else
            {
                return (this.bits[index] & (1 << (id % NumberOfBits))) != 0;
            }
        }

        public void AddId(int id)
        {
            if (id == NullId)
            {
                throw new ArgumentException("Can not add Null Id to IdBag");
            }

            if (id < 0)
            {
                throw new ArgumentException("IdBag can not store negative ids");
            }

            // don't add it if it already exists
            if (this.ContainsId(id))
            {
                return;
            }

            int index = id / NumberOfBits;
            int listSizeNeeded = index + 1;

            // making sure there are enough space in the bits list for the new id
            while (this.bits.Count < listSizeNeeded)
            {
                this.bits.Add(0);
            }

            this.bits[index] |= 1 << (id % NumberOfBits);
            this.idCount++;
        }

        public void AddIds(params int[] ids)
        {
            if (ids != null)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    this.AddId(ids[i]);
                }
            }
        }

        public void RemoveId(int id)
        {
            if (id == NullId)
            {
                throw new ArgumentException("Can not add Null Id to IdBag");
            }

            if (id < 0)
            {
                throw new ArgumentException("IdBag can not store negative ids");
            }

            // can't remove it if it doesn't exist
            if (this.ContainsId(id) == false)
            {
                return;
            }

            int index = id / NumberOfBits;
            this.bits[index] &= ~(1 << (id % NumberOfBits));
            this.idCount--;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.idCount = this.CalculateBitCount();
        }

        private int CalculateBitCount()
        {
            int count = 0;

            for (int i = 0; i < this.bits.Count; i++)
            {
                for (int j = 0; j < NumberOfBits; j++)
                {
                    count += (this.bits[i] & (1 << j)) == 0 ? 0 : 1;
                }
            }

            return count;
        }
    }
}
