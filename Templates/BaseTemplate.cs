using NeonLib.Events;
using NeonLib.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NeonLib.Templates {
    public abstract class BaseTemplate : ScriptableObject {

        public GameEvent OnTemplateActivated;
        public GameEvent OnTemplateDeactivated;

        public ScriptableVariable IsActive;

        // Add any common methods or properties for base templates here

        //Override and call base.Activate(currentObject) to implement overwriting the values of the given object
        public virtual void Activate(object currentObject) {
            if (IsActive != null) {
                IsActive.Value = true;
            }
            OnTemplateActivated?.Invoke();
        }
        //Override and call base.Deactivate(currentObject) to implement custom behavior to clean up the object
        public virtual void Deactivate(object currentObject) {
            if (IsActive != null) {
                IsActive.Value = false;
            }
            OnTemplateDeactivated?.Invoke();
        }

    }
}
