using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN
{
    [Serializable]
    class DropoutLayer
    {

        Matrix<float> vals;
        float prob;
        int[] samples;

        public DropoutLayer(int miniBatchSize, int numNeurons, float prob)
        {
            vals = DenseMatrix.Create(miniBatchSize, numNeurons, 0);
            this.prob = prob;
            samples = new int[miniBatchSize * numNeurons];
        }

        public void Sample()
        {
            Bernoulli bernouli = new Bernoulli(1 - prob, Global.Random);

            bernouli.Samples(samples);

            int count = 0;

            for (int r = 0; r < vals.RowCount; r++)
            {
                for (int c = 0; c < vals.ColumnCount; c++)
                {
                    vals[r, c] = samples[count] / (1-prob);
                    count++;
                }
            }      
        }

 

        public Matrix<float> Vals
        {
            get { return vals; }
            set { vals = value; }
        }

        public float Prob
        {
            get { return prob; }
        }
    }
}
