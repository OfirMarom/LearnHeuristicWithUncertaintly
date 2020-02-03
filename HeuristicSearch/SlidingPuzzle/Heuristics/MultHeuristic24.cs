using HeuristicSearch.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.SlidingPuzzle.Heuristics
{
    class MultHeuristic24 : IMultHeuristic
    {
        MDHeuristic mdHeuristic;
        HammingHeuristic hammingHeuristic;
        PDBHeuristic[] pdbHeuristics;
        float[] scale;
        byte[][] pdbs;
        int[] hArr;
        int maxPDBLength;

        public MultHeuristic24()
        {
            mdHeuristic = new MDHeuristic();
            hammingHeuristic = new HammingHeuristic();

            int capacity = 5000000;

            string[] fileNames = {

               "1_2_5_6_7",
               "3_4_8_9_14",
               "10_15_16_20_21",
               "13_18_19_23_24",
               "11_12_17_22",

               "1_2_3_7_8",
               "5_6_10_11_12",
               "15_16_17_20_21",
               "18_19_22_23_24",
               "4_9_13_14"

            };

            pdbHeuristics = new PDBHeuristic[fileNames.Length];

            scale = new float[fileNames.Length + 4];

            hArr = new int[fileNames.Length + 4];

            pdbs = new byte[fileNames.Length][];

            for (int i = 0; i < pdbHeuristics.Length; i++)
            {
                pdbHeuristics[i] = new PDBHeuristic(fileNames[i], capacity);
                scale[i] = pdbHeuristics[i].Values.Max();
                pdbs[i] = new byte[pdbHeuristics[i].PDBArr.Length];
            }

            scale[fileNames.Length] = scale.Take(5).Sum();

            scale[fileNames.Length + 1] = scale.Skip(5).Take(5).Sum();

            scale[fileNames.Length + 2] = mdHeuristic.MDMax;

            scale[fileNames.Length + 3] = Global.SIZE;

            maxPDBLength = Global.MaxLengthJaggedArrays(pdbs);
        }

        public void H(IState state)
        {

            byte[] stateArr = state.Arr;

            for (byte i = 0; i < stateArr.Length; i++)
            {
                byte val = stateArr[i];

                for (int j = 0; j < maxPDBLength; j++)
                {
                    if (j < pdbHeuristics[0].PDBArr.Length && val == pdbHeuristics[0].PDBArr[j])
                    {
                        pdbs[0][j] = i;
                    }

                    if (j < pdbHeuristics[1].PDBArr.Length && val == pdbHeuristics[1].PDBArr[j])
                    {
                        pdbs[1][j] = i;
                    }

                    if (j < pdbHeuristics[2].PDBArr.Length && val == pdbHeuristics[2].PDBArr[j])
                    {
                        pdbs[2][j] = i;
                    }

                    if (j < pdbHeuristics[3].PDBArr.Length && val == pdbHeuristics[3].PDBArr[j])
                    {
                        pdbs[3][j] = i;
                    }

                    if (j < pdbHeuristics[4].PDBArr.Length && val == pdbHeuristics[4].PDBArr[j])
                    {
                        pdbs[4][j] = i;
                    }




                    if (j < pdbHeuristics[5].PDBArr.Length && val == pdbHeuristics[5].PDBArr[j])
                    {
                        pdbs[5][j] = i;
                    }

                    if (j < pdbHeuristics[6].PDBArr.Length && val == pdbHeuristics[6].PDBArr[j])
                    {
                        pdbs[6][j] = i;
                    }

                    if (j < pdbHeuristics[7].PDBArr.Length && val == pdbHeuristics[7].PDBArr[j])
                    {
                        pdbs[7][j] = i;
                    }

                    if (j < pdbHeuristics[8].PDBArr.Length && val == pdbHeuristics[8].PDBArr[j])
                    {
                        pdbs[8][j] = i;
                    }

                    if (j < pdbHeuristics[9].PDBArr.Length && val == pdbHeuristics[9].PDBArr[j])
                    {
                        pdbs[9][j] = i;
                    }
                }
            }

            int h = 0;

            int sum = 0;



            h = pdbHeuristics[0].H(pdbs[0]);
            sum += h;
            hArr[0] = h;


            h = pdbHeuristics[1].H(pdbs[1]);
            sum += h;
            hArr[1] = h;

            h = pdbHeuristics[2].H(pdbs[2]);
            sum += h;
            hArr[2] = h;


            h = pdbHeuristics[3].H(pdbs[3]);
            sum += h;
            hArr[3] = h;


            h = pdbHeuristics[4].H(pdbs[4]);
            sum += h;
            hArr[4] = h;

            hArr[10] = sum;

            sum = 0;

            h = pdbHeuristics[5].H(pdbs[5]);
            sum += h;
            hArr[5] = h;


            h = pdbHeuristics[6].H(pdbs[6]);
            sum += h;
            hArr[6] = h;


            h = pdbHeuristics[7].H(pdbs[7]);
            sum += h;
            hArr[7] = h;


            h = pdbHeuristics[8].H(pdbs[8]);
            sum += h;
            hArr[8] = h;


            h = pdbHeuristics[9].H(pdbs[9]);
            sum += h;
            hArr[9] = h;

            hArr[11] = sum;

            hArr[12] = mdHeuristic.H(state);

            hArr[13] = hammingHeuristic.H(state);

         
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
