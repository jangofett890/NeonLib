using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using NeonLib.States;
using UnityEditor;

namespace NeonLib.Editor {
    [CustomEditor(typeof(States.StateMachine), true)]
    public class StateMachineEditor : UnityEditor.Editor{
        private StateMachineGraphView _graphView;
        private List<State> _states;
        private List<BehaviorState> _behaviorStates;

        public override VisualElement CreateInspectorGUI() {
            return base.CreateInspectorGUI();
        }
    }
}
