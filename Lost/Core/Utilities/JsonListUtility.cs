//-----------------------------------------------------------------------
// <copyright file="JsonListUtility.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public static class JsonListUtility
    {
        public static string ToJsonList<T>(List<T> list) where T : class
        {
            if (list == null)
            {
                return "[]";
            }

            StringBuilder json = new StringBuilder();
            json.Append("[");

            for (int i = 0; i < list.Count; i++)
            {
                json.Append(JsonUtility.ToJson(list[i]));

                if (i != list.Count - 1)
                {
                    json.Append(",");
                }
            }

            json.Append("]");

            return json.ToString();
        }

        public static List<T> FromJsonList<T>(string json) where T : class
        {
            var results = new List<T>();

            int itemStartIndex = 0;
            int curlyCount = 0;

            for (int i = 1; i < json.Length; i++)
            {
                if (json[i] == '{')
                {
                    curlyCount++;

                    if (curlyCount == 1)
                    {
                        itemStartIndex = i;
                    }
                }
                else if (json[i] == '}')
                {
                    curlyCount--;

                    if (curlyCount == 0)
                    {
                        string itemJson = json.Substring(itemStartIndex, i - itemStartIndex + 1);
                        results.Add(JsonUtility.FromJson<T>(itemJson));
                    }
                }
            }

            return results;
        }
    }
}
