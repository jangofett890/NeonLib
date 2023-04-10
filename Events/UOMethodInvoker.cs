using NeonLib.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
[assembly: InternalsVisibleTo("NeonLib.Editor")]
namespace NeonLib.Events {

    public class UOMethodInvoker : ScriptableObject {

        [SerializeReference]
        public UnityEngine.Object Target;

        public string MethodName;

        private MethodInfo m_MethodCache;

        [SerializeField]
        internal List<ScriptableVariable> arguments = new List<ScriptableVariable>();

        public IReadOnlyList<ScriptableVariable> Arguments => arguments;

        public MethodInfo TargetAction {
            get
            {
                if (m_MethodCache == null || m_MethodCache.Name != MethodName) {
                    m_MethodCache = Target.GetType().GetMethod(MethodName);
                }
                return m_MethodCache;
            }
        }

        public void Invoke(object[] o = null) {
            if (TargetAction != null) {
                // Prepare the parameter array from the SerializedArgument list or use the provided parameters
                object[] parameters = o ?? new object[arguments.Count];
                if (o == null) {
                    for (int i = 0; i < arguments.Count; i++) {
                        parameters[i] = arguments[i].Value;
                    }
                }

                // Invoke the TargetAction with the prepared parameters
                TargetAction.Invoke(Target, parameters);
            }
        }
    }
}