using NeonLib.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeonLib.Templates {
    [CreateAssetMenu(fileName = "StateMachineTemplate", menuName = "NeonLib/Templates/State Machine Template")]
    public class StateMachineTemplate : BaseTemplate {
        //This list should be populated by the state machine editor, templates should be loadable by their respective object's editor, or one that mimics it
        public List<State> AvailableStates = new List<State>();

        public int StartingStateIndex = 0;

        public override void Activate(object currentObject) {
            base.Activate(currentObject);
            StateMachine stateMachine = (StateMachine)currentObject;
            State startingState = AvailableStates[StartingStateIndex];
            if (startingState != null) {
                stateMachine.Initialize(startingState);
            }
        }

        public override void Deactivate(object currentObject) {
            base.Deactivate(currentObject);
            StateMachine stateMachine = (StateMachine)currentObject;
            stateMachine.ResetStateMachine();
        }
    }
}
