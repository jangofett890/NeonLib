using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

namespace NeonLib.Graphing {
    public interface INodeProvider<N> where N : Node {
        public N GetNode();
    }
}
