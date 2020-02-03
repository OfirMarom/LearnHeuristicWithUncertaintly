using HeuristicSearch.Heuristics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    interface IRepresentation
    {
        int NumInputs { get; }
        Func<double,double> ResponseFunc { get; }
        Func<double, double> ResponseFuncInv { get; }
        IMultHeuristic MultHeuristic { get; }
        Tuple<Matrix<float>, Vector<float>> BuildData(List<TrainingInstance> trainingInstances);
    }
}
