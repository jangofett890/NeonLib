using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NeonLib.Debugging {
    [CreateAssetMenu]
    public class LogChannel : ScriptableObject {
        public string Name;
        public string Description;
        public Color PrintColor;
        public UnityEvent<string> OnLog;
        public bool Enabled;
        public LogChannel() {
            if(OnLog == null)
                OnLog = new UnityEvent<string>();
        }

        public void Log(string msg) {
            OnLog.Invoke(msg);
        }
    }
}