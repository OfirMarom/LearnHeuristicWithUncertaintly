using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.Heuristics
{
    interface IHeuristic
    {
        int H(IState state, bool verbose=false);
        void Update(List<IState>[] paths);
        void ClearCache();
    }
}
