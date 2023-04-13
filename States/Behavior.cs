using NeonLib.Events;
using NeonLib.Graphing;
using NeonLib.Graphing.States;
using UnityEngine;

namespace NeonLib.States {
    public class Behavior : ScriptableObject, INodeProvider<BaseStatesNode> {
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

        public BaseStatesNode GetNode() {
            BehaviorNode node = new(this);
            return node;
        }
    }
}