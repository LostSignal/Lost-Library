//-----------------------------------------------------------------------
// <copyright file="VectorConverter.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class VectorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector2) || objectType == typeof(UnityEngine.Vector3) || objectType == typeof(UnityEngine.Vector4);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var temp = JObject.Load(reader);

            if (objectType == typeof(UnityEngine.Vector2))
            {
                return new UnityEngine.Vector2((float)temp["x"], (float)temp["y"]);
            }
            else if (objectType == typeof(UnityEngine.Vector3))
            {
                return new UnityEngine.Vector3((float)temp["x"], (float)temp["y"], (float)temp["z"]);
            }
            else if (objectType == typeof(UnityEngine.Vector4))
            {
                return new UnityEngine.Vector4((float)temp["x"], (float)temp["y"], (float)temp["z"], (float)temp["w"]);
            }

            throw new Exception("Tried to read Json of unkonwn Vector type!");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is UnityEngine.Vector2 vector2)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(vector2.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vector2.y);
                writer.WriteEndObject();
            }
            else if (value is UnityEngine.Vector3 vector3)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(vector3.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vector3.y);
                writer.WritePropertyName("z");
                writer.WriteValue(vector3.z);
                writer.WriteEndObject();
            }
            else if (value is UnityEngine.Vector4 vector4)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(vector4.x);
                writer.WritePropertyName("y");
                writer.WriteValue(vector4.y);
                writer.WritePropertyName("z");
                writer.WriteValue(vector4.z);
                writer.WritePropertyName("w");
                writer.WriteValue(vector4.w);
                writer.WriteEndObject();
            }
            else
            {
                throw new Exception("Tried to write Json of unkonwn Vector type!");
            }
        }
    }}