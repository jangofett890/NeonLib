using NeonLib.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonLib.Variables {
    [CreateAssetMenu(fileName ="RuntimeSet", menuName = "NeonLib/Variables/Runtime Set")]
    public class RuntimeSet : ScriptableObject {
        public List<ScriptableVariable> Items = new List<ScriptableVariable>();

        public GameEvent OnListChanged;
        public GameEvent OnItemAdded;
        public GameEvent OnItemRemoved;

        public void Add(ScriptableVariable t) {
            if (Items.Contains(t))
                return;

            int count = Items.Count;
            Items.Add(t);
            OnListChanged?.Invoke(count);
            OnItemAdded?.Invoke(t);
        }
        public void Remove(ScriptableVariable t) {
            if (!Items.Contains(t))
                return;
            int count = Items.Count;
            Items.Remove(t);
            OnListChanged?.Invoke(count);
            OnItemRemoved?.Invoke(t);
        }
    }
}