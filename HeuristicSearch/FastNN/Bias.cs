using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra;

namespace HeuristicSearch.FastNN
{
    [Serializable]
    class Bias
    {
        Matrix<float> vals;

        public Bias(int numNeuronsNext)
        {
            vals = DenseMatrix.Create(1,numNeuronsNext, 0);
        }

        public Bias(Bias bias)
        {
            this.vals = DenseMatrix.Create(bias.vals.RowCount, bias.vals.ColumnCount, new Func<int, int, float>((r, c) => bias.vals[r, c]));
        }

        public Matrix<float> Vals
        {
            get { return vals; }
            set { vals = value; }
        }

        public Matrix<float> Broadcast(int n)
        {
            return DenseMatrix.Create(n, vals.ColumnCount,new Func<int, int, float>((r, c) => vals[0,c]));
        }
    }
}
