using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NeonLib.Localization {
    [CreateAssetMenu(fileName = "Localized Audio Clip", menuName = "NeonLib/Localization/Audio Clip", order = 1)]
    public class LocalizedAudioClip : ScriptableObject {
        [SerializeField]
        private string _Identifier;
        [SerializeField]
        private AudioClip _DefaultValue;

        public string Identifier {
            get { return _Identifier; }
        }

        public AudioClip DefaultValue {
            get { return _DefaultValue; }
        }

        public AudioClip Value {
            get
            {
                AudioClip localized = LocalizationManager.GetClip(_Identifier);
                if (localized == null)
                    return _DefaultValue;

                return localized;
            }
        }


    }
}