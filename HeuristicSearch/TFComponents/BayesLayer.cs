using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TensorFlow;

namespace HeuristicSearch.TFComponents
{

    class BayesLayer
    {
        public TFOutput Mu_W { get; set; }

        public TFOutput Phi_W { get; set; }

        public TFOutput InitMu_W { get; set; }

        public TFOutput InitPhi_W { get; set; }

        public TFOutput Mu_b { get; set; }

        public TFOutput Phi_b { get; set; }

        public TFOutput InitMu_b { get; set; }

        public TFOutput InitPhi_b { get; set; }


        public TFOutput SampleW(TFGraph graph)
        {
            int seed = Global.Random.Next(0, int.MaxValue);
            TFOutput eps = graph.Cast( graph.RandomNormal(graph.GetTensorShape(Mu_W), 0, 1, seed),TFDataType.Float);
            TFOutput sigma_W =NNOperations.LogTrans(graph, Phi_W);
            return graph.Add(graph.Mul(eps, sigma_W), Mu_W);
        }

        public TFOutput Sampleb(TFGraph graph)
        {
            int seed = Global.Random.Next(0, int.MaxValue);
            TFOutput eps = graph.Cast(graph.RandomNormal(graph.GetTensorShape(Mu_b), 0, 1, seed), TFDataType.Float);
            TFOutput sigma_b = NNOperations.LogTrans(graph, Phi_b);
            return graph.Add(graph.Mul(eps, sigma_b), Mu_b);
        }

        public TFOutput Samplez(TFGraph graph, TFOutput act)
        {
            TFOutput mu = graph.Add(graph.MatMul(act, Mu_W), Mu_b);
            TFOutput sigma = graph.Sqrt(graph.Add(graph.MatMul(graph.Square(act), graph.Square(NNOperations.LogTrans(graph, Phi_W))), graph.Square(NNOperations.LogTrans(graph, Phi_b))));
            int seed = Global.Random.Next(0, int.MaxValue);
            TFOutput eps = graph.Cast(graph.RandomNormal(new TFShape(Global.TFMAXBATCHSIZE, graph.GetShape(mu)[1]), 0, 1, seed), TFDataType.Float);
            eps = graph.Slice(eps, graph.Const(new int[] { 0, 0 }), graph.Shape(mu));
            return graph.Add(graph.Mul(eps, sigma), mu);
            
        }

        public void Init(TFSession.Runner runner)
        {
            runner.AddTarget(InitMu_W.Operation,
                InitPhi_W.Operation,
                InitMu_b.Operation,
                InitPhi_b.Operation);

        }

        public BayesLayer(TFGraph graph, int inSize, int outSize)
        {
            TFShape wShape = new TFShape(inSize, outSize);

            TFShape bShape = new TFShape(outSize);

            TFOutput init = new TFOutput();

            TFOutput initFactor = graph.Sqrt(graph.Div(graph.Const(2F), graph.Const((float)inSize)));

            Mu_W = graph.VariableV2(wShape, TFDataType.Float);

            init = graph.Mul(graph.Cast( graph.RandomNormal(wShape, 0, 1),TFDataType.Float), initFactor);

            InitMu_W = graph.Assign(Mu_W, init);


            Phi_W = graph.VariableV2(wShape, TFDataType.Float);

            init = graph.Mul(graph.Cast( graph.RandomNormal(wShape, -5, 1),TFDataType.Float), initFactor);

            InitPhi_W = graph.Assign(Phi_W, init);


            Mu_b = graph.VariableV2(bShape, TFDataType.Float);

            init = graph.Zeros(bShape,TFDataType.Float);

            InitMu_b = graph.Assign(Mu_b, init);


            Phi_b = graph.VariableV2(bShape, TFDataType.Float);

            init  = graph.Mul(graph.Const(-5F), graph.Ones(bShape,TFDataType.Float));

            InitPhi_b = graph.Assign(Phi_b, init);



        }
    }
}
