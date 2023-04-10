using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonLib.Localization {
    [System.Serializable]
    public class LocaleAsset : ScriptableObject {
        public Dictionary<string, string> localizedStrings;
        public Dictionary<string, string> localizedClipPaths;
    }
}