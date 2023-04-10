using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeonLib.Events;
using UnityEngine.Events;

namespace NeonLib.Gameplay.MicroGames {
    public class MicroGameEventHandler : MonoBehaviour {

        private MicroGame microGame;

        public MicroGame MicroGame => microGame;

        public GameEventListener StartGame, BeforeStart, OnEnd;

        public void LoadMicroGame(MicroGame m) {
            microGame = m;

            StartGame?.ChangeEvent(microGame.StartGameEvent);
            BeforeStart?.ChangeEvent(microGame.BeforeStartEvent);
            OnEnd?.ChangeEvent(microGame.OnEndEvent);
        }
    }
}

