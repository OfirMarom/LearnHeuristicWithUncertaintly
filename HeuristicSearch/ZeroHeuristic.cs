using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.Heuristics
{
    class ZeroHeuristic : IHeuristic
    {
        public int H(IState state, bool verbose = false)
        {
            return 0;
        }


        public void Update(List<IState>[] paths)
        {
            return;
        }

        public void ClearCache()
        {
            return;
        }

    }
}
