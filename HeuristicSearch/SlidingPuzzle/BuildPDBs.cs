using HeuristicSearch.SlidingPuzzle.Heuristics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.SlidingPuzzle
{
    static class BuildPDBs
    {

        public static void BuildPDBs24()
        {
            PDBHeuristic pdbHeuristic;

            pdbHeuristic = new PDBHeuristic(new int[] { 1, 2, 5, 6, 7 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 3, 4, 8, 9, 14 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 10, 15, 16, 20, 21 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 13, 18, 19, 23, 24 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 11, 12, 17, 22 });

            pdbHeuristic.Save();



            pdbHeuristic = new PDBHeuristic(new int[] { 1, 2, 3, 7, 8 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 5, 6, 10, 11, 12 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 15, 16, 17, 20, 21 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 18, 19, 22, 23, 24 });

            pdbHeuristic.Save();

            pdbHeuristic = new PDBHeuristic(new int[] { 4, 9, 13, 14 });

            pdbHeuristic.Save();

        }
    }
}
