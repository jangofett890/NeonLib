using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeonLib.Editor {
    public interface IGraphViewNodeProvider<T> {
        List<T> GetCurrentNodes();
    }
}
