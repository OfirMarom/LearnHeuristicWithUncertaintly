using HeuristicSearch.NNs;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.Heuristics
{
    class NNBaseHeuristic : IHeuristic
    {
        protected List<TrainingInstance> memoryBuffer;
        protected int memoryBufferMaxRecords;
        protected bool isTrained;
        protected Dictionary<long, int> hCache;
        protected IRepresentation representationSolve;
        protected IRepresentation representationUncert;
        

        public NNBaseHeuristic()
        {
            memoryBuffer = new List<TrainingInstance>();
            memoryBufferMaxRecords = 25000;
            hCache = new Dictionary<long, int>();
        }

        public void ClearCache()
        {
            hCache.Clear();
        }

        public virtual void Update(List<IState>[] paths)
        {
            return;
        }

        public virtual void Dispose()
        {

        }

        public virtual SummaryStatistics[] GetSummaryStatistics(IState[] states, params object[] parameters)
        {
            return null;
        }


        public virtual int H(IState state, bool verbose=false)
        {
            return 0;
        }

        protected void ManageMemoryBuffer(List<IState>[] plans)
        {
            List<TrainingInstance> trainingInstances = new List<TrainingInstance>();

            foreach (var plan in plans)
            {
                for (int i = 0; i < plan.Count; i++)
                {
                    double pathCost = Global.CostOfPath(plan.GetRange(i, plan.Count - i).ToArray());
                    TrainingInstance trainingInstance = new TrainingInstance(plan[i], pathCost);
                    trainingInstances.Add(trainingInstance);
                }

            }
       
            for (int i = 0; i < trainingInstances.Count; i++)
            {
                TrainingInstance trainingInstance = trainingInstances[i];

                if (!memoryBuffer.Any(x => x.Equals(trainingInstance)))
                {
                    memoryBuffer.Add(trainingInstance);
                }         
            }
            

            if (memoryBuffer.Count > memoryBufferMaxRecords)
            {
                int numIter = memoryBuffer.Count - memoryBufferMaxRecords;
                memoryBuffer.RemoveRange(0, numIter);
            }

        }

        protected List<TrainingInstance> SampleFromMemoryBuffer(int numSample, double[] sampleWeights)
        {
            if (sampleWeights != null)
            {
                sampleWeights = sampleWeights.ToArray();
            }

            double norm = 1;

            List<TrainingInstance> trainingInstances = new List<TrainingInstance>();

            List<int> sampledIndexes = new List<int>();

            for (int i = 0; i < numSample; i++)
            {
                int index;

                if (sampleWeights == null)
                {
                    index = Global.Random.Next(0, memoryBuffer.Count);
                }
                else
                {
                    index = Global.Sample(sampleWeights, norm);

                    if (memoryBuffer.Count > numSample)
                    {
                        norm = 1 - sampleWeights[index];
                        sampleWeights[index] = 0;
                    }


                }

                var trainingInstance = memoryBuffer[index];
                trainingInstances.Add(trainingInstance);
            }

            return trainingInstances;

        }

        public bool IsTrained
        {
            get { return isTrained; }
        }
    }
}
