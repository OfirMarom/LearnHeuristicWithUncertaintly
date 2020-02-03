using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TensorFlow;

namespace HeuristicSearch.TFComponents
{
   

    public static class NNOperations
    {

        public static float NUMERICALCONSTANT = 1e-8F;

     
        //https://github.com/migueldeicaza/TensorFlowSharp/issues/82
        public static TFOutput Dropout(TFGraph graph, TFOutput x, TFOutput keep_prob, int? seed = null, bool scale=true)
        {
            TFOutput shapeTensor = graph.Shape(x);

            // uniform [keep_prob, 1.0 + keep_prob)
            TFOutput random_tensor = keep_prob;
            random_tensor = graph.Add(random_tensor, graph.RandomUniform(shapeTensor, seed: seed, dtype: x.OutputType));

            // 0. if [keep_prob, 1.0) and 1. if [1.0, 1.0 + keep_prob)
            TFOutput binary_tensor = graph.Floor(random_tensor);
            TFOutput ret;
            if(scale)
            {
                ret = graph.Mul(graph.Div(x, keep_prob), binary_tensor);

            }
            else
            {
                ret = graph.Mul(x, binary_tensor);

            }
            graph.SetTensorShape(ret, graph.GetShape(x));
            return ret;

        }

        public static TFOutput Relu(TFGraph graph, TFOutput z, float minVal, float maxVal)
        {
           return graph.Maximum(graph.Const(minVal), graph.Minimum(z, graph.Const(maxVal)));
        }

        public static TFOutput LogTrans(TFGraph graph, TFOutput z)
        {
            return graph.Log(graph.Add(graph.Const(1F), graph.Exp(z)));
        }

        public static TFOutput LogOfLaplace(TFGraph graph, TFOutput x, TFOutput mu, TFOutput sigma)
        {
            TFOutput term1 = graph.Mul(graph.Const(-1F), graph.Log(sigma));
            TFOutput term2 = graph.Div(graph.Abs(graph.Sub(x, mu)), graph.Mul(graph.Const(-1F), sigma));

            return graph.Add(term1, term2);
        }

        public static TFOutput LogOfNormal(TFGraph graph, TFOutput x, TFOutput mu, TFOutput sigma)
        {

            TFOutput term1 = graph.Mul(graph.Const(-0.5F), graph.Log(graph.Square(sigma)));
            TFOutput term2 = graph.Div(graph.SquaredDifference(x, mu), graph.Mul(graph.Const(-2F), graph.Square(sigma)));

            return graph.Add(term1, term2);
        }

        public static TFOutput KLUnivariateNormal(TFGraph graph, TFOutput mu1, TFOutput sigma1, TFOutput mu0, TFOutput sigma0)
        {
         
            TFOutput t1 = graph.Div(graph.Add(graph.Square(sigma1), graph.Square(graph.Sub(mu1, mu0))), graph.Mul(graph.Const(2F), graph.Square(sigma0)));
            TFOutput t2 = graph.Log(graph.Div(sigma0, graph.Add( sigma1, graph.Const(NUMERICALCONSTANT))));
            return graph.Sub(graph.Add(t1, t2), graph.Const(0.5F));
        }
    }
}
