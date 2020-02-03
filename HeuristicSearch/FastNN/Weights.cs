using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra;
using HeuristicSearch.FastNN.Initialization;

namespace HeuristicSearch.FastNN
{
    [Serializable]
    class Weights
    {
        Matrix<float> vals;

        public Weights(int numNeurons, int numNeuronsNext, IInitialization initialization)
        {
            vals = DenseMatrix.Create(numNeurons, numNeuronsNext, new Func<int,int,float>((r,c)=>initialization.Initialize(numNeurons,numNeuronsNext)));
        }

        public Weights (Weights weights)
        {
            this.vals = DenseMatrix.Create(weights.vals.RowCount, weights.vals.ColumnCount, new Func<int, int, float>((r, c) => weights.vals[r, c]));
        }

        public Matrix<float> Vals
        {
            get { return vals; }
            set { vals = value; }
        }

    }
}
