//-----------------------------------------------------------------------
// <copyright file="JsonUtil.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace Lost{    public static class JsonUtil    {
        public static JsonSerializerSettings JsonSerializerSettings { get; private set; } = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatString = "yyyy-MM-ddTHH:mmZ",
                NullValueHandling = NullValueHandling.Ignore,
            };

        static JsonUtil()
        {
            JsonSerializerSettings.Converters.Add(new ColorConverter());
            JsonSerializerSettings.Converters.Add(new VectorConverter());
        }

        public static string Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }

        public static object Deserialize(string json, System.Type type)
        {
            return JsonConvert.DeserializeObject(json, type, JsonSerializerSettings);
        }    }}