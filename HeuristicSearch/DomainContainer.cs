using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    class DomainContainer
    {
        ISearchDomain domain;

        public ISearchDomain Domain
        {
            get { return domain; }
        }

        public DomainContainer(IHeuristic heuristic, byte[] init) 
        {
            if(Global.DOMAINTYPE == typeof(SlidingPuzzle.SlidingPuzzle))
            {
                domain = new SlidingPuzzle.SlidingPuzzle(heuristic, init);
            }
            else if (Global.DOMAINTYPE == typeof(PancakePuzzle.PancakePuzzle))
            {
                domain = new PancakePuzzle.PancakePuzzle(heuristic, init);
            }
            else if (Global.DOMAINTYPE == typeof(BlocksWorld.BlocksWorld))
            {
                domain = new BlocksWorld.BlocksWorld(heuristic, init);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }


        public DomainContainer(IHeuristic heuristic, int steps)
        {
            if (Global.DOMAINTYPE == typeof(SlidingPuzzle.SlidingPuzzle))
            {
                domain = new SlidingPuzzle.SlidingPuzzle(heuristic, steps);
            }
            else if (Global.DOMAINTYPE == typeof(PancakePuzzle.PancakePuzzle))
            {
                domain = new PancakePuzzle.PancakePuzzle(heuristic, steps);
            }
            else if (Global.DOMAINTYPE == typeof(BlocksWorld.BlocksWorld))
            {
                domain = new BlocksWorld.BlocksWorld(heuristic, steps);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public DomainContainer(NNBaseHeuristic heuristic, double maxUncert, int maxSteps = 1000) 
        {
            if (Global.DOMAINTYPE == typeof(SlidingPuzzle.SlidingPuzzle))
            {
                domain = new SlidingPuzzle.SlidingPuzzle(heuristic, maxUncert,maxSteps);
            }
            else if (Global.DOMAINTYPE == typeof(PancakePuzzle.PancakePuzzle))
            {
                domain = new PancakePuzzle.PancakePuzzle(heuristic, maxUncert, maxSteps);
            }
            else if (Global.DOMAINTYPE == typeof(BlocksWorld.BlocksWorld))
            {
                domain = new BlocksWorld.BlocksWorld(heuristic, maxUncert,maxSteps);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }


    }
}
