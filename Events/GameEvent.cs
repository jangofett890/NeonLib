using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeonLib.Events {
    [CreateAssetMenu(fileName = "GameEvent", menuName = "NeonLib/Events/Game Event", order = 0)]
    public class GameEvent : ScriptableObject {
        private List<GameEventListener> listeners = new List<GameEventListener>();
        public string InstanceID { get; private set; }
        public static Dictionary<string, GameEvent> Instances = new Dictionary<string, GameEvent>();

        public void Invoke(params object[] Arguments) {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventInvoked(Arguments);
            }
        }
        public GameEvent Instantiate(string instanceID) {
            if (Instances.ContainsKey(instanceID)) {
                NeonLib.Debugging.NeonDebug.Log("Events", "An instance with the given ID already exists. Returning the existing instance.");
                return Instances[instanceID];
            }
            var instance = ScriptableObject.CreateInstance<GameEvent>();
            instance.InstanceID = instanceID;
            foreach (var listener in listeners) {
                listener.OnEventInstanceCreated(instance);
            }
            Instances.Add(instanceID, instance);
            return instance;
        }
        public static List<GameEvent> GetInstancesByName(string eventName) {
            return Instances.Values.Where(instance => instance.name == eventName).ToList();
        }
        public void RegisterListener(GameEventListener listener) {
            if (!listeners.Contains(listener)) listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener) {
            if (listeners.Contains(listener)) listeners.Remove(listener);
        }

        public void OnDestroy() {
            if (!string.IsNullOrEmpty(InstanceID)) {
                Instances.Remove(InstanceID);
            }
        }
    }
}