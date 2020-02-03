using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TensorFlow;

namespace HeuristicSearch.TFComponents
{


    class AdamOptimizer
    {
        private List<TFOutput> _variables;

        private List<TFOutput> _gradients;

        public List<TFOutput> _m;

        public List<TFOutput> _v;

        public List<TFOperation> Operations { get; set; }

        public List<TFOutput> InitM { get; set; }

        public List<TFOutput> InitV { get; set; }


   
        public void AddBayesLayer(TFGraph graph, BayesLayer[] layers, TFOutput cost)
        {
            TFOutput init = new TFOutput();


            foreach (BayesLayer layer in layers)
            {
                _variables.Add(layer.Mu_W);
                _variables.Add(layer.Phi_W);
                _variables.Add(layer.Mu_b);
                _variables.Add(layer.Phi_b);

              
                _gradients.Add(graph.AddGradients(new TFOutput[] { cost }, new TFOutput[] { layer.Mu_W })[0]);
                _gradients.Add(graph.AddGradients(new TFOutput[] { cost }, new TFOutput[] { layer.Phi_W })[0]);
                _gradients.Add(graph.AddGradients(new TFOutput[] { cost }, new TFOutput[] { layer.Mu_b })[0]);
                _gradients.Add(graph.AddGradients(new TFOutput[] { cost }, new TFOutput[] { layer.Phi_b })[0]);


                init = graph.Zeros(graph.GetTensorShape(layer.Mu_W), TFDataType.Float);
                _m.Add(graph.VariableV2(graph.GetTensorShape(layer.Mu_W), TFDataType.Float));
                InitM.Add(graph.Assign(_m.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.Phi_W), TFDataType.Float);
                _m.Add(graph.VariableV2(graph.GetTensorShape(layer.Phi_W), TFDataType.Float));
                InitM.Add(graph.Assign(_m.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.Mu_b), TFDataType.Float);
                _m.Add(graph.VariableV2(graph.GetTensorShape(layer.Mu_b), TFDataType.Float));
                InitM.Add(graph.Assign(_m.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.Phi_b), TFDataType.Float);
                _m.Add(graph.VariableV2(graph.GetTensorShape(layer.Phi_b), TFDataType.Float));
                InitM.Add(graph.Assign(_m.Last(), init));


                init = graph.Zeros(graph.GetTensorShape(layer.Mu_W), TFDataType.Float);
                _v.Add(graph.VariableV2(graph.GetTensorShape(layer.Mu_W), TFDataType.Float));
                InitV.Add(graph.Assign(_v.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.Phi_W), TFDataType.Float);
                _v.Add(graph.VariableV2(graph.GetTensorShape(layer.Phi_W), TFDataType.Float));
                InitV.Add(graph.Assign(_v.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.Mu_b), TFDataType.Float);
                _v.Add(graph.VariableV2(graph.GetTensorShape(layer.Mu_b), TFDataType.Float));
                InitV.Add(graph.Assign(_v.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.Phi_b), TFDataType.Float);
                _v.Add(graph.VariableV2(graph.GetTensorShape(layer.Phi_b), TFDataType.Float));
                InitV.Add(graph.Assign(_v.Last(), init));

          

            }

        }


        public void AddConvLayers(TFGraph graph, ConvLayer[] layers, TFOutput cost)
        {
            TFOutput init = new TFOutput();

            foreach (ConvLayer layer in layers)
            {
                _variables.Add(layer.W);
                _variables.Add(layer.b);

                _gradients.Add(graph.AddGradients(new TFOutput[] { cost }, new TFOutput[] { layer.W })[0]);
                _gradients.Add(graph.AddGradients(new TFOutput[] { cost }, new TFOutput[] { layer.b })[0]);



                init = graph.Zeros(graph.GetTensorShape(layer.W), TFDataType.Float);
                _m.Add(graph.VariableV2(graph.GetTensorShape(layer.W), TFDataType.Float));
                InitM.Add(graph.Assign(_m.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.b), TFDataType.Float);
               _m.Add(graph.VariableV2(graph.GetTensorShape(layer.b), TFDataType.Float));
               InitM.Add(graph.Assign(_m.Last(), init));


                init = graph.Zeros(graph.GetTensorShape(layer.W), TFDataType.Float);
                _v.Add(graph.VariableV2(graph.GetTensorShape(layer.W), TFDataType.Float));
                InitV.Add(graph.Assign(_v.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.b), TFDataType.Float);
               _v.Add(graph.VariableV2(graph.GetTensorShape(layer.b), TFDataType.Float));
               InitV.Add(graph.Assign(_v.Last(), init));
            }
        }


        public void AddLinearLayers(TFGraph graph, LinearLayer[] layers, TFOutput cost)
        {
            TFOutput init = new TFOutput();

            foreach (LinearLayer layer in layers)
            {
                _variables.Add(layer.W);
                _variables.Add(layer.b);

                _gradients.Add(graph.AddGradients(new TFOutput[] { cost }, new TFOutput[] { layer.W })[0]);
                _gradients.Add(graph.AddGradients(new TFOutput[] { cost }, new TFOutput[] { layer.b })[0]);

               

                init = graph.Zeros(graph.GetTensorShape(layer.W), TFDataType.Float);
                _m.Add(graph.VariableV2(graph.GetTensorShape(layer.W), TFDataType.Float));
                InitM.Add(graph.Assign(_m.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.b), TFDataType.Float);
                _m.Add(graph.VariableV2(graph.GetTensorShape(layer.b), TFDataType.Float));
                InitM.Add(graph.Assign(_m.Last(), init));


                init = graph.Zeros(graph.GetTensorShape(layer.W), TFDataType.Float);
                _v.Add(graph.VariableV2(graph.GetTensorShape(layer.W), TFDataType.Float));
                InitV.Add(graph.Assign(_v.Last(), init));

                init = graph.Zeros(graph.GetTensorShape(layer.b), TFDataType.Float);
                _v.Add(graph.VariableV2(graph.GetTensorShape(layer.b), TFDataType.Float));
                InitV.Add(graph.Assign(_v.Last(), init));
            }
        }

        public AdamOptimizer()
        {
            _variables = new List<TFOutput>();
            _gradients = new List<TFOutput>();
            _m = new List<TFOutput>();
            InitM = new List<TFOutput>();
            _v = new List<TFOutput>();
            InitV = new List<TFOutput>();
            Operations = new List<TFOperation>();

        }


        public void Init(TFSession.Runner runner)
        {
            foreach (TFOutput initM in InitM)
            {
                runner.AddTarget(initM.Operation);

            }

            foreach (TFOutput initV in InitV)
            {
                runner.AddTarget(initV.Operation);

            }

        }


        public void Apply(TFGraph graph, float learningRate = 0.01F, float beta1 = 0.9F, float beta2 = 0.999F, float eps = 1e-8F)
        {
            TFOutput tfLearningRate = graph.Const(learningRate);

            TFOutput tfBeta1 = graph.Const(beta1);

            TFOutput tfBeta2 = graph.Const(beta2);

            TFOutput tfEps = graph.Const(eps);

            TFOutput tfBeta1Power = graph.Const(beta1);

            TFOutput tfBeta2Power = graph.Const(beta2);


            for (int i = 0; i < _variables.Count; i++)
            {
                Operations.Add(graph.ApplyAdam(_variables[i], _m[i], _v[i], tfBeta1Power, tfBeta2Power, tfLearningRate, tfBeta1, tfBeta2, tfEps, _gradients[i],null,true).Operation);
            }
        }

    }
}
