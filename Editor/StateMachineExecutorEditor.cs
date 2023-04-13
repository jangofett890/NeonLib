using NeonLib.Graphing;
using NeonLib.Graphing.States;
using NeonLib.States;
using NeonLib.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NeonLib.Editor {
    [CustomEditor(typeof(StateMachineExecutor))]
    public class StateMachineExecutorEditor : GenericGraphEditor<BaseStatesNode, GenericGraphView> {
        private StateMachineExecutor _stateMachineExecutor;
        private ListView _templateListView;
        private GenericGraphView genericGraphView;
        public override VisualElement CreateInspectorGUI() {
            // Call the base method to create the default UI
            VisualElement rootElement = base.CreateInspectorGUI();

            // Create a ListView for the templates
            _templateListView = new ListView();
            _templateListView.AddToClassList("template-list-view");
            // Get a reference to the target StateMachineExecutor
            _stateMachineExecutor = (StateMachineExecutor)target;
            genericGraphView = (this._graphView as GenericGraphView);
            
            
            // Initialize the ListView
            PopulateTemplateListView();

            // Insert the ListView above the existing UI
            rootElement.Insert(0, _templateListView);

            return rootElement;
        }

        protected override List<BaseStatesNode> GetCurrentNodes() {
            return base.GetCurrentNodes();
        }

        protected override List<BaseStatesNode> GetAvailableNodes() {
            List<BaseStatesNode> nodes = new List<BaseStatesNode>();
            List<Type> availableNodeTypes = new List<Type>();

            foreach (Type type in (_graphView as GenericGraphView).AvailableNodeTypes) {
                if (typeof(INodeProvider<BaseStatesNode>).IsAssignableFrom(type)) {
                    INodeProvider<BaseStatesNode> nodeProvider = (INodeProvider<BaseStatesNode>)Activator.CreateInstance(type);
                    BaseStatesNode node = nodeProvider.GetNode();
                    nodes.Add(node);
                    availableNodeTypes.Add(node.GetType());
                }
                else if(typeof(ScriptableObject).IsAssignableFrom(type)) {
                    BaseStatesNode newNode = new BaseStatesNode(ScriptableObject.CreateInstance(type));
                    nodes.Add(newNode);
                    availableNodeTypes.Add(newNode.GetType());
                }
            }
            
            return nodes;
        }

        private void PopulateTemplateListView() {
            _templateListView.itemsSource = _stateMachineExecutor._stateMachineTemplates;
            _templateListView.bindItem = (element, i) => {
                element.Clear();
                element.Add(new Label(_stateMachineExecutor._stateMachineTemplates[i].name));
            };
            _templateListView.onSelectionChange += OnTemplateSelectionChanged;
            _templateListView.onItemsChosen += OnTemplateChosen;
        }

        protected virtual void OnTemplateSelectionChanged(IEnumerable<object> selectedItems) {
            // Implement your logic for handling the selection change
        }

        protected virtual void OnTemplateChosen(object chosenItem) {
            StateMachineTemplate chosenTemplate = chosenItem as StateMachineTemplate;
            if (chosenTemplate != null) {
                // Implement your logic for adding or removing the chosen template
            }
        }
    }
}
