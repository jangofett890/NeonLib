using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeonLib.Events {
    public class GameEventRoleAttribute : Attribute {
        public string EventName { get; }
        public Role EventRole { get; }

        public GameEventRoleAttribute(string eventName, Role eventRole) {
            EventName = eventName;
            EventRole = eventRole;
        }

        public enum Role {
            Invoker,
            Listener,
        }
    }
}