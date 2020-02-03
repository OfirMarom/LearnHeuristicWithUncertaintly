using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace HeuristicSearch.FastNN.Initialization
{
    [Serializable]
    class HeNormal : IInitialization
    {
        public float Initialize(int numNeurons, int numNeuronsNext)
        {
            double multFactor = Math.Sqrt(2D / numNeurons);
            double init = Normal.Sample(Global.Random,0,1) * multFactor;
            return (float)init;
        }
    }
}
