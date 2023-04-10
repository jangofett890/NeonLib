using NeonLib.Events;
using UnityEngine;

namespace NeonLib.States {
    public class Behavior : ScriptableObject {
        public GameEvent OnActivate;
        public GameEvent OnUpdate;
        public GameEvent OnDeactivate;
        public int Weight;

        public virtual void Activate() {
            OnActivate?.Invoke();
        }
        public virtual void Update() {
            OnUpdate?.Invoke();
        }
        public virtual void Deactivate() {
            OnDeactivate?.Invoke();
        }
    }
}