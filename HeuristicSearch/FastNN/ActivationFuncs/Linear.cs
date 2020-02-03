using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN.ActivationFuncs
{
    [Serializable]
    class Linear : IActivationFunc
    {
        public float F(float x)
        {
           return x;
        }

        public float dF(float x)
        {
            return 1;
        }
    }
}
