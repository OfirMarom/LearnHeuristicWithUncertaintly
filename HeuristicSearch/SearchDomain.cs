using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    abstract class SearchDomain : ISearchDomain
    {

       protected  byte[] init;
       protected IHeuristic heuristic;
       protected int? numSteps;


        public SearchDomain(IHeuristic heuristic, byte[] init)
        {
            this.heuristic = heuristic;
            this.init = init;
            this.numSteps = null;
        }

        public virtual List<Edge> Expand(IState state)
        {
            return null;
        }

        public int? NumSteps
        {
            get { return numSteps; }
        }


        public virtual IState Goal()
        {
            return null;
        }

        public virtual bool IsGoal(IState state)
        {
            return false;
        }

        public virtual int H(IState state)
        {
            return 0;
        }

        public virtual Operation[] Operations(IState state)
        {
            return null;
        }

        

        public virtual Edge Apply(ref IState state, Operation op)
        {
            return new Edge();
        }

        public virtual void Undo(ref IState state, Edge edge)
        {
            return;
        }

        public virtual IState Initial()
        {
            return null;
        }
       

        protected int Generate(int steps)
        {

            IState state = Goal();

            Operation? pop = null;

            for (int i = 0; i < steps; i++)
            {
                List<Edge> children = Expand(state);

                if (pop != null)
                {
                    children = children.Where(x => !x.OP.Equals(pop) ).ToList();
                }


                int index = Global.Random.Next(0, children.Count);
                Edge e = children[index];
                state = e.Child;
                pop = e.POP;

            }

            Console.WriteLine("steps: " + steps);

            this.init = state.Arr;

            return steps;
        }

        protected int Generate(double maxUncert, NNBaseHeuristic nn, int maxSteps = 1000)
        {
            IState state = Goal();

            Operation? pop = null;

            int count = 0;

            double uncert = 0;

            while (uncert < maxUncert)
            {
                count++;

                if (count >= maxSteps)
                {
                    break;
                }

                List<Edge> children = Expand(state);

                if (pop != null)
                {
                    children = children.Where(x =>  !x.OP.Equals(pop)).ToList();
                }

                SummaryStatistics[] summaryStatistics = nn.GetSummaryStatistics(children.Select(x => x.Child).ToArray());
              
                double[] stateUncerts = new double[summaryStatistics.Length];

                for (int i = 0; i < summaryStatistics.Length; i++)
                {
                    stateUncerts[i] = summaryStatistics[i].SDModel;
                }

                double norm = stateUncerts.Sum(x => Math.Exp(x));

                double[] weights = stateUncerts.Select(x => Math.Exp(x) / norm).ToArray();

                int index = Global.Sample(weights);

                Edge e = children[index];

                state = e.Child;

                uncert = Math.Max(uncert, stateUncerts[index]);

                pop = e.POP;
            }

            Console.WriteLine("steps: " + count);

            this.init = state.Arr;

            return count;

        }


    }
}
