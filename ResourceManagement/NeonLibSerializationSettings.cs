using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace NeonLib.ResourceManagement {
    public static class NeonLibSerializationSettings {
        public static JsonSerializerSettings settings {
            get {
                return new JsonSerializerSettings() {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Converters = new List<JsonConverter> { new ColorJsonConverter() }
                };
            }
        }


    }
}