using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.PancakePuzzle
{
    class PancakePuzzle : SearchDomain, ISearchDomain
    {

        public PancakePuzzle(IHeuristic heuristic, byte[] init) : base(heuristic,init)
        {
        }


        public PancakePuzzle(IHeuristic heuristic, int steps):base(heuristic,null)
        {
            this.numSteps = Generate(steps);
        }

        public PancakePuzzle(NNBaseHeuristic heuristic, double maxUncert, int maxSteps = 1000):base(heuristic,null)
        {
            this.numSteps = Generate(maxUncert, heuristic, maxSteps);
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

            state.Op = new Operation(0);

            for (byte i = 0; i < Global.SIZE; i++)
            {
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
            int nops = state.Arr.Length - 1;

            Operation[] ops = new Operation[nops];

            for (int i=0;i< nops;i++)
            {
                ops[i] = new Operation((byte)(i + 1));
            }

            return ops;
        }
        
        private Edge Child(IState state, Operation op)
        {
            IState child = new State();

            for (byte i = 0; i < Global.SIZE; i++)
            {
                child.Arr[i] = state.Arr[i];
            }
            
            Edge e = Apply(ref child,op);
            e.Child = child;
            return e;
        }


        public override Edge Apply(ref IState state, Operation op)
        {
            Edge e = new Edge(Global.STEPCOST, op, op);
            UndoToken u = new UndoToken();
            u.H = state.H;
            u.Op = op;
            e.UndoToken = u;
            Array.Reverse(state.Arr, 0, op.Vals[0]+1);
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
            Array.Reverse(state.Arr, 0, undo.Op.Vals[0]+1);
        }


    }
}
