using HeuristicSearch.Heuristics;
using HeuristicSearch.NNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.Heuristics
{
    class NNBayesHeuristic : NNBaseHeuristic, IHeuristic
    {

        NNBayes nnUncert;
        MyNN nnSolve;
        float beta;
        double? confLevel;   
        bool updateBeta;
        bool l2Loss;
        float betaDiscountFactor;
        double reduceUncertFactor;
        double q;
        double hq = double.MinValue;



        public double? ConfLevel
        {
            get { return confLevel; }
            set { confLevel = value; }
        }

        public bool UpdateBeta
        {
            get { return updateBeta; }
            set { updateBeta = value; }
        }

        public override SummaryStatistics[] GetSummaryStatistics(IState[] states, params object[] parameters)
        {
         
            List<double>[] preds = new List<double>[states.Length];

            SummaryStatistics[] summaryStatistics = new SummaryStatistics[states.Length];

            List<Tuple<int, int, IState[]>> batches = GetBatches(states, Global.TFMAXBATCHSIZE);

            foreach (var batch in batches)
            {             
                List<TrainingInstance> trainingInstances = new List<TrainingInstance>();

                foreach (IState state in batch.Item3)
                {
                    trainingInstances.Add(new TrainingInstance(state, 0));
                }

                var X = representationUncert.BuildData(trainingInstances).Item1;
   
                List<double>[] batchPreds = nnUncert.Predict(X, null, Global.UNCERTMINSAMPLE).Preds;

                int count = 0;

                for(int i=batch.Item1;i<batch.Item2;i++)
                {
                    preds[i] = batchPreds[count];
                    count++;
                }
            }

            for (int i = 0; i < preds.Length; i++)
            {
                double mean = preds[i].Average();
                double sdModel = Math.Sqrt(Math.Max(0, preds[i].Select(x => Math.Pow(x, 2)).Average() - Math.Pow(mean, 2)));
                summaryStatistics[i] = new SummaryStatistics(mean, sdModel, 0);
            }

            return summaryStatistics;

        }

        public NNBayesHeuristic(IRepresentation representationSolve, IRepresentation representationUncert, int numHiddenSolve, int numOutputSolve, int? numHiddenUncert, float dropout, bool l2Loss) : base()
        {
            this.l2Loss = l2Loss;
            this.beta= 0.05F;
            float minBeta = 0.00001F;
            this.q = 0.95;

            this.representationSolve = representationSolve;
            this.representationUncert= representationUncert;
            nnSolve = new MyNN(new int[] {representationSolve.NumInputs, numHiddenSolve, numOutputSolve }, memoryBufferMaxRecords, l2Loss,dropout);

            nnUncert = null;

            if(numHiddenUncert!=null)
            {
                nnUncert = new NNBayes(new int[] { representationUncert.NumInputs, (int)numHiddenUncert, 1 });
            }

            this.confLevel = null;
            this.updateBeta= true;

            this.betaDiscountFactor = (float) Math.Pow(minBeta / beta, 1F / Global.NITER);
            this.reduceUncertFactor = 0.8;

        }

        public override void Dispose()
        {
            if(nnUncert!=null)
            {
                nnUncert.Dispose();
            }          
        }

        public override int H(IState state,bool verbose=false)
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

                if(verbose==true)
                {
                    hUnadj = (int)Math.Round(representationSolve.ResponseFuncInv(pred), 0);
                }


                if (confLevel == null)
                {
                    pred = representationSolve.ResponseFuncInv(pred);
                }
                else
                {
                    if (pred < hq)
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
                    else
                    {
                        pred = representationSolve.ResponseFuncInv(pred);

                        if (l2Loss == true)
                        {
                            pred = MathNet.Numerics.Distributions.Normal.InvCDF(pred, Global.UNCERTTHRESH, (double)confLevel);
                        }
                        else
                        {
                            pred = Global.InvLaplace(pred, Global.UNCERTTHRESH, (double)confLevel);
                        }
                    }
                }

                h = (int)Math.Round(pred, 0);

                if(representationSolve.MultHeuristic==null)
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

                if(verbose==true)
                {
                    Console.WriteLine("h: " + h + " hUnadj: " + hUnadj);
                }

                if (hCache.Count > Global.HCAHCEMAXRECORDS)
                {
                    ClearCache();
                }

                hCache[state.GetHashCodeLong()] = h;
            }



            return h;

        }

        public MyNN NNSolve
        {
            get { return nnSolve; }
        }


        public double[] GetMemoryBufferErrors()
        {

            int numStates = memoryBuffer.Count;

            double[] errors = new double[memoryBuffer.Count];


            for (int i = 0; i < numStates; i++)
            {

                var X = representationSolve.BuildData(new List<TrainingInstance>() { memoryBuffer[i] }).Item1;

                List<double>[] preds = nnSolve.Predict(X).Preds;

                errors[i] = Math.Abs(representationSolve.ResponseFuncInv(preds[0][0]) - memoryBuffer[i].Response);

            }



            return errors;
        }

        private List<Tuple<int,int,IState[]>> GetBatches(IState[] states,int batchSize)
        {
            int numStates = states.Length;

            if (batchSize >= numStates)
            {
                return new List<Tuple<int, int, IState[]>>() { new Tuple<int, int, IState[]>(0,numStates,states)};
            }

            int rem = numStates % batchSize;

            int iter = rem == 0 ? numStates / batchSize : numStates / batchSize + 1;

            List<Tuple<int, int, IState[]>> batches = new List<Tuple<int, int, IState[]>>();

            for (int i = 0; i < iter; i++)
            {
                int skip = i * batchSize;

                int take = batchSize;

                if (i == iter - 1 && rem != 0)
                {
                    take = rem;
                }

                IState[] batch = states.Skip(skip).Take(take).ToArray();

                batches.Add(new Tuple<int, int, IState[]>(skip, skip + take, batch));
            }

            return batches;
        }

        public double[] GetMemoryBufferUncerts()
        {
            
            double[] uncerts = new double[memoryBuffer.Count];

            List<Tuple<int, int, IState[]>> batches = GetBatches(memoryBuffer.Select(x=>x.State).ToArray(), Global.TFMAXBATCHSIZE);

            foreach (var batch in batches)
            {
                var summaryStatistics = GetSummaryStatistics(batch.Item3);

                int count = 0;
                for (int i = batch.Item1; i < batch.Item2; i++)
                {
                    uncerts[i] = summaryStatistics[count].SDModel;
                    count++;
                }

            }


            return uncerts;
        }

        
        

        private List<TrainingInstance> GetRecords()
        {
            List<TrainingInstance> records = new List<TrainingInstance>(memoryBufferMaxRecords);

            int count = 0;

            for (int i = 0; i < memoryBufferMaxRecords; i++)
            {
                records.Add(memoryBuffer[count]);

                count++;

                if (count == memoryBuffer.Count)
                {
                    count = 0;
                }


            }

            return records;
        }

        public override void Update(List<IState>[] plans)
        {

            ManageMemoryBuffer(plans);

            if(memoryBuffer.Count==0)
            {
                return;
            }

            double maxUncert = double.MaxValue;

            int count = 0;

            double[] sampleWeightsUncert = new double[memoryBuffer.Count];

            var records = GetRecords();

            var dataSolve = representationSolve.BuildData(records);

            for (int i = 0; i < 1000; i++)
            {
                nnSolve.Train(dataSolve.Item1, dataSolve.Item2);
                double[] errors = GetMemoryBufferErrors();
                double maxError = errors.Max();

                if (i > 1 && maxError < 0.5)
                {
                    break;
                }

                double averageError = errors.Average();
                Console.WriteLine("iter: " + i + " max error: " + maxError + " avg error: " + averageError);
            }

            if(nnUncert!=null)
            {
                do
                {
                    if (count > 100)
                    {
                        if (IsTrained == true && updateBeta == true)
                        {
                            beta *= betaDiscountFactor;
                        }

                        break;
                    }


                    double[] uncerts = GetMemoryBufferUncerts();

                    maxUncert = uncerts.Max();

                    for (int i = 0; i < sampleWeightsUncert.Length; i++)
                    {
                        if (uncerts[i] > Global.UNCERTTHRESH * reduceUncertFactor)
                        {
                            sampleWeightsUncert[i] = uncerts[i];
                        }
                        else
                        {
                            sampleWeightsUncert[i] = -1;
                        }
                    }

                    double normUncert = sampleWeightsUncert.Sum(x => Math.Exp(x));

                    sampleWeightsUncert = sampleWeightsUncert.Select(x => Math.Exp(x) / normUncert).ToArray();

                    Console.WriteLine("iter: " + count + " max uncert: " + maxUncert);

                    if (isTrained == true && maxUncert < Global.UNCERTTHRESH * reduceUncertFactor)
                    {
                        break;
                    }

                    for (int i = 0; i < 50; i++)
                    {
                        List<TrainingInstance> sampledRecords = SampleFromMemoryBuffer(Global.TFMAXBATCHSIZE, sampleWeightsUncert);
                        var dataUncert = representationUncert.BuildData(sampledRecords);
                        nnUncert.Train(dataUncert.Item1, dataUncert.Item2, beta);
                    }


                    count++;

                } while (true);
            }

            hq = MathNet.Numerics.Statistics.Statistics.Quantile(memoryBuffer.Select(x => x.Response), q);
            hq = representationSolve.ResponseFunc(hq);

            isTrained = true;
        }

   
    }
}
