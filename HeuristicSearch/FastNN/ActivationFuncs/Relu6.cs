using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN.ActivationFuncs
{
    [Serializable]
    class Relu6 : IActivationFunc
    {
        public float F(float x)
        {
            return Math.Min(Math.Max(x, 0), 6);
        }

        public float dF(float x)
        {
            if (x > 0 && x < 6)
            {
                return 1;
            }

            return 0;
        }
    }
}
