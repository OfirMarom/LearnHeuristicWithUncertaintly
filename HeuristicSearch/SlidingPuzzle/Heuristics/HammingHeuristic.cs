using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.SlidingPuzzle.Heuristics
{
    class HammingHeuristic : IHeuristic
    {

        public int H(IState state, bool verbose = false)
        {
            byte[] tiles = state.Arr;

            int count = 0;

            for (int i=0;i<tiles.Length;i++)
            {
                if(tiles[i]==0)
                {
                    continue;
                }

                if(i != tiles[i])
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
