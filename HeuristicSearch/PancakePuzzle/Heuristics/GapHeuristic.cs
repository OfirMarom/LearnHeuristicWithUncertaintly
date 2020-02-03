using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.PancakePuzzle.Heuristics
{
    class GapHeuristic : IHeuristic
    {
        public int H(IState state, bool verbose = false)
        {
            byte[] arr = state.Arr;

            int count = 0;
     
            for(int i=0;i<arr.Length;i++)
            {
                int curr = arr[i];

                int next;

                if(i==arr.Length-1)
                {
                    next = arr.Length;
                }
                else
                {
                    next = arr[i + 1];
                }

                int diff = curr - next;

                if(diff>1 || diff <-1)
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
