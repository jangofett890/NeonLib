using NeonLib.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NeonLib.Editor {
    [CustomEditor(typeof(GameEventListener), true)]
    public class GameEventListenerEditor : UnityEditor.Editor {
        private GameEventListener _listener;
        private VisualElement _RootElement;
        private VisualTreeAsset _treeAsset;

        private Button _RemoveGameEventResponse;
        private Button _AddGameEventResponse;

        private ListView _responseListView;

        public override VisualElement CreateInspectorGUI() {
            _treeAsset = Resources.Load<VisualTreeAsset>("UI/Editor/NeonLib/GameEventListenerInspector");
            _listener = (GameEventListener)target;
            _RootElement = new VisualElement();
            _RootElement.styleSheets.Add(Resources.Load<StyleSheet>("UI/Editor/NeonLib/GameEventListenerStyles"));
            _RootElement.Add(_treeAsset.CloneTree());
            _RemoveGameEventResponse = _RootElement.Q<Button>("Remove");
            _AddGameEventResponse = _RootElement.Q<Button>("Add");
            _responseListView = _RootElement.Q<ListView>("ResponseListView");
            _responseListView.selectionType = SelectionType.Multiple;
            _responseListView.itemsSource = _listener.gameEventResponses;
            _responseListView.makeItem = CreateGameEventResponseContainer;
            _responseListView.bindItem = AddGameEventResponseInspectorToListView;
            _responseListView.unbindItem = RemoveGameEventResponseInspectorFromListView;
            _responseListView.Rebuild();
            _RemoveGameEventResponse.clicked += RemoveSelectedGameEventResponse;
            _AddGameEventResponse.clicked += AddNewGameEventResponse;

            return _RootElement;
        }



        private VisualElement CreateGameEventResponseContainer() {
            VisualElement Container = new VisualElement();
            Container.AddToClassList("ResponseItemContainer");
            Container.Add(new Label("Invoke Method With Event Arguments"));
            return Container;
        }
        private void AddGameEventResponseInspectorToListView(VisualElement Container, int index) {
            var editor = UnityEditor.Editor.CreateEditor(_listener.gameEventResponses[index]);
            if (editor != null) {
                VisualElement editorElement = editor.CreateInspectorGUI();
                editorElement.AddToClassList("EditorContainer");
                Container.Add(editorElement);
            }
        }
        private void RemoveGameEventResponseInspectorFromListView(VisualElement Container, int index) {
            Container.Clear();
        }
        private void AddNewGameEventResponse() {
            Undo.RecordObject(_listener, "Add new Game Event Response to Game Event Listener");
            var objToAdd = ScriptableObject.CreateInstance<UOMethodInvoker>();
            _listener.gameEventResponses.Add(objToAdd);
            string assetPath = AssetDatabase.GetAssetPath(_listener);
            if (!string.IsNullOrEmpty(assetPath)) {
                AssetDatabase.AddObjectToAsset(objToAdd, assetPath);
            }
            else {
                AssetDatabase.AddObjectToAsset(objToAdd, _listener);
            }
            EditorUtility.SetDirty(_listener);
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            _responseListView.Rebuild();
        }

        private void RemoveSelectedGameEventResponse() {
            Undo.RecordObject(_listener, "Remove selected Game Event Responses from Game Event Listener");
            List<UOMethodInvoker> responsesToRemove = new List<UOMethodInvoker>();
            foreach (int i in _responseListView.selectedIndices) {
                responsesToRemove.Add(_listener.gameEventResponses[i]);
            }
            foreach (UOMethodInvoker response in responsesToRemove) {
                _listener.gameEventResponses.Remove(response);
                DestroyImmediate(response, true);
            }

            EditorUtility.SetDirty(_listener);
            AssetDatabase.SaveAssets();
            PrefabUtility.RecordPrefabInstancePropertyModifications(_listener);
            _responseListView.Rebuild();
        }
    }
}


