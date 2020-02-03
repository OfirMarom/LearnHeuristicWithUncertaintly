using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TensorFlow;

namespace HeuristicSearch.TFComponents
{
    class ConvLayer
    {
        public TFOutput W { get; set; }

        public TFOutput b { get; set; }

        public TFOutput InitW { get; set; }

        public TFOutput InitB { get; set; }

        long[] strides;

        string padding;

        public void Init(TFSession.Runner runner)
        {
            runner.AddTarget(InitW.Operation, InitB.Operation);
        }

        public TFOutput Getz(TFGraph graph, TFOutput act)
        {
            TFOutput z = graph.Conv2D(act, W, strides, padding);
            z = graph.BiasAdd(z, b);
            return z;
        }

        public ConvLayer(TFGraph graph, int filter_height, int filter_width, int in_channels, int out_channels, long[] strides, string padding)
        {
            this.strides = strides;

            this.padding = padding;

            var wShape = new TFShape(filter_height,filter_width,in_channels,out_channels);

            W = graph.VariableV2(wShape, TFDataType.Float);

            TFOutput initFactor = graph.Sqrt(graph.Div(graph.Const(2F), graph.Const((float)(filter_height*filter_width*in_channels))));

            TFOutput initialW = graph.Mul(graph.Cast(graph.RandomNormal(wShape, 0, 1), TFDataType.Float), initFactor);

            InitW = graph.Assign(W, initialW);

            var bShape = new TFShape(out_channels);

            b = graph.VariableV2(bShape, TFDataType.Float);

            TFOutput initialB = graph.Zeros(bShape, TFDataType.Float);

            InitB = graph.Assign(b, initialB);

        }
    }
}

