using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.BlocksWorld.Heuristics
{
    class MultHeuristic15: IMultHeuristic
    {
        HammingHeuristic hammingHeuristic;

        PDBHeuristic[] pdbHeuristics;

        int[] hArr;
        float[] scale;



        public MultHeuristic15()
        {
            string[] fileNames = {

                "1_2_3_4",
                "5_6_7_8",
                "9_10_11_12",
                "12_13_14_15",
   

                "3_4_5_6",
                "7_8_9_10",
                "11_12_13_14",
                "1_2_14_15",

                "2_3_4_5",
                "6_7_8_9",
                "10_11_12_13",
                "1_13_14_15"


            };

            pdbHeuristics = new PDBHeuristic[fileNames.Length];

            hammingHeuristic = new HammingHeuristic();

            scale = new float[fileNames.Length + 1];

            hArr = new int[fileNames.Length + 1];


            for (int i = 0; i < pdbHeuristics.Length; i++)
            {
                pdbHeuristics[i] = new PDBHeuristic(fileNames[i], null);
                scale[i] = pdbHeuristics[i].Values.Max();
            }

            scale[fileNames.Length] = Global.SIZE;

        }

        public void H(IState state)
        {
          
            List<List<byte>> stacks = PDBHeuristic.ToStacks(state.Arr);

 
            hArr[0] = pdbHeuristics[0].H(stacks);
            hArr[1] = pdbHeuristics[1].H(stacks);
            hArr[2] = pdbHeuristics[2].H(stacks);
            hArr[3] = pdbHeuristics[3].H(stacks);

            hArr[4] = pdbHeuristics[4].H(stacks);
            hArr[5] = pdbHeuristics[5].H(stacks);
            hArr[6] = pdbHeuristics[6].H(stacks);
            hArr[7] = pdbHeuristics[7].H(stacks);

            hArr[8] = pdbHeuristics[8].H(stacks);
            hArr[9] = pdbHeuristics[9].H(stacks);
            hArr[10] = pdbHeuristics[10].H(stacks);
            hArr[11] = pdbHeuristics[11].H(stacks);

            hArr[12] = hammingHeuristic.H(state);


        }

        public int[] HArr
        {
            get { return hArr; }
        }

        public float[] Scale
        {
            get { return scale; }
        }
    }
}
