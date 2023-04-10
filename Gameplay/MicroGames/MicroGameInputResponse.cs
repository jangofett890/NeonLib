using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using NeonLib.Events;
using NeonLib.States;

namespace NeonLib.Gameplay.MicroGames {
    public class MicroGameInputResponse : ScriptableObject {

        public enum InputResponseType {
            GameEvent, //Invokes GameEvent
            UOMethodInvoker, //Inlines the UOMethodInvoker Editor to allow it's use of Reflection to target any object and invoke any function. This is may warrant a name change of GameEventResponse.
            UnityEvent, //Invokes a UnityEvent
            //SendMessage, Functionality provided by UnityEvent
            ChangeState, //Targets a StateMachine and changes it's state using the ChangeState method
            MicroGameManager //Uses the static CurrentMicroGameManager from the MicroGameManager monobehavior to start or stop a new or current microgame.
        }
        [SerializeReference]
        private MicroGame microGame;
        
        public MicroGame MicroGame => microGame;

        public InputActionReference InputAction;

        public UnityEvent UnityEvent;
        [SerializeReference]
        public GameEvent GameEvent;

        public UOMethodInvoker MethodInvoker;
        [SerializeReference]
        public State StateToChangeTo;
        [SerializeReference]
        public StateMachine StateMachineTarget;

        public InputResponseType ResponseType;

        public void Initialize(MicroGame microGame) {
            this.microGame = microGame;
            InputAction.action.performed += OnInputAction;
            InputAction.action.Enable();
        }

        public void Cleanup() {
            microGame = null;
            InputAction.action.performed -= OnInputAction;
            InputAction.action.Disable();
        }

        private void OnInputAction(InputAction.CallbackContext ctx) {
            switch (ResponseType) {
                case InputResponseType.GameEvent:
                    GameEvent?.Invoke();
                    break;
                case InputResponseType.UOMethodInvoker:
                    MethodInvoker?.Invoke(null);
                    break;
                case InputResponseType.UnityEvent:
                    UnityEvent?.Invoke();
                    break;
                case InputResponseType.ChangeState:
                    StateMachineTarget?.ChangeState(StateToChangeTo);
                    break;
                case InputResponseType.MicroGameManager:
                    // Handle MicroGameManager related logic
                    break;
            }
        }
    }
}
