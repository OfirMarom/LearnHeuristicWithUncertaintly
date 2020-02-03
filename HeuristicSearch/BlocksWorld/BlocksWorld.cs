using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.BlocksWorld
{
    class BlocksWorld : SearchDomain, ISearchDomain
    {

        public BlocksWorld(IHeuristic heuristic, byte[] init) : base(heuristic, init)
        {
        }


        public BlocksWorld(IHeuristic heuristic, int steps) : base(heuristic, null)
        {
            this.numSteps = Generate(steps);
        }

        public BlocksWorld(NNBaseHeuristic heuristic, double maxUncert, int maxSteps = 1000) : base(heuristic, null)
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
            List<Operation> operations = new List<Operation>();

            byte[] arr = state.Arr;

            Operation op;

            List<byte> moveableBlocks = new List<byte>(Global.SIZE);

            for (int i = 0; i < arr.Length; i++)
            {
                byte block = (byte)(i + 1);

                int blockAboveIndex = Array.FindIndex(arr, x => x == block);

                if (blockAboveIndex == -1)
                {
                    moveableBlocks.Add(block);
                }
            }

            for (int i = 0; i < moveableBlocks.Count; i++)
            {
                byte block = moveableBlocks[i];

                byte blockBelow = arr[block - 1];

                for (int j = 0; j < moveableBlocks.Count; j++)
                {
                    if (i != j)
                    {
                        op = new Operation(block, moveableBlocks[j], blockBelow);
                        operations.Add(op);
                    }
                }

                if (blockBelow != 0)
                {
                    op = new Operation(block, 0, blockBelow);
                    operations.Add(op);
                }

            }

            return operations.ToArray();
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


        public override Edge Apply(ref IState state, Operation op)
        {
            Edge e = new Edge(Global.STEPCOST, op, op);
            UndoToken u = new UndoToken();
            u.H = state.H;
            u.Op = op;
            e.UndoToken = u;
            state.Arr[op.Vals[0] - 1] = op.Vals[1];

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
            Operation op = edge.OP;
            state.Arr[op.Vals[0] - 1] = op.Vals[2];
        }
    }
}
