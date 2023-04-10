using NeonLib.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonLib.Variables {
    public class RuntimeSet<T> : ScriptableObject {
        public List<T> Items = new List<T>();

        public GameEvent OnListChanged;
        public GameEvent OnItemAdded;
        public GameEvent OnItemRemoved;

        public void Add(T t) {
            if (Items.Contains(t))
                return;

            int count = Items.Count;
            Items.Add(t);
            OnListChanged?.Invoke(count);
            OnItemAdded?.Invoke(t);
        }
        public void Remove(T t) {
            if (!Items.Contains(t))
                return;
            int count = Items.Count;
            Items.Remove(t);
            OnListChanged?.Invoke(count);
            OnItemRemoved?.Invoke(t);
        }
    }
}