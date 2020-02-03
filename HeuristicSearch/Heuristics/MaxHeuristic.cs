using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.Heuristics
{
    class MaxHeuristic : IHeuristic
    {
        IMultHeuristic multHeuristic;

        public MaxHeuristic(IMultHeuristic multHeuristic)
        {
            this.multHeuristic = multHeuristic;

        }

        public void ClearCache()
        {
            return;
        }

        public virtual int H(IState state,bool verbose=false)
        {
            multHeuristic.H(state);
            return multHeuristic.HArr.Max();    
        }

        public void Update(List<IState>[] paths)
        {
            return;
        }
    }
}
