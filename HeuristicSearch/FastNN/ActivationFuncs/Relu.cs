using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN.ActivationFuncs
{
    [Serializable]
    class Relu:IActivationFunc
    {
        public float F(float x)
        {
            return Math.Max(x, 0);
        }

        public float dF(float x)
        {
            if (x > 0)
            {
                return 1;
            }

            return 0;
        }
    }
}
