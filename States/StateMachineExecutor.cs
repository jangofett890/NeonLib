using NeonLib.Graphing;
using NeonLib.Graphing.States;
using NeonLib.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[assembly: InternalsVisibleTo("NeonLib.Editor")]
namespace NeonLib.States {
    public class StateMachineExecutor : MonoBehaviour, IGraphViewCurrentNodesProvider<BaseStatesNode> {
        [SerializeField] 
        internal List<StateMachineTemplate> _stateMachineTemplates = new();

        internal StateMachine _stateMachine;

        internal List<State> _availableStates;

        private void Start() {
            _stateMachine = ScriptableObject.CreateInstance<StateMachine>();

            foreach (var template in _stateMachineTemplates) {
                template.Activate(_stateMachine);
            }
        }

        private void Update() {
            _stateMachine.Tick();
        }

        private void UpdateAvailableStates() {
            _availableStates = new List<State>();
            foreach(StateMachineTemplate template in _stateMachineTemplates) {
                foreach(State state in template.AvailableStates) {
                    _availableStates.Add(state);
                }
            }
        }
        
        public void AddTemplate(StateMachineTemplate template) {
            if (_stateMachineTemplates.Contains(template))
                return;

            _stateMachineTemplates.Add(template);
            template.Activate(_stateMachine);
            foreach (State state in template.AvailableStates) {
                _availableStates.Add(state);
            }
        }
        public void RemoveTemplate(StateMachineTemplate template) {
            if (!_stateMachineTemplates.Contains(template))
                return;

            _stateMachineTemplates.Remove(template);
            template.Deactivate(_stateMachine);
            foreach (State state in template.AvailableStates) {
                _availableStates.Remove(state);
            }
        }

        public List<BaseStatesNode> GetCurrentNodes() {
            UpdateAvailableStates();
            List<BaseStatesNode> nodes = new List<BaseStatesNode>();
            foreach (State s in _availableStates) {
                if(s as INodeProvider<BaseStatesNode> != null) {
                    nodes.Add((s as INodeProvider<BaseStatesNode>).GetNode());
                } else {
                    nodes.Add(new BaseStatesNode(s));
                }
            }
            return nodes;
        }
    }
}
