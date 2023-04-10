using NeonLib.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using NeonLib.States;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("NeonLib.Editor")]
namespace NeonLib.Gameplay.MicroGames {
    public class MicroGame : ScriptableObject {
        [SerializeField]
        private string developerName = "New MicroGame";
        [SerializeField]
        private string developerDescription = "A new game using the micro game system, a system reliant on neonlib, its event system, and dialog system.";

        public string DeveloperName => developerName;
        public string DeveloperDescription => developerDescription;

        [SerializeField]
        private Localization.LocalizedString inGameName;
        [SerializeField]
        private Localization.LocalizedString inGameDescription;

        public string InGameName => inGameName.Value;
        public string InGameDescription => inGameDescription.Value;

        [SerializeField]
        internal List<MicroGameInputResponse> inputResponses;

        public void InitializeInputResponses() {
            foreach (var inputResponse in inputResponses) {
                inputResponse.Initialize(this);
            }
        }

        public void CleanupInputResponses() {
            foreach (var inputResponse in inputResponses) {
                inputResponse.Cleanup();
            }
        }

        [SerializeField]
        private InputActionAsset microGameControlMap;
        public InputActionAsset MicroGameControls => microGameControlMap;

        [SerializeField]
        private StateMachine stateMachine;
        public StateMachine StateMachine => stateMachine;

        [SerializeField]
        private List<State> states = new List<State>();

        public List<State> States => states;

        [SerializeField]
        private GameEvent startGameEvent;
        [SerializeField]
        private GameEvent beforeStartEvent;
        [SerializeField]
        private GameEvent onEndEvent;

        public GameEvent StartGameEvent => startGameEvent;
        public GameEvent BeforeStartEvent => beforeStartEvent;
        public GameEvent OnEndEvent => onEndEvent;

        private void OnEnable() {
            if (stateMachine == null) {
                stateMachine = CreateInstance<StateMachine>();
                stateMachine.name = "State Machine";
            }
            if (startGameEvent == null) {
                startGameEvent = CreateInstance<GameEvent>();
                startGameEvent.name = "On Game Start";
            }
            if (beforeStartEvent == null) {
                beforeStartEvent = CreateInstance<GameEvent>();
                beforeStartEvent.name = "Before Game Start";
            }
            if (onEndEvent == null) { 
                onEndEvent = CreateInstance<GameEvent>();
                onEndEvent.name = "On Game End";
            }
            if(states.Count == 0) {
                State defaultState = State.CreateInstance<State>();
                defaultState.name = "Default State";
                states.Add(defaultState);
            }
        }
    }
}
