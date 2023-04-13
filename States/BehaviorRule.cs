using NeonLib.Events;
using NeonLib.Graphing;
using NeonLib.Graphing.States;
using NeonLib.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeonLib.States {
    public class BehaviorRule : ScriptableObject, INodeProvider<BaseStatesNode> {
        public ScriptableVariable[] Variables;
        public string ComparisonID = "compare_equal";
        public ScriptableVariable[] ComparisonOperationParameters;

        private bool isActive = false;

        public bool CheckRule() {
            var comparisonMapping = ComparisonMappings.Mappings[ComparisonID];
            isActive = comparisonMapping.ComparisonFunction(Variables, ComparisonOperationParameters);
            return isActive;
        }

        public BaseStatesNode GetNode() {
            BehaviorRuleNode node = new(this);
            return node;
        }

        public class ComparisonMapping {
            public string ID { get; set; }
            public string DisplayName { get; set; }
            //Variables are anything that are exposed to BehaviorStateGraphView via the BehaviorStateNode
            //Parameters may be displayed depending on the comparison mapping, as well as the visibility may be mixed depending on the Comparison mapping
            public Func<ScriptableVariable[], ScriptableVariable[], bool> ComparisonFunction { get; set; }

            public ComparisonMapping(string id, string displayName, Func<ScriptableVariable[], ScriptableVariable[], bool> comparisonFunction) {
                ID = id;
                DisplayName = displayName;
                ComparisonFunction = comparisonFunction;
            }
        }

        public static class ComparisonMappings {
            public static Dictionary<string, ComparisonMapping> Mappings = new Dictionary<string, ComparisonMapping> {
                { "compare_equal", new ComparisonMapping("compare_equal", "Equals", (a, b) => Compare(a, b[0])) },
                { "compare_greater", new ComparisonMapping("compare_greater", "Greater Than", (a, b) => Compare(a, b[0])) },
                { "compare_less", new ComparisonMapping("compare_less", "Less Than", (a, b) => Compare(a, b[0])) },
                { "bool_and", new ComparisonMapping("bool_and", "AND", (a, b) => BoolOperation(a, b)) },
                { "bool_or", new ComparisonMapping("bool_or", "OR", (a, b) => BoolOperation(a, b)) },
                { "has_component", new ComparisonMapping("has_component", "Has Component", (a, b) => HasComponent(a, b)) },
                { "is_active", new ComparisonMapping("is_active", "Is Active", (a, b) => IsActive(a)) },
                { "game_event_raised", new ComparisonMapping("game_event_raised", "Game Event Raised", (a, b) => RegisterGameEvents(a, b)) },
                { "variable_modified", new ComparisonMapping("variable_modified", "Variable Modified", (a, b) => VariableModified(a)) },
                { "in_range", new ComparisonMapping("in_range", "In Range", (a, b) => InRange(a, b)) },
                { "is_layer", new ComparisonMapping("is_layer", "Is Layer", (a, layer) => IsLayer(a, layer)) },
                // Add other mappings here
            };
            private static bool RegisterGameEvents(ScriptableVariable[] gameEvents, ScriptableVariable[] listenersAndRule) {
                if (gameEvents.Length != listenersAndRule.Length - 1) {
                    return false;
                }
                BehaviorRule rule = (BehaviorRule)listenersAndRule[listenersAndRule.Length - 1].Value;
                if (rule != null) {
                    for (int i = 0; i < gameEvents.Length; i++) {
                        GameEvent gameEvent = gameEvents[i].Value as GameEvent;
                        GameEventListener listener = listenersAndRule[i].Value as GameEventListener;

                        if (gameEvent != null && listener != null) {
                            listener.ChangeEvent(gameEvent);
                        }
                    }
                }
                return true;
            }
            private static bool IsLayer(ScriptableVariable[] a, ScriptableVariable[] layer) {
                bool useAll = (bool)layer[1].Value;
                bool toReturn = false;
                if (useAll) {
                    toReturn = a.All(variable => variable.Value is GameObject gameObject && gameObject.layer == (int)layer[0].Value);
                } else {
                    toReturn = a.Any(variable => variable.Value is GameObject gameObject && gameObject.layer == (int)layer[0].Value);
                }
                return toReturn;
            }

            private static bool InRange(ScriptableVariable[] a, ScriptableVariable[] b) {
                Vector3 pointA = (Vector3)a[0].Value;
                Vector3 pointB = (Vector3)a[1].Value;
                float range = (float)b[0].Value;
                return Vector3.Distance(pointA, pointB) <= range;
            }

            private static bool VariableModified(ScriptableVariable[] a) {
                throw new NotImplementedException();
            }
            private static bool IsActive(ScriptableVariable[] GameObjects) {
                return GameObjects.Any(variable => variable.Value is GameObject gameObject && gameObject.activeInHierarchy);
            }

            private static bool HasComponent(ScriptableVariable[] GameObjects, ScriptableVariable[] ComponentsRequired) {
                return GameObjects.All(variable => variable.Value is GameObject gameObject && ComponentsRequired.All(componentVariable => gameObject.GetComponent(componentVariable.ValueType) != null));
            }

            private static bool BoolOperation(ScriptableVariable[] Values, ScriptableVariable[] Operators) {
                bool result = (bool)Values[0].Value;
                for (int i = 1; i < Values.Length; i++) {
                    bool value = (bool)Values[i].Value;
                    string op = Operators[i - 1].Value as string;
                    if (op == "AND") {
                        result = result && value;
                    }
                    else if (op == "OR") {
                        result = result || value;
                    }
                }
                return result;
            }

            private static bool Compare(ScriptableVariable[] Values, ScriptableVariable Operation) {
                string operation = Operation.Value as string;
                IComparable valueA = (IComparable)Values[0].Value;
                IComparable valueB = (IComparable)Values[1].Value;

                int comparisonResult = valueA.CompareTo(valueB);
                switch (operation) {
                    case "compare_equal":
                        return comparisonResult == 0;
                    case "compare_greater":
                        return comparisonResult > 0;
                    case "compare_less":
                        return comparisonResult < 0;
                    default:
                        return false;
                }
            }
        }
    }
 }
