using NeonLib.Gameplay.MicroGames;
using NeonLib.States;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace NeonLib.Events {
    [CreateAssetMenu(fileName = "CustomEventPlayable", menuName = "Timeline/CustomGameEventPlayable")]
    public class CustomEventPlayable : PlayableAsset, ITimelineClipAsset {

        public enum PlayableClipType {
            SimpleEvent,
            MicroGameEvent
        }

        public PlayableClipType clipType;
        public GameEvent gameEvent;
        public MicroGameManager microGameManager;
        public State targetState;

        public ClipCaps clipCaps {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
            var playable = ScriptPlayable<CustomEventPlayableBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();

            behaviour.clipType = clipType;
            behaviour.gameEvent = gameEvent;
            behaviour.microGameManager = microGameManager;
            behaviour.targetState = targetState;

            return playable;
        }
    }
}