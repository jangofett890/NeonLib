using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace NeonLib.Graphing.States {
    public class BaseStatesNode : Node {
        [SerializeReference]
        private UnityEngine.Object _target;

        //The Dataobject that this node is associated with, this could mean we could have GameObject nodes, Prefab Nodes, as well as ScriptableObject type nodes like State and Behavior as nodes on a graph.
        public UnityEngine.Object Target {
            get { return _target; }
            set { _target = value; }
        }

        public BaseStatesNode() : this(null) { }

        public BaseStatesNode(UnityEngine.Object Target) : base() {
            _target = Target;

            if (Target == null)
                return;

            base.title = Target.name;
        }

    }
}
