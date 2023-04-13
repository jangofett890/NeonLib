using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

namespace NeonLib.Graphing.States {
    public class StateMachineGraphView : GenericGraphView {
        private List<Type> availableNodeTypes = new List<Type>() {
            typeof(StateNode), typeof(BehaviorStateNode)
        };
        public override List<Type> AvailableNodeTypes { 
            get {
                List<Type> available = new List<Type>();
                available.AddRange(availableNodeTypes);
                available.AddRange(base.AvailableNodeTypes);
                return available; 
            } 
        }
    }
}
