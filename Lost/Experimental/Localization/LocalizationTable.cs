//-----------------------------------------------------------------------
// <copyright file="LocalizationTable.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [CreateAssetMenu]
    public class LocalizationTable : ScriptableObject
    {
        /// <summary>
        /// Represents what a null id is for a localization table entry.
        /// </summary>
        public const ushort NullId = 0;

        /// <summary>
        /// List of all the localization entries.
        /// </summary>
        [SerializeField]
        private List<Entry> entries = new List<Entry>();
                
        #if UNITY_EDITOR

        /// <summary>
        /// Gets the number of localization entries.
        /// </summary>
        /// <value>Number of localized entries.</value>
        public int Count
        {
            get { return this.entries.Count; }
        }

        public IEntry this[int index]
        {
            get { return this.entries[index]; }
        }
        
        public ushort AddNewText(string newText)
        {
            ushort newId = this.entries.Count > 0 ? (ushort)(this.entries.Last().Id + 1) : (ushort)1;
            this.AddNewText(newId, newText);
            return newId;
        }
        
        public void AddNewText(ushort newId, string newText)
        {
            if (newId == NullId)
            {
                Debug.LogErrorFormat(this, "LocalizationTable.AddNewText(newId, newText) was given a null id of {0}!", NullId);
            }
            else if (newText == null)
            {
                Debug.LogErrorFormat(this, "LocalizationTable.AddNewText(newId, newText) was given a null newText!");
            }
            else if (this.GetIndex(newId) != -1)
            {
                Debug.LogErrorFormat(this, "LocalizationTable.AddNewText(newId, newText) already contains id {0}!", newId);
            }
            else
            {
                this.entries.Add(new Entry(newId, newText));

                this.entries.Sort(delegate(Entry e1, Entry e2) 
                {
                    return e1.Id.CompareTo(e2.Id);
                });
            }
        }

        public void UpdateText(ushort id, string newText)
        {
            if (id == NullId)
            {
                Debug.LogErrorFormat(this, "LocTable.UpdateText(id, newText) was given a null id of {0}!", NullId);
            }
            else if (newText == null)
            {
                Debug.LogError("LocTable.AddNewText(id, newText) was given a null newText!", this);
            }
            else if (this.GetIndex(id) == -1)
            {
                Debug.LogErrorFormat(this, "LocTable.UpdateText(id, newText) was given an unknown id {0}!", id);
            }
            else
            {
                this.entries[id].Text = newText;
            }
        }

        public void OverrideLastEdited(ushort id, DateTime dateTime)
        {
            if (id == NullId)
            {
                Debug.LogErrorFormat(this, "LocTable.OverrideLastEdited(id, dateTime) was given a null id of {0}!", NullId);
            }
            else if (this.GetIndex(id) == -1)
            {
                Debug.LogErrorFormat(this, "LocTable.OverrideLastEdited(id, dateTime) was given an unknown id {0}!", id);
            }
            else
            {
                this.entries[id].SetLastEdited(dateTime);
            }
        }

        #endif

        /// <summary>
        /// Returns the text for the given id.
        /// </summary>
        /// <param name="id">The id to get the text for.</param>
        /// <returns>The localized text.</returns>
        public string GetText(ushort id)
        {
            int index = this.GetIndex(id);

            if (index == -1)
            {
                Debug.LogErrorFormat(this, "LocTable.GetText(id) was given an unknown id {0}", id);
                return null;
            }
            else
            {
                return this.entries[index].Text;
            }
        }

        /// <summary>
        /// Search the entries list for an item with the given id.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <returns>The index of the entry that matches the given id.</returns>
        private int GetIndex(ushort id)
        {
            // TODO do a binary search to find the localization value instead of the current linear one
            for (int i = 0; i < this.entries.Count; i++)
            {
                if (this.entries[i].Id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        [Serializable]
        private class Entry : IEntry
        {
            [SerializeField] private ushort id;
            [SerializeField] private string text;
            [SerializeField] private DateTime lastEdited;

            public Entry(ushort id, string text)
            {
                this.id = id;
                this.Text = text;
            }

            public ushort Id
            {
                get { return this.id; }
            }

            public string Text
            {
                get
                {
                    return this.text;
                }

                set
                {
                    if (this.text != value)
                    {
                        this.text = value;
                        this.lastEdited = DateTime.UtcNow;
                    }
                }
            }

            public DateTime LastEdited
            {
                get { return this.lastEdited; }
            }

            public void SetLastEdited(DateTime dateTime)
            {
                this.lastEdited = dateTime;
            }
        }
    }
}