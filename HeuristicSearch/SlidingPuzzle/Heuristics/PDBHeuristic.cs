using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using HeuristicSearch.Heursitics;

namespace HeuristicSearch.SlidingPuzzle.Heuristics
{

    
    
    class PDBHeuristic : PDBBaseHeuristic
    {

        static readonly OpTab[] optab;

        static PDBHeuristic()
        {
            optab = SlidingPuzzle.BuildOpTab();
        }

        public PDBHeuristic(int[] pdbArr) :base(pdbArr)
        {
        }

        public PDBHeuristic (string fileName, int? capacity):base(fileName,capacity)
        {

        }
           
        protected override long VisitedHash(Node n)
        {
            long hash = 0;

            for (int i = 0; i < n.pdb.size; i++)
            {
                hash *= Global.SIZE;
                hash += n.pdb.Get(i);
            }


            hash *= Global.SIZE;
            hash += n.op;

            return hash;
        }

        protected override long PDBHash(Node n)
        {

            long hash = 0;

            for (int i = 0; i < n.pdb.size; i++)
            {
                hash *= Global.SIZE;
                hash += n.pdb.Get(i);
            }

            return hash;
        }


        protected override void ApplyOp(ref Node n, byte op)
        {
            for (int i = 0; i < n.pdb.size; i++)
            {
                if (n.pdb.Get(i) == op)
                {
                    n.pdb.Set(i, n.op);
                    n.h_value += Global.STEPCOST;
                    break;
                }
            }

            n.op = op;
        }

        protected override int NOPS(byte op)
        {
            return optab[op].n;   
        }

        protected override byte NTHOP(byte op, int index)
        {
            return optab[op].ops[index];
        }
    }
}
