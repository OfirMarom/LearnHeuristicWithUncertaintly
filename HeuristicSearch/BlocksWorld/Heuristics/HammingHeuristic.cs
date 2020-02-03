using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.BlocksWorld.Heuristics
{
    class HammingHeuristic : IHeuristic
    {

        public int H(IState state, bool verbose=false)
        {
            byte[] arr = state.Arr;

            int count = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                if (i != arr[i])
                {
                    count++;
                }
            }

            return count;
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
