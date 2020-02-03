using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    class IDAStar<D> : SearchAlg<D> where D : ISearchDomain
    {
        List<IState> path;
        int bound;
        int minoob;
        Stopwatch stopwatch;

        public IDAStar(D d):base(d)
        {
            path = new List<IState>();
            stopwatch = new Stopwatch();
          
        }

        public override List<IState> Search(IState startState, int? tMax = null)
        {
            stopwatch.Start();

            bound = d.H(startState);
           
            int n = 0;

          
            do
            {
                minoob = -1;

                bool? goal = DFS(startState, 0, new Operation(),tMax);

                if(goal==null)
                {
                    break;
                       
                }
               
                n++;

                bound = minoob;

            } while (path.Count == 0);

            path.Reverse();

            stopwatch.Stop();

            return path;
        }

        private bool? DFS(IState state, int cost, Operation pop, int? tMax=null)
        {
            if (tMax != null && stopwatch.ElapsedMilliseconds > tMax)
            {
               return  null;
            }


            int f = cost + d.H(state);

            if (f <= bound && d.IsGoal(state))
            {
                path.Add(state.Clone());
                return true;
            }

            if (f > bound)
            {
                if (minoob < 0 || f < minoob)
                {
                    minoob = f;
                }

                return false;
            }
            expanded++;

            Operation[] ops = d.Operations(state);

            for (int i = 0; i < ops.Length; i++)
            {
                Operation op = ops[i];

                if (op.Equals(pop))
                {
                    continue;
                }

                generated++;

                Edge e = d.Apply(ref state, op);
                
                bool? goal = DFS(state, e.Cost + cost, e.POP,tMax);

                d.Undo(ref state, e);

                if (goal==true)
                {
                    path.Add(state.Clone());
                    return true;
                }

                if(goal==null)
                {
                    return null;
                }
            }

            return false;
        }

        public long Expanded
        {
            get { return expanded; }
        }

        public long Generated
        {
            get { return generated; }
        }

        public Stopwatch SwopWatch
        {
            get { return stopwatch; }
        }
    }

 


}
