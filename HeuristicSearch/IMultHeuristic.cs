using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.Heuristics
{
    interface IMultHeuristic
    {
        int[] HArr { get; }
        float[] Scale { get; }
        void H(IState state);
    }
}
