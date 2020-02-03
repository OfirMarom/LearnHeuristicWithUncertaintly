using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    interface ISearchDomain
    {
        int H(IState state);
        bool IsGoal(IState state);
        Operation[] Operations(IState state);
        Edge Apply(ref IState state, Operation op);
        void Undo(ref IState state, Edge edge);
        IState Initial();
        IState Goal();
        List<Edge> Expand(IState state);
        int? NumSteps { get; }

    }
}
