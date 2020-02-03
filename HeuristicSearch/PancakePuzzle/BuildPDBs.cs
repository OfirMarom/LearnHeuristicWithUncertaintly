using HeuristicSearch.PancakePuzzle.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.PancakePuzzle
{
    static class BuildPDBs
    {
     
        public static void BuildPDBs24()
        {
            
            PDBLocHeuristic pdbHeuristic;

            pdbHeuristic = new PDBLocHeuristic(new int[] { 0, 1, 2, 3, 4 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBLocHeuristic(new int[] { 5, 6, 7, 8, 9 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBLocHeuristic(new int[] { 10, 11, 12, 13, 14 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBLocHeuristic(new int[] { 15, 16, 17, 18, 19 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBLocHeuristic(new int[] {  20, 21, 22, 23 });

            pdbHeuristic.Save();




            pdbHeuristic = new PDBLocHeuristic(new int[] { 0,1,2,18});

            pdbHeuristic.Save();

            pdbHeuristic = new PDBLocHeuristic(new int[] { 3,4,5,6,7});

            pdbHeuristic.Save();

            pdbHeuristic = new PDBLocHeuristic(new int[] { 8,9,10,11,12});

            pdbHeuristic.Save();

            pdbHeuristic = new PDBLocHeuristic(new int[] { 13,14,15,16,17});

            pdbHeuristic.Save();

            pdbHeuristic = new PDBLocHeuristic(new int[] { 19,20,21,22,23});

            pdbHeuristic.Save();


        }


    }
}
