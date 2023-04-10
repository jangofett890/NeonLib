using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NeonLib.Events {
    [CreateAssetMenu(fileName = "GameEventListener", menuName = "NeonLib/Events/Game Event Listener")]
    public class GameEventListener : ScriptableObject {
        private GameEvent _event;
        public GameEvent Event { get { return _event; } private set { _event = value; } }
        public List<UOMethodInvoker> gameEventResponses = new List<UOMethodInvoker>();
        public UnityEvent UnityEventResponse;
        public bool ListenToInstancesOnly;
        public string AllowedInstanceID;

        private void OnEnable() {
            Event?.RegisterListener(this);
        }
        private void OnDisable() {
            Event?.UnregisterListener(this);
        }

        public void ChangeEvent(GameEvent e) {
            Event?.UnregisterListener(this);
            Event = e;
            Event.RegisterListener(this);
        }

        public void OnEventInstanceCreated(GameEvent instance) {
            if (ListenToInstancesOnly && instance.InstanceID == AllowedInstanceID) {
                ChangeEvent(instance);
            } else if(!ListenToInstancesOnly && instance.name == Event?.name) {
                instance.UnregisterListener(this);
            }
        }

        public void OnEventInvoked(params object[] o) {
            Debugging.NeonDebug.Log("Game Events", "Invoke " + UnityEventResponse + o);
            UnityEventResponse.Invoke();
            gameEventResponses.ForEach(response => { response.Invoke(o); });
        }
    }
}