using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    struct Operation
    {
        byte[] vals;

        public Operation(params byte[] vals)
        {
            this.vals = vals;
        }

        
        public byte[] Vals
        {
            get { return vals; }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            Operation op = (Operation)obj;

            if(op.vals==null || this.vals==null)
            {
                return false;
            }

            if(op.vals.Length != this.vals.Length)
            {
                return false;
            }

            for(int i=0;i<op.vals.Length;i++)
            {
                if(op.vals[i] != vals[i])
                {
                    return false;
                }
            }

            return true;
        }
    }

    class UndoToken : IUndoToken
    {
        int h;
        Operation op;

        public int H
        {
            get { return h; }
            set { h = value; }
        }

        public Operation Op
        {
            get { return op; }
            set { op = value; }
        }
    }

    class State : IState
    {
        byte[] arr = new byte[Global.SIZE];
        Operation op;
        int h;

        public byte[] Arr
        {
            get { return arr; }
            set { arr = value; }
        }

        public Operation Op
        {
            get { return op; }
            set { op = value; }
        }

        public int H
        {
            get { return h; }
            set { h = value; }
        }

        public override int GetHashCode()
        {
            int h = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                h *= Global.SIZE;
                h += arr[i];
            }

            return h;


        }

        public long GetHashCodeLong()
        {
            long h = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                h = h * Global.SIZE;
                h += arr[i];
            }

            return h;

        }

        public override bool Equals(object obj)
        {
            State state = (State)obj;

            if (!state.op.Equals( this.op))
            {
                return false;
            }

            for (int i = 0; i < arr.Length; i++)
            {
                if (this.arr[i] != state.arr[i])
                {
                    return false;
                }
            }

            return true;
        }

        public IState Clone()
        {
            State state = new State();
            state.op = this.op;
            state.h = this.h;
            for (byte i = 0; i < this.arr.Length; i++)
            {
                state.arr[i] = this.arr[i];
            }

            return state;
        }
    }

    struct Edge
    {
        Operation op;
        Operation pop;
        int cost;
        IState child;
        IUndoToken undoToken;

        public Edge(int cost, Operation op, Operation pop)
        {
            this.op = op;
            this.cost = cost;
            this.pop = pop;
            child = null;
            undoToken = null;
        }

        public Operation OP
        {
            get { return op; }
        }

        public Operation POP
        {
            get { return pop; }
        }

        public int Cost
        {
            get { return cost; }
        }

        public IState Child
        {
            get { return child; }
            set { child = value; }
        }

        public IUndoToken UndoToken
        {
            get { return undoToken; }
            set { undoToken = value; }
        }
    }

    abstract class SearchAlg<D> where D : ISearchDomain
    {
        protected long expanded;
        protected long generated;
        protected D d;

        public SearchAlg(D d)
        {
            this.d = d;
            this.expanded = 0;
            this.generated = 0;
        }

        public virtual List<IState> Search(IState startState, int? tMax = null)
        {
            return null;
        }

    }
}
