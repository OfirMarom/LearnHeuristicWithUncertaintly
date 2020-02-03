using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN.Optimizers
{
    interface IOptimizer
    {
        void UpdateParams(DenseLayer[] layers);
       
    }
}
