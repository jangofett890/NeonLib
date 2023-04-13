using NeonLib.Events;
using NeonLib.Graphing;
using NeonLib.Graphing.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonLib.States {
    public class State : ScriptableObject, INodeProvider<BaseStatesNode> {

        public GameEvent OnStateEnter;
        public GameEvent OnStateExit;
        public GameEvent OnStateUpdate;

        public virtual void Enter()  {
            OnStateEnter?.Invoke();
        }

        public virtual void Exit() {
            OnStateExit?.Invoke();
        }

        public virtual void Tick()  {
            OnStateUpdate?.Invoke();
        }
        public virtual BaseStatesNode GetNode() {
            StateNode node = new StateNode(this);
            return node;
        }
    }
}