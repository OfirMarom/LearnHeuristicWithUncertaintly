using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN.Optimizers
{
    [Serializable]
    class GradientDescent : IOptimizer
    {
        float learningRate;

        public GradientDescent(float learningRate)
        {
            this.learningRate = learningRate;
        }

        public void UpdateParams(DenseLayer[] layers)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                DenseLayer layer = layers[i];
                Matrix<float> W = layer.Weights.Vals;
                Matrix<float> b = layer.Bias.Vals;
                Matrix<float> gW = layer.GradWeights.Vals;
                Matrix<float> gb = layer.GradBias.Vals;
                gW.Multiply(learningRate, gW);
                gb.Multiply(learningRate, gb);
                W.Subtract(gW, W);
                b.Subtract(gb, b);
            }
        }

    
    }
}
