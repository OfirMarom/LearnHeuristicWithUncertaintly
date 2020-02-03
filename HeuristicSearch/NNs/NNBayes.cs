using HeuristicSearch.TFComponents;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TensorFlow;

namespace HeuristicSearch.NNs
{
    class NNBayes : IDisposable, INN
    {
        TFSession _session;

        TFGraph _graph;

        TFOutput _input;

        TFOutput _output;

        BayesLayer[] _layers;

        AdamOptimizer optimizer;

        TFOutput _cost;

        TFOutput _decay;

        TFOutput[] _preds;

        int numSamples = 5;

        public NNBayes(int[] numNeurons, params object[] parameters)
        {
            float l2Penalty = 0.1F;

            _preds = new TFOutput[numSamples];

            _session = new TFSession();

            _graph = _session.Graph;

            _graph.Seed = Global.Random.Next();

            _layers = new BayesLayer[numNeurons.Length - 1];

            _input = _graph.Placeholder(TFDataType.Float, new TFShape(-1, numNeurons[0]));

            _output = _graph.Placeholder(TFDataType.Float, new TFShape(-1));

            _decay = _graph.Placeholder(TFDataType.Float, new TFShape(1));

            for (int i = 0; i < numNeurons.Length - 1; i++)
            {
                _layers[i] = new BayesLayer(_graph, numNeurons[i], numNeurons[i + 1]);
            }


            TFOutput likelihood = _graph.Const(0F);


            for (int i = 0; i < numSamples; i++)
            {
                TFOutput act = _input;

                foreach (BayesLayer layer in _layers)
                {
                    // TFOutput W = layer.SampleW(_graph);
                    // TFOutput b = layer.Sampleb(_graph);
                    // TFOutput z = _graph.Add(_graph.MatMul(act, W), b);

                    TFOutput z = layer.Samplez(_graph, act);

                    if (layer == _layers.Last())
                    {

                        TFOutput pred = _graph.Reshape(z, _graph.Const(new TFShape(-1)));
                        _preds[i] = pred;
                        TFOutput sample_likelihood = NNOperations.LogOfNormal(_graph, pred, _output, _graph.Const(1F));
                        likelihood = _graph.Add(likelihood, sample_likelihood);
                    }
                    else
                    {
                        act = _graph.Relu(z);

                    }
                }
            }


            TFOutput kl_W = _graph.Const(0F);
            TFOutput kl_b = _graph.Const(0F);

            foreach (BayesLayer layer in _layers)
            {
                kl_W = _graph.Add(kl_W, _graph.ReduceSum(NNOperations.KLUnivariateNormal(_graph, layer.Mu_W, NNOperations.LogTrans(_graph, layer.Phi_W), _graph.Const(0F), _graph.Const((float)(1 / (Math.Sqrt(l2Penalty)))))));
                kl_b = _graph.Add(kl_b, _graph.ReduceSum(NNOperations.KLUnivariateNormal(_graph, layer.Mu_b, NNOperations.LogTrans(_graph, layer.Phi_b), _graph.Const(0F), _graph.Const((float)(1 / Math.Sqrt(l2Penalty))))));

            }

            TFOutput kl = _graph.Add(kl_W, kl_b);
            likelihood = _graph.Div(likelihood, _graph.Const((float)numSamples));

            _cost = _graph.ReduceMean(_graph.Sub(_graph.Mul(_decay, kl), likelihood));

            optimizer = new AdamOptimizer();

            optimizer.AddBayesLayer(_graph, _layers, _cost);

            optimizer.Apply(_graph);

            var runner = _session.GetRunner();

            foreach (BayesLayer layer in _layers)
            {
                layer.Init(runner);
            }

            optimizer.Init(runner);

            runner.Run();
        }



        public NNFetch Predict(Matrix<float> X, Vector<float> y = null, params object[] parameters)
        {
            if (X.RowCount > Global.TFMAXBATCHSIZE)
            {
                throw new Exception();
            }

            int minSample = 1;

            if (parameters != null && parameters.Length != 0)
            {
                minSample = (int)parameters[0];
            }

            int iter = minSample % numSamples == 0 ? minSample / numSamples : minSample / numSamples + 1;


            NNFetch nnFetch = new NNFetch(X.RowCount);

            List<float> costs = new List<float>();

            for (int i = 0; i < iter; i++)
            {
                var runner = _session.GetRunner();

                runner.AddInput(_input, X.ToArray());

                runner.AddInput(_decay, 0F);

                if (y != null)
                {
                    runner.AddInput(_output, y.ToArray());
                }

                foreach (TFOutput pred in _preds)
                {
                    runner.Fetch(pred);
                }

                if (y != null)
                {
                    runner.Fetch(_cost);
                }


                var result = runner.Run();


                if (y != null)
                {
                    costs.Add((float)result.Last().GetValue());
                }

                for (int j = 0; j < _preds.Length; j++)
                {
                    nnFetch.AddSampleToPreds(((float[])result[j].GetValue()).Select(x => (double)x).ToArray());
                }

            }

            if (y != null)
            {
                nnFetch.SetCost(costs.Average());
            }


            return nnFetch;
        }

        public void Train(Matrix<float> X, Vector<float> y, params object[] paramaters)
        {
            if (X.RowCount > Global.TFMAXBATCHSIZE)
            {
                throw new Exception();
            }

            float decay = (float)paramaters[0];

            var runner = _session.GetRunner();

            runner.AddInput(_input, X.ToArray());

            runner.AddInput(_output, y.ToArray());

            runner.AddInput(_decay, decay);

            runner.AddTarget(optimizer.Operations.ToArray());

            runner.Run();

        }


        public void Dispose()
        {
            _graph.Dispose();
            _session.Dispose();
        }
    }

}

