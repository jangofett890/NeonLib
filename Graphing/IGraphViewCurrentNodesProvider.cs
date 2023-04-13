using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeonLib.Graphing {
    public interface IGraphViewCurrentNodesProvider<T> {
        List<T> GetCurrentNodes();
    }
}
