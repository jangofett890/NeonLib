using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeonLib.States;

namespace NeonLib.Gameplay.MicroGames {
    [RequireComponent(typeof(MicroGameEventHandler))]
    public class MicroGameManager : MonoBehaviour {

        public static MicroGameManager CurrentMicroGameManager;

        private MicroGame currentMicroGame;

        private bool microGameInProgress;

        private MicroGameEventHandler eventHandler;

        // A reference to the current MicroGame ScriptableObject being used
        public MicroGame CurrentMicroGame => currentMicroGame;

        // A dictionary to cache all MicroGame ScriptableObjects
        private Dictionary<string, MicroGame> _microGameDictionary;


        private void OnEnable() {
            if(CurrentMicroGameManager != null) {
                Destroy(this);
                return;
            }
            CurrentMicroGameManager = this;
        }


        private void Awake() {

            eventHandler = GetComponent<MicroGameEventHandler>();

            // Cache all MicroGame ScriptableObjects found in the Resources folder
            CacheMicroGames();

            // Load the initial MicroGame if one is assigned
            if (CurrentMicroGame != null) {
                StartMicroGame(CurrentMicroGame.DeveloperName);
            }
        }

        private void Update() {
            // Update the current state in the state machine
            currentMicroGame?.StateMachine?.Tick();
        }

        private void CacheMicroGames() {
            _microGameDictionary = new Dictionary<string, MicroGame>();
            MicroGame[] microGames = Resources.LoadAll<MicroGame>("");

            foreach (var microGame in microGames) {
                _microGameDictionary.Add(microGame.name, microGame);
            }
        }


        public void StartMicroGame(string microGameName) {
            if (microGameInProgress) {
                Debug.LogWarning("A MicroGame is already in progress. Please end the current MicroGame before starting a new one.");
                return;
            }

            MicroGame microGameToStart = _microGameDictionary[microGameName];

            if (microGameToStart == null) {
                Debug.LogWarning($"MicroGame with the name '{microGameName}' not found.");
                return;
            }

            StartMicroGame(microGameToStart);
        }

        public void StartMicroGame(MicroGame microGame) {
            currentMicroGame = microGame;
            eventHandler.LoadMicroGame(currentMicroGame);
            microGameInProgress = true;
            CurrentMicroGame.BeforeStartEvent.Invoke();
            CurrentMicroGame.StateMachine.Initialize(CurrentMicroGame.States[0]);
            CurrentMicroGame.StartGameEvent.Invoke();
            CurrentMicroGame.InitializeInputResponses();
        }

        public void EndMicroGame() {
            if (!microGameInProgress) {
                Debug.LogWarning("There is no MicroGame in progress to end.");
                return;
            }

            CurrentMicroGame.OnEndEvent.Invoke();
            CurrentMicroGame.StateMachine.ResetStateMachine();
            CurrentMicroGame.CleanupInputResponses();

            microGameInProgress = false;
            currentMicroGame = null;
        }

        public bool IsMicroGameInProgress() {
            return microGameInProgress;
        }

        public MicroGame GetCurrentMicroGame() {
            return CurrentMicroGame;
        }
    }
}