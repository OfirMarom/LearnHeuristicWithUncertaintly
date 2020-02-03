using HeuristicSearch.Heuristics;
using HeuristicSearch.NNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.Heuristics
{
    class NNPreTrainedHeuristic : NNBaseHeuristic, IHeuristic
    {
        MyNN nnSolve;
        double? confLevel;
        bool l2Loss;
       
        public NNPreTrainedHeuristic(MyNN nnSolve, IRepresentation representationSolve, double? confLevel, bool l2Loss)
        {
            this.nnSolve = nnSolve;
            this.representationSolve = representationSolve;
            this.confLevel = confLevel;
            this.l2Loss=l2Loss;
        }

        public override int H(IState state, bool verbose)
        {
            int h;

            bool cached = hCache.TryGetValue(state.GetHashCodeLong(), out h);

            if (cached == false)
            {
                List<TrainingInstance> trainingInstances = new List<TrainingInstance>() { new TrainingInstance(state, 0) };

                var data = representationSolve.BuildData(trainingInstances);

                NNFetch nnFetch = nnSolve.Predict(data.Item1);

                double pred = nnFetch.Preds[0][0];

                int hUnadj = 0;

                if (verbose==true)
                {
                    hUnadj = (int)Math.Round(representationSolve.ResponseFuncInv(pred), 0); ;
                }

                if (confLevel == null)
                {
                    pred = representationSolve.ResponseFuncInv(pred);
                }
                else
                {
                    if (l2Loss == true)
                    {
                        pred = MathNet.Numerics.Distributions.Normal.InvCDF(nnFetch.Preds[0][0], nnFetch.Sigmas[0][0], (double)confLevel);
                    }
                    else
                    {
                        pred = Global.InvLaplace(nnFetch.Preds[0][0], nnFetch.Sigmas[0][0], (double)confLevel);
                    }

                    pred = representationSolve.ResponseFuncInv(pred);
                }


                

                h = (int)Math.Round(pred, 0);

                if (verbose == true)
                {
                    Console.WriteLine("h: " + h + " hUnadj: " + hUnadj);
                }

                if (representationSolve.MultHeuristic == null)
                {
                    if (h < 0)
                    {
                        h = 0;
                    }
                }
                else
                {
                    h = Math.Max(h, representationSolve.MultHeuristic.HArr.Max());
                }

                if (hCache.Count>Global.HCAHCEMAXRECORDS)
                {
                    ClearCache();
                }

                hCache[state.GetHashCodeLong()] = h;

              
            }

            


            return h;
        }

    }
}
