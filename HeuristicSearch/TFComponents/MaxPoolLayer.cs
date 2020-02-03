using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TensorFlow;

namespace HeuristicSearch.TFComponents
{
    class MaxPoolLayer
    {
        long[] ksize;

        long[] strides;

        string padding;

        public TFOutput Getz(TFGraph graph, TFOutput act)
        {
            return graph.MaxPool(act, ksize, strides, padding);
        }

        public MaxPoolLayer(TFGraph graph, long[] ksize, long[] strides, string padding)
        {
            this.ksize = ksize;
            this.strides = strides;
            this.padding = padding;
        }
    }
}
