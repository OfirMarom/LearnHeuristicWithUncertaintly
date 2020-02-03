using HeuristicSearch.Heuristics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.PancakePuzzle.Representations
{
    class Features : IRepresentation
    {
        int numInputs;
        IMultHeuristic multHeuristic;
        Func<double, double> responseFunc;
        Func<double, double> responseFuncInv;
        int middleTokenIndex;

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
            get { return multHeuristic; }
        }

        public Features(IMultHeuristic multHeuristic, Func<double, double> responseFunc, Func<double, double> responseFuncInv)
        {
            if (Global.DOMAINTYPE != typeof(PancakePuzzle))
            {
                throw new Exception();
            }

            this.numInputs = multHeuristic.HArr.Length+2;
            this.multHeuristic = multHeuristic;
            this.responseFunc = responseFunc;
            this.responseFuncInv = responseFuncInv;
            this.middleTokenIndex = Global.SIZE / 2;

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

                multHeuristic.H(state);

                for (int i = 0; i < multHeuristic.HArr.Length; i++)
                {
                    trainingData[r, i] = multHeuristic.HArr[i] / multHeuristic.Scale[i];
                }

                float maxOutOfPlace = 0;

                for (int i = 0; i < Global.SIZE; i++)
                {
                    byte val = stateArr[i];

                    int diff = (val > i ? val - i : i - val);

                    if (diff > 0 && val > maxOutOfPlace)
                    {
                        maxOutOfPlace = val;
                    }                 
                }

                trainingData[r, multHeuristic.HArr.Length] = stateArr[middleTokenIndex] == middleTokenIndex ? 1 : 0;

                trainingData[r, multHeuristic.HArr.Length + 1] = maxOutOfPlace / Global.SIZE;
                
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
