using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HeuristicSearch.NNs
{
    [Serializable]
    class MyNN : INN
    {
        FastNN.FastNN nn;

        public MyNN(int[] numNeurons, int miniBatchSize, bool l2loss, float dropoutProb   = 0)
        {
            nn = new FastNN.FastNN(numNeurons,miniBatchSize,l2loss, dropoutProb);
        }

        public MyNN(FastNN.FastNN nn)
        {
            this.nn = nn;
        }

        public void Save(string path, bool copyOnlyParams=false)
        {
            FastNN.FastNN nn = this.nn;

            if (copyOnlyParams==true)
            {
                nn = this.nn.CopyOnlyParams();
            }

            Global.SerializeObject<MyNN>(new MyNN(nn), path);
        }

        public static MyNN Load(string path)
        {
            return Global.DeserializeObject<MyNN>(path);
        }

        public NNFetch Predict(Matrix<float> X, Vector<float> y = null, params object[] parameters)
        {
           
            float[][] preds = nn.Forward(X,true);

            NNFetch nnFetch = new NNFetch(X.RowCount);

            nnFetch.AddSampleToPreds(preds[0].Select(x => (double)x).ToArray());

            if (nn.ComputeSDOutput == true)
            {
                nnFetch.AddSampleToSigmas(preds[1].Select(x => Global.ToSigma(Global.TrimPhi(x))).ToArray());
            }

            nnFetch.SetCost(0);

            return nnFetch;

        }

        public void Train(Matrix<float> X, Vector<float> y, params object[] parameters)
        {
            nn.Learn(X, y);
        }


      
    }
}
