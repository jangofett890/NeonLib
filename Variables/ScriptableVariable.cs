using NeonLib.ResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
[assembly: InternalsVisibleTo("NeonLib.Editor")]
namespace NeonLib.Variables {
    [CreateAssetMenu(menuName = "NeonLib/ScriptableVariable", fileName = "ScriptableVariable")]
    public class ScriptableVariable : ScriptableObject {
        [SerializeReference]
        internal UnityEngine.Object unityObjectValue;
        [SerializeField]
        internal string jsonValue;
        [SerializeField]
        internal string typeFullName;
        public Type ValueType {
            get {
                if (typeFullName == null || typeFullName == "Null")
                    return null;
                return Type.GetType(typeFullName);
            }
            internal set {
                typeFullName = value.AssemblyQualifiedName;
            }
        }
        public object Value {
            get {
                if (unityObjectValue != null) {
                    return unityObjectValue;
                }
                else {
                    if (jsonValue != null && jsonValue != "")
                        return JsonConvert.DeserializeObject(jsonValue, ValueType, NeonLibSerializationSettings.settings);
                }
                return null;
            }
            set {
                if (value is UnityEngine.Object unityObj) {
                    unityObjectValue = unityObj;
                    jsonValue = null;
                }
                else {
                    if(value != null) {
                        jsonValue = JsonConvert.SerializeObject(value, NeonLibSerializationSettings.settings);
                    }
                    unityObjectValue = null;
                }
                typeFullName = value.GetType().AssemblyQualifiedName;
            }
        }
    }
}
