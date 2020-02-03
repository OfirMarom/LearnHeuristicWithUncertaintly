using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN.Initialization
{
    interface IInitialization
    {
        float Initialize(int numNeurons, int numNeuronsNext);
    }
}
