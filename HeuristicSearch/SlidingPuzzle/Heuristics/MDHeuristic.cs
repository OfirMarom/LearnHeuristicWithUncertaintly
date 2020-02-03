using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.SlidingPuzzle.Heuristics
{
    class MDHeuristic : IHeuristic
    {
        int[,] md = new int[Global.SIZE, Global.SIZE];
        readonly int DIM;
        int mdMax;

      

        private int ManhattenDistCached(Operation op, byte[] tiles)
        {
            int sum = 0;

            for (int i = 0; i < Global.SIZE; i++)
            {
                if (i == op.Vals[0])
                {
                    continue;
                }

                sum += md[i, tiles[i]];
            }

            return sum;
        }

        private void InitMD()
        {
            List<int> allVals = new List<int>();

            for (int t = 0; t < Global.SIZE; t++)
            {
                int grow = t / DIM;
                int gcol = t % DIM;
                for (int l = 0; l < Global.SIZE; l++)
                {
                    int row = l / DIM;
                    int col = l % DIM;
                    int val = Math.Abs(col - gcol) + Math.Abs(row - grow);
                    md[t, l] = val;

                    allVals.Add(val);
                }
            }

            allVals = allVals.OrderByDescending(x=>x).ToList();

            mdMax = allVals.Take(Global.SIZE - 1).Sum();
        }


        public MDHeuristic()
        {
            mdMax = 0;
            DIM = (int)Math.Sqrt(Global.SIZE);
            InitMD();
        }

        public int MDMax
        {
            get { return mdMax; }
        }



        public int H(IState state, bool verbose = false)
        {
            byte[] arr = state.Arr;
            Operation op = state.Op;
            int h = ManhattenDistCached(op, arr);
            return h;

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
