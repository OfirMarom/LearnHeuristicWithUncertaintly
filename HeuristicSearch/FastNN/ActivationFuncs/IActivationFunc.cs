using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN.ActivationFuncs
{
    interface IActivationFunc
    {
        float F(float z);
        float dF(float a);
    }
}
