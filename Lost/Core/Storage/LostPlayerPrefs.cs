//-----------------------------------------------------------------------
// <copyright file="LostPlayerPrefs.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;

    public static class LostPlayerPrefs
    {
        private static bool isDirty;

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void DeleteKey(string key)
        {
            isDirty = true;
            PlayerPrefs.DeleteKey(key);
        }

        public static string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public static void SetString(string key, string value)
        {
            isDirty = true;
            PlayerPrefs.SetString(key, value);
        }
        
        public static void Save()
        {
            if (isDirty)
            {
                isDirty = false;
                PlayerPrefs.Save();
            }
        }
    }
}
