using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.NNs
{
    interface INN
    {
        NNFetch Predict(Matrix<float> X, Vector<float> y, params object[] parameters);
        void Train(Matrix<float> X, Vector<float> y, params object[] parameters);
    }
}
