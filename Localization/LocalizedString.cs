using NeonLib.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonLib.Localization {
    [CreateAssetMenu(fileName = "Localized String", menuName = "Localization/String", order = 0)]
    public class LocalizedString : ScriptableObject {
        [SerializeField]
        private string _Identifier, _DefaultValue;

        public string Identifier {
            get { return _Identifier; }
        }

        public string DefaultValue {
            get { return _DefaultValue; }
        }

        public string Value {
            get
            {
                string localized = LocalizationManager.GetString(_Identifier);
                if (string.IsNullOrEmpty(localized))
                    return _DefaultValue;

                return localized;
            }
        }
    }
}