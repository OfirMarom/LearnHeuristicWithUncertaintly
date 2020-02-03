using HeuristicSearch.Heuristics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.BlocksWorld.Representations
{
    class OneDim : IRepresentation
    {
        int numInputs;
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

        public OneDim(Func<double, double> responseFunc, Func<double, double> responseFuncInv)
        {
            if (Global.DOMAINTYPE != typeof(BlocksWorld))
            {
                throw new Exception();
            }

            this.numInputs = Global.SIZE * (Global.SIZE+1);
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

                for (int i = 0; i < stateArr.Length; i++)
                {
                    trainingData[r, count + stateArr[i]] = 1;
                    count += stateArr.Length+1;
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
