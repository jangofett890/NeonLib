using NeonLib.Graphing;
using NeonLib.Graphing.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeonLib.States {
    public class BehaviorState : State, INodeProvider<BaseStatesNode> {
        [SerializeField] private List<Behavior> _behaviors;
        [SerializeField] private List<List<BehaviorRule>> _behaviorRules;

        private List<Behavior> _activeBehaviors = new List<Behavior>();

        public override BaseStatesNode GetNode() {
            BehaviorStateNode node = new(this);
            return node;
        }

        public void Initialize() {
            // does an Initial Check to allow any custom behaviors to initilize before doing first checks
            foreach (List<BehaviorRule> rules in _behaviorRules) {
                foreach (BehaviorRule rule in rules) {
                    rule.CheckRule();
                }
            }
        }

        public override void Enter() {
            base.Enter();
            EvaluateAndActivateDeactivateBehaviors();
        }

        public override void Exit() {
            base.Exit();
            DeactivateActiveBehaviors();
        }

        public override void Tick() {
            base.Tick();
            UpdateActiveBehaviors();
        }

        private void EvaluateAndActivateDeactivateBehaviors() {
            foreach (List<BehaviorRule> rules in _behaviorRules) {
                bool allRulesActive = true;
                foreach (BehaviorRule rule in rules) {
                    if (!rule.CheckRule()) {
                        allRulesActive = false;
                        break;
                    }
                }

                int index = _behaviorRules.IndexOf(rules);
                Behavior behavior = _behaviors[index];

                if (allRulesActive) {
                    if (!_activeBehaviors.Contains(behavior)) {
                        behavior.Activate();
                        _activeBehaviors.Add(behavior);
                    }
                }
                else {
                    if (_activeBehaviors.Contains(behavior)) {
                        behavior.Deactivate();
                        _activeBehaviors.Remove(behavior);
                    }
                }
            }
        }

        private void DeactivateActiveBehaviors() {
            foreach (Behavior behavior in _activeBehaviors) {
                behavior.Deactivate();
            }
            _activeBehaviors.Clear();
        }

        private void UpdateActiveBehaviors() {
            foreach (Behavior behavior in _activeBehaviors) {
                behavior.Update();
            }
        }
    }
}
