using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Reflection;
using NeonLib.Events;
using NeonLib.Variables;

namespace NeonLib.Editor {
    [CustomEditor(typeof(UOMethodInvoker), true)]
    public class UOMethodInvokerEditor : UnityEditor.Editor {

        public UOMethodInvoker _response { get; set; }
        public VisualElement _RootElement { get; private set; }
        public VisualTreeAsset _treeAsset { get; private set; }

        private ObjectField _TargetField;
        private DropdownField _methodDropdown;
        private VisualElement _targetMethodParams;

        public override VisualElement CreateInspectorGUI() {
            _treeAsset = Resources.Load<VisualTreeAsset>("UI/Editor/UOMethodInvokerInspector");
            _response = (UOMethodInvoker)target;
            _RootElement = new VisualElement();
            _RootElement.styleSheets.Add(Resources.Load<StyleSheet>("UI/Editor/UOMethodInvokerStyles"));
            _RootElement.Add(_treeAsset.CloneTree());
            _methodDropdown = _RootElement.Q<DropdownField>("MethodDropdown");
            _methodDropdown.choices = GetAvailableMethodsThroughReflection();
            _methodDropdown.RegisterCallback<ChangeEvent<string>>(OnMethodChanged);
            _TargetField = _RootElement.Q<ObjectField>("TargetField");
            _TargetField.RegisterCallback<ChangeEvent<UnityEngine.Object>>(OnTargetChange);
            _targetMethodParams = _RootElement.Q<VisualElement>("TargetMethodParams");
            LoadCurrentValues();
            Undo.undoRedoPerformed += UndoListener;
            return _RootElement;
        }



        private void LoadCurrentValues() {
            _TargetField.value = _response.Target;
            _methodDropdown.value = _response.MethodName;
            RefreshArguments();
        }

        private void OnTargetChange(ChangeEvent<UnityEngine.Object> evt) {
            Undo.RecordObject(_response, "Change Game Event Response Target");
            _response.Target = evt.newValue;
            EditorUtility.SetDirty(_response);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_response);
            RefreshChoices();
        }

        private void UndoListener() {
            LoadCurrentValues();
            RefreshChoices();
        }

        private void OnMethodChanged(ChangeEvent<string> evt) {
            Undo.RecordObject(_response, "Change Game Event Response Method Name");
            _response.MethodName = evt.newValue;
            RefreshArguments();
            EditorUtility.SetDirty(_response);
            PrefabUtility.RecordPrefabInstancePropertyModifications(_response);
        }

        public void RefreshArguments() {
            if (string.IsNullOrEmpty(_response.MethodName))
                return;

            MethodInfo method = _response.TargetAction;
            ParameterInfo[] parameters = method.GetParameters();

            // Resize the arguments list to match the method's parameter count
            while (_response.arguments.Count < parameters.Length) {
                ScriptableVariable sv = ScriptableVariable.CreateInstance<ScriptableVariable>();
                _response.arguments.Add(sv);
            }
            while (_response.arguments.Count > parameters.Length) {
                _response.arguments.RemoveAt(_response.arguments.Count - 1);
            }

            _targetMethodParams.Clear();

            for (int i = 0; i < parameters.Length; i++) {
                ParameterInfo param = parameters[i];

                // Update the ValueType for each argument
                _response.arguments[i].ValueType = param.ParameterType;

                // Create a container for the argument
                VisualElement argumentContainer = new VisualElement();
                argumentContainer.AddToClassList("argument-container");
                _targetMethodParams.Add(argumentContainer);

                // Add a label for the argument name
                Label argName = new Label(param.Name);
                argName.AddToClassList("argument-name");
                argumentContainer.Add(argName);

                // Add the argument's editor
                var editor = UnityEditor.Editor.CreateEditor(_response.arguments[i]);
                if (editor != null) {
                    VisualElement editorElement = editor.CreateInspectorGUI();
                    editorElement.AddToClassList("EditorContainer");
                    argumentContainer.Add(editorElement);
                }
            }
        }

        public void RefreshChoices() {
            _methodDropdown.choices = GetAvailableMethodsThroughReflection();
        }

        private List<string> GetAvailableMethodsThroughReflection() {
            List<string> result = new List<string>();
            if(_response.Target != null) {
                MethodInfo[] PublicMethods = _response.Target.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (MethodInfo method in PublicMethods) {
                    result.Add(method.Name);
                }
            }
            return result;
        }
    }
}
