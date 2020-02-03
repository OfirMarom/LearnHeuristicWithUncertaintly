using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TensorFlow;

namespace HeuristicSearch.TFComponents
{
    class LinearLayer
    {
        public TFOutput W { get; set; }

        public TFOutput b { get; set; }

        public TFOutput InitW { get; set; }

        public TFOutput InitB { get; set; }

        public void Init(TFSession.Runner runner)
        {
            runner.AddTarget(InitW.Operation, InitB.Operation);

        }

        public TFOutput Getz(TFGraph graph, TFOutput act)
        {
          return  graph.Add(graph.MatMul(act,W), b);
        }

        public LinearLayer(TFGraph graph,int inSize, int outSize)
        {
            var wShape = new TFShape(inSize, outSize);

            W = graph.VariableV2(wShape, TFDataType.Float);

            TFOutput initFactor = graph.Sqrt(graph.Div(graph.Const(2F), graph.Const((float)inSize)));

            TFOutput initialW = graph.Mul(graph.Cast( graph.RandomNormal(wShape, 0, 1),TFDataType.Float), initFactor);
            
            InitW = graph.Assign(W, initialW);

            var bShape = new TFShape(outSize);

            b = graph.VariableV2(bShape, TFDataType.Float);

            TFOutput initialB = graph.Zeros(bShape, TFDataType.Float);

            InitB = graph.Assign(b, initialB);

        }
    }
}
