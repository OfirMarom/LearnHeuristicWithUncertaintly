using HeuristicSearch.Heuristics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HeuristicSearch.SlidingPuzzle.Representations
{
    class TwoDim : IRepresentation
    {
        int numInputs;
        int dim;
        Func<double, double> responseFunc;
        Func<double, double> responseFuncInv;

        public int NumInputs
        {
            get { return numInputs; }
        }

        public Func<double, double> ResponseFunc
        {
            get { return responseFunc; }
        }

        public Func<double, double> ResponseFuncInv
        {
            get { return responseFuncInv; }
        }

        public IMultHeuristic MultHeuristic
        {
            get { return null; }
        }
        public TwoDim(Func<double, double> responseFunc, Func<double, double> responseFuncInv)
        {
            if (Global.DOMAINTYPE != typeof(SlidingPuzzle))
            {
                throw new Exception();
            }

            this.dim = SlidingPuzzle.DIM;
            this.numInputs = dim * dim * (dim + dim);
            this.responseFunc = responseFunc;
            this.responseFuncInv = responseFuncInv;
        }

        public Tuple<Matrix<float>, Vector<float>> BuildData(List<TrainingInstance> trainingInstances)
        {
            IState[] states = trainingInstances.Select(x => x.State).ToArray();

            double[] pathCosts = trainingInstances.Select(x => x.Response).ToArray();

            Matrix<float> trainingData = DenseMatrix.Create(states.Length, numInputs, 0);

            for (int r = 0; r < trainingData.RowCount; r++)
            {
                IState state = states[r];

                byte[] stateArr = state.Arr;

                int count = 0;

                for (byte i = 0; i < stateArr.Length; i++)
                {
                    int index = Array.IndexOf(stateArr, i);

                    var coord = SlidingPuzzle.IndexToCoord(index);

                    trainingData[r, count + coord.Item1 - 1] = 1;

                    trainingData[r, count + dim + coord.Item2 - 1] = 1;

                    count += dim + dim;
                }
            }

            Vector<float> response = DenseVector.Create(pathCosts.Length, 0);

            for (int i = 0; i < pathCosts.Length; i++)
            {
                double y = pathCosts[i];

                if (responseFunc != null)
                {
                    y = responseFunc(y);
                }

                response[i] = (float)y;
            }

            return new Tuple<Matrix<float>, Vector<float>>(trainingData, response);

        }


    }
}
