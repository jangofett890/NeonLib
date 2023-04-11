using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using NeonLib.States;
using UnityEditor;
using NeonLib.Templates;
using UnityEditor.Experimental.GraphView;

namespace NeonLib.Editor {
    public class GenericGraphEditor<N, G> : UnityEditor.Editor where N : UnityEditor.Experimental.GraphView.Node, new() where G : GraphView, new() {
        protected VisualTreeAsset _treeAsset;
        protected VisualElement _rootElement;
        protected VisualElement _graphViewContainer;

        protected int _currentGraphViewIndex = -1;
        protected List<GraphView> _graphViewHistory = new List<GraphView>();
        protected GraphView _graphView;

        protected List<N> _nodesInGraph;
        protected List<N> _nodesAvailableForGraph;


        protected ListView _currentNodesListView;
        protected ListView _availableNodesListView;

        protected ToolbarButton _backButton;
        protected ToolbarButton _forwardButton;
        protected ToolbarButton _showHideGraphButton;

        protected ToolbarSearchField _currentNodesFilter;
        protected ToolbarSearchField _availableNodesFilter;

        public override VisualElement CreateInspectorGUI() {
            // Load the UXML
            _treeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/NeonLib/Defaults/Resources/UI/Editor/NeonLib/GenericGraphEditor.uxml");
            _rootElement = _treeAsset.CloneTree();
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/NeonLib/Defaults/Resources/UI/Editor/NeonLib/GenericGraphStyles.uss");
            _rootElement.styleSheets.Add(styleSheet);
            // Get references to UI elements
            _currentNodesListView = _rootElement.Q<ListView>("currentNodesContainer");
            _availableNodesListView = _rootElement.Q<ListView>("availableNodesContainer");
            _graphViewContainer = _rootElement.Q<VisualElement>("GraphViewContainer");

            // Get references to toolbars
            _backButton = _rootElement.Q<ToolbarButton>("Back");
            _forwardButton = _rootElement.Q<ToolbarButton>("Forward");
            _showHideGraphButton = _rootElement.Q<ToolbarButton>("ShowHideGraph");
            _currentNodesFilter = _rootElement.Q<ToolbarSearchField>("currentNodesFilter");
            _availableNodesFilter = _rootElement.Q<ToolbarSearchField>("availableNodesFilter");

            // Set up search filter functionality
            _currentNodesFilter.RegisterValueChangedCallback(evt => FilterCurrentNodes(evt.newValue));
            _availableNodesFilter.RegisterValueChangedCallback(evt => FilterAvailableNodes(evt.newValue));



            // Bind toolbar button click events
            _backButton.clicked += GoBack;
            _forwardButton.clicked += GoForward;
            _showHideGraphButton.clicked += ToggleGraphVisibility;


            // Initialize the StateMachineGraphView
            InitilizeGraphView();

            // Populate the list views with the appropriate data
            _nodesInGraph = GetCurrentNodes();
            _nodesAvailableForGraph = GetAvailableNodes();

            PopulateCurrentNodesListView();
            PopulateAvailableNodesListView();

            return _rootElement;
        }

        protected virtual void FilterCurrentNodes(string searchValue) {
            // Implement filtering logic for current nodes
            if (string.IsNullOrEmpty(searchValue)) {
                _currentNodesListView.itemsSource = _nodesInGraph;
            }
            else {
                _currentNodesListView.itemsSource = _nodesInGraph.Where(node => node.title.ToLowerInvariant().Contains(searchValue.ToLowerInvariant())).ToList();
            }
            _currentNodesListView.Rebuild();
        }

        protected virtual void FilterAvailableNodes(string searchValue) {
            // Implement filtering logic for available nodes
            if (string.IsNullOrEmpty(searchValue)) {
                _availableNodesListView.itemsSource = _nodesAvailableForGraph;
            }
            else {
                _availableNodesListView.itemsSource = _nodesAvailableForGraph.Where(node => node.title.ToLowerInvariant().Contains(searchValue.ToLowerInvariant())).ToList();
            }
            _availableNodesListView.Rebuild();
        }

        protected void ToggleGraphVisibility() {
            if (_graphViewContainer.style.display == DisplayStyle.Flex) {
                _graphViewContainer.style.display = DisplayStyle.None;
                _showHideGraphButton.text = "▲";
            }
            else {
                _graphViewContainer.style.display = DisplayStyle.Flex;
                _showHideGraphButton.text = "▼";
            }
        }

        protected virtual List<N> GetCurrentNodes() {
            List<N> nodes = new List<N>();

            return nodes;
        }

        protected virtual List<N> GetAvailableNodes() {
            List<N> nodes = new List<N>();

            return nodes;
        }

        protected virtual void RemoveNodeFromGraph(N node) {
            if (_nodesInGraph.Contains(node)) {
                _nodesInGraph.Remove(node);
                _graphView.RemoveElement(node);
                PopulateCurrentNodesListView();
            }
        }

        protected virtual void AddNodeToGraph(N node) {
            if(!_nodesInGraph.Contains(node)) {
                _nodesInGraph.Add(node);
                _graphView.AddElement(node);
            }
        }

        protected virtual void InitilizeGraphView() {
            _graphView = new G();
            _currentGraphViewIndex++;
            _graphViewHistory.Insert(_currentGraphViewIndex, _graphView);
            _graphViewHistory.RemoveRange(_currentGraphViewIndex + 1, _graphViewHistory.Count - _currentGraphViewIndex - 1);
            _graphViewContainer.Add(_graphView);
        }

        private void GoBack() {
            if (_currentGraphViewIndex > 0) {
                _currentGraphViewIndex--;
                GraphView previousGraphView = _graphViewHistory[_currentGraphViewIndex];

                _graphViewContainer.Remove(_graphView);
                _graphView = previousGraphView;
                _graphViewContainer.Add(_graphView);
            }
        }

        private void GoForward() {
            if (_currentGraphViewIndex < _graphViewHistory.Count - 1) {
                _currentGraphViewIndex++;
                GraphView nextGraphView = _graphViewHistory[_currentGraphViewIndex];

                _graphViewContainer.Remove(_graphView);
                _graphView = nextGraphView;
                _graphViewContainer.Add(_graphView);
            }
        }

        protected virtual void PopulateCurrentNodesListView() {
            _currentNodesListView.itemsSource = _nodesInGraph;
            _currentNodesListView.bindItem = (element, i) => {
                element.Clear();
                element.Add(new Label(_nodesInGraph[i].name));
            };
            _currentNodesListView.onSelectionChange += OnCurrentNodesSelectionChanged;
            _currentNodesListView.onItemsChosen += OnCurrentNodeChosen;
        }

        protected virtual void PopulateAvailableNodesListView() {
            _availableNodesListView.itemsSource = _nodesInGraph;
            _availableNodesListView.bindItem = (element, i) => {
                element.Clear();
                element.Add(new Label(_nodesInGraph[i].name));
            };
            _availableNodesListView.onSelectionChange += OnAvailableNodesSelectionChanged;
            _availableNodesListView.onItemsChosen += OnAvailableNodeChosen;
        }

        protected virtual void OnCurrentNodeChosen(object chosenItem) {
            N chosenNode = chosenItem as N;
            if (chosenNode != null) {
                // Implement your logic for focusing the chosen current node
                chosenNode.BringToFront();
                chosenNode.Select(_graphView, false);
            }
        }

        protected virtual void OnAvailableNodeChosen(object chosenItem) {
            N chosenNode = chosenItem as N;
            if (chosenNode != null) {
                // Implement your logic for adding the chosen available node to the current ones
                _nodesInGraph.Add(chosenNode);
                PopulateCurrentNodesListView();
            }
        }

        protected virtual void OnCurrentNodesSelectionChanged(IEnumerable<object> selectedItems) {
            // Implement your logic for handling the selection change
        }

        protected virtual void OnAvailableNodesSelectionChanged(IEnumerable<object> selectedItems) {
            // Implement your logic for handling the selection change
        }
    }
}
