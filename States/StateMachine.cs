using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonLib.States {
    public class StateMachine : ScriptableObject {
        [SerializeField] private State _currentState;
        [SerializeField] private State _previousState;

        public void Initialize(State startingState) {
            _previousState = null;
            _currentState = startingState;
            _currentState.Enter();
        }

        public void ChangeState(State newState) {
            _currentState.Exit();
            _previousState = _currentState;
            _currentState = newState;
            _currentState.Enter();
        }

        public void Tick() {
            _currentState.Tick();
        }

        public void ResetStateMachine() {
            _currentState.Exit();
            _previousState = null;
            _currentState = null;
        }
    }
}