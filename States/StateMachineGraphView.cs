using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

namespace NeonLib.States {
    public class StateMachineGraphView : GraphView {
        private UnityEngine.Object target;

        public StateMachineGraphView(UnityEngine.Object target) : base() {
            this.target = target;
        }
    }
}
