using NeonLib.Events;
using NeonLib.Gameplay.MicroGames;
using NeonLib.States;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NeonLib.Editor {
    namespace NeonLib.Editor {
        [CustomEditor(typeof(CustomEventPlayable))]
        public class CustomEventPlayableEditor : UnityEditor.Editor {
            private VisualTreeAsset _treeAsset;
            private CustomEventPlayable _customEventPlayable;
            private VisualElement _rootElement;

            private EnumField _clipTypeField;
            private ObjectField _gameEventField;
            private ObjectField _microGameManagerField;
            private ObjectField _targetStateField;

            public override VisualElement CreateInspectorGUI() {
                _treeAsset = Resources.Load<VisualTreeAsset>("UI/Editor/CustomEventPlayableInspector");
                _customEventPlayable = (CustomEventPlayable)target;
                _rootElement = new VisualElement();
                _rootElement.styleSheets.Add(Resources.Load<StyleSheet>("UI/Editor/CustomEventPlayableStyles"));
                _rootElement.Add(_treeAsset.CloneTree());

                _clipTypeField = _rootElement.Q<EnumField>("ClipTypeField");
                _gameEventField = _rootElement.Q<ObjectField>("GameEventField");
                _microGameManagerField = _rootElement.Q<ObjectField>("MicroGameManagerField");
                _targetStateField = _rootElement.Q<ObjectField>("TargetStateField");

                _clipTypeField.RegisterValueChangedCallback(e => _customEventPlayable.clipType = (CustomEventPlayable.PlayableClipType)e.newValue);
                _gameEventField.RegisterValueChangedCallback(e => _customEventPlayable.gameEvent = (GameEvent)e.newValue);
                _microGameManagerField.RegisterValueChangedCallback(e => _customEventPlayable.microGameManager = (MicroGameManager)e.newValue);
                _targetStateField.RegisterValueChangedCallback(e => _customEventPlayable.targetState = (State)e.newValue);

                LoadCurrentValues();

                return _rootElement;
            }

            private void LoadCurrentValues() {
                _clipTypeField.SetValueWithoutNotify(_customEventPlayable.clipType);
                _gameEventField.SetValueWithoutNotify(_customEventPlayable.gameEvent);
                _microGameManagerField.SetValueWithoutNotify(_customEventPlayable.microGameManager);
                _targetStateField.SetValueWithoutNotify(_customEventPlayable.targetState);
            }
        }
    }
}
