using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NeonLib.Variables {
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ShowInScriptableVariableAttribute : Attribute {

        public DateTime Timestamp { get; private set; }

        public string DisplayName { get; }
        public Func<VisualElement> FieldCreator { get; }
        public Action OnTypeSelected { get; }

        public ShowInScriptableVariableAttribute(
            string displayName,
            Func<VisualElement> fieldCreator = null,
            Action onTypeSelected = null) {
            DisplayName = displayName;
            FieldCreator = fieldCreator;
            OnTypeSelected = onTypeSelected;
            Timestamp = DateTime.UtcNow;
        }
    }
}
