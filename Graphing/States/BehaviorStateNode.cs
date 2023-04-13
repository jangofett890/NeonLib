using UnityEngine;

namespace NeonLib.Graphing.States {
    public class BehaviorStateNode : StateNode {
        public BehaviorStateNode(Object Target) : base(Target) {
            this.name = "Behavior State";
        }
    }
}