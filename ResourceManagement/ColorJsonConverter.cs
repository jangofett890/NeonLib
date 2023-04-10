using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace NeonLib.ResourceManagement {
    public class ColorJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Color);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            Color color = (Color)value;
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(color.r);
            writer.WritePropertyName("g");
            writer.WriteValue(color.g);
            writer.WritePropertyName("b");
            writer.WriteValue(color.b);
            writer.WritePropertyName("a");
            writer.WriteValue(color.a);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            JObject jsonObject = JObject.Load(reader);
            return new Color(
                jsonObject["r"].Value<float>(),
                jsonObject["g"].Value<float>(),
                jsonObject["b"].Value<float>(),
                jsonObject["a"].Value<float>()
            );
        }
    }
}