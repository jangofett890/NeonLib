using NeonLib.Events;
using NeonLib.Gameplay.MicroGames;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NeonLib.Editor {
    [CustomEditor(typeof(MicroGameInputResponse), true)]

    public class MicroGameInputResponseEditor : UnityEditor.Editor {
        private MicroGameInputResponse _response;
        private VisualElement _RootElement;
        private VisualTreeAsset _treeAsset;

    }
}