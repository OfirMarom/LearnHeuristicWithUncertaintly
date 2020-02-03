using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicSearch.Heursitics;

namespace HeuristicSearch.PancakePuzzle.Heuristics
{
    class PDBLocHeuristic : PDBBaseHeuristic
    {
        byte[] stateArr = new byte[Global.SIZE];

        public PDBLocHeuristic(int[] pdbArr) : base(pdbArr)
        {
        }

        public PDBLocHeuristic(string fileName, int? capacity) : base(fileName,capacity)
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
            for (int i = 0; i < stateArr.Length; i++)
            {
                stateArr[i] = Global.SIZE;
            }

            for (int i = 0; i < n.pdb.size; i++)
            {
                stateArr[n.pdb.Get(i)] = pdbArr[i];
            }

            Array.Reverse(stateArr, 0, op + 1);

            byte c = 0;

            for (int i = 0; i < pdbArr.Length; i++)
            {
                for (int j = 0; j < stateArr.Length; j++)
                {

                    if (stateArr[j] == pdbArr[i])
                    {
                        n.pdb.Set(i, (byte)j);

                        if (j == 0)
                        {
                            c = Global.STEPCOST;
                        }

                        break;
                    }
                }
            }


            n.h_value += c;
            n.op = op;
        }


        protected override int NOPS(byte op)
        {
            return Global.SIZE - 1;
        }

        protected override byte NTHOP(byte op, int index)
        {
            return  (byte)(index + 1);
        }
    }
}
