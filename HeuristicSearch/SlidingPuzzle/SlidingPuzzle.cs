using HeuristicSearch.Heuristics;
using HeuristicSearch.SlidingPuzzle.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HeuristicSearch.SlidingPuzzle
{

class SlidingPuzzle : SearchDomain, ISearchDomain
    {
        static readonly OpTab[] optab;
        public static readonly int DIM;
        public const int MAXBRANCHFACTOR = 4;


        public static Tuple<int, int> IndexToCoord(int index)
        {
            int y = (index / DIM) + 1;

            int x = (index % DIM) + 1;

            return new Tuple<int, int>(x, y);
        }

        static SlidingPuzzle()
        {
           DIM = (int)Math.Sqrt(Global.SIZE);
           optab = BuildOpTab();  
        }

        public SlidingPuzzle(IHeuristic heuristic, byte[] init) : base(heuristic,init)
        {
        }

      
        public SlidingPuzzle(IHeuristic heuristic,int steps) : base(heuristic, null)
        {
            this.numSteps = Generate(steps);
        }

        public SlidingPuzzle(NNBaseHeuristic heuristic, double maxUncert, int maxSteps=1000) : base(heuristic,null)
        {
            this.numSteps = Generate(maxUncert, heuristic,maxSteps);
        }

        public static OpTab[] BuildOpTab()
        {
            OpTab[] optab = new OpTab[Global.SIZE];

            for (int i = 0; i < Global.SIZE; i++)
            {
                optab[i] = new OpTab(MAXBRANCHFACTOR);
                optab[i].n = 0;
                if (i >= DIM)
                    optab[i].ops[optab[i].n++] = (byte) (i - DIM);
                if (i % DIM > 0)
                    optab[i].ops[optab[i].n++] = (byte)(i - 1);
                if (i % DIM < DIM - 1)
                    optab[i].ops[optab[i].n++] = (byte)(i + 1);
                if (i < Global.SIZE - DIM)
                    optab[i].ops[optab[i].n++] = (byte)(i + DIM);
              
            }

            return optab;
        }


        public override IState Goal()
        {
            IState state = new State();

            byte[] init = new byte[Global.SIZE];

            for (byte i = 0; i < init.Length; i++)
            {
                init[i] = i;
            }

            state.Arr = init;

            state.Op = new Operation(0);

            return state;
        }


        public override IState Initial()
        {
            State state = new State();

            for (byte i = 0; i < Global.SIZE; i++)
            {
                if (init[i] == 0)
                {
                    state.Op = new Operation(i);
                }

                state.Arr[i] = init[i];
            }

            if (heuristic != null)
            {
                state.H = heuristic.H(state,true);
            }

            return state;
        }

        public override int H(IState state)
        {
            return state.H;
        }

        public override bool IsGoal(IState state)
        {
            for (byte i = 0; i < Global.SIZE; i++)
            {         
                if (state.Arr[i] != i)
                {
                    return false;
                }
            }

            return true;
        }

        public override List<Edge> Expand(IState state)
        {         
            Operation[] ops = Operations(state);

            List<Edge> children = new List<Edge>(ops.Length);

            for (int i = 0; i < ops.Length; i++)
            {
                children.Add(Child(state, ops[i]));
            }

            return children;
        }

        public override Operation[] Operations(IState state)
        {
            int nops = optab[state.Op.Vals[0]].n;

            Operation[] ops = new Operation[nops];
            
            for(int i=0;i< nops;i++)
            {
                ops[i] = new Operation(optab[state.Op.Vals[0]].ops[i]);
            }

            return ops;
        }

        
        public override Edge Apply(ref IState state, Operation op)
        {
            Edge e = new Edge(Global.STEPCOST, op, state.Op);
            UndoToken u = new UndoToken();
            u.H = state.H;
            u.Op = state.Op;
            e.UndoToken = u;


            byte tile = state.Arr[op.Vals[0]];
            state.Arr[state.Op.Vals[0]] = tile;
            state.Op = op;
            state.Arr[op.Vals[0]] = 0;

            if (heuristic != null)
            {
                state.H = heuristic.H(state);
            }

            return e;
        }

        public override void Undo(ref IState state, Edge edge)
        {
            IUndoToken undo = edge.UndoToken;
            state.H = undo.H;
            state.Arr[state.Op.Vals[0]] = state.Arr[undo.Op.Vals[0]];
            state.Op = undo.Op;
            state.Arr[undo.Op.Vals[0]] = 0;
        }

        private Edge Child(IState state, Operation op)
        {
            IState child = new State();

            for (byte i = 0; i < Global.SIZE; i++)
            {
                child.Arr[i] = state.Arr[i];
            }

            child.Op = state.Op;

            Edge e = Apply(ref child, op);

            e.Child = child;

            return e;
            
        }
    }
}
