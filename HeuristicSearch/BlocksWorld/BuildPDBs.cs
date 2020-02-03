using HeuristicSearch.BlocksWorld.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.BlocksWorld
{
    class BuildPDBs
    {


        public static void BuildPDBs15()
        {

            PDBHeuristic pdbHeuristic;

            
            pdbHeuristic = new PDBHeuristic(new int[] { 1, 2, 3,4});

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 5,6,7,8 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 9,10,11,12 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 12,13,14,15});

            pdbHeuristic.Save();



            pdbHeuristic = new PDBHeuristic(new int[] { 3,4,5,6 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 7,8,9,10});

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 11,12,13,14 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 1,2,14,15 });

            pdbHeuristic.Save();
            

            
            pdbHeuristic = new PDBHeuristic(new int[] { 2,3,4,5 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 6,7,8,9});

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 10,11,12,13 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 1,13,14,15});

            pdbHeuristic.Save();
            

        }


     


    }
}
