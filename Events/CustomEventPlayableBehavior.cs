using NeonLib.Gameplay.MicroGames;
using NeonLib.States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NeonLib.Events {
    public class CustomEventPlayableBehaviour : PlayableBehaviour {
        public CustomEventPlayable.PlayableClipType clipType;
        public GameEvent gameEvent;
        public MicroGameManager microGameManager;
        public State targetState;
        private bool eventInvoked;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
            if (!eventInvoked && playable.GetTime() > 0) {
                gameEvent?.Invoke();
                if (clipType == CustomEventPlayable.PlayableClipType.MicroGameEvent && microGameManager != null && targetState != null) {
                    microGameManager.CurrentMicroGame.StateMachine.ChangeState(targetState);
                }
                eventInvoked = true;
            }
        }
    }
}