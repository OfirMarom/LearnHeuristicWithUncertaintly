using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch
{
    public class OpTab
    {
        public OpTab(int maxBranchFactor)
        {
            ops = new byte[maxBranchFactor];
        }

        public int n;
        public byte[] ops;
    }

    class TestDomainData
    {
        byte[] init;
        int optimalCost;
        long generated;
        int solvedCost;
        long solvedGenerated;
        long solvedExpanded;

        public TestDomainData(byte[] init, int optimalCost, long generated)
        {
            this.init = init;
            this.optimalCost = optimalCost;
            this.generated = generated;
            this.solvedCost = 0;
            this.solvedGenerated = 0;
            this.solvedExpanded = 0;
        }

        public byte[] Init
        {
            get { return init; }
        }

        public int OptimalCost
        {
            get { return optimalCost; }
        }

        public long Generated
        {
            get { return generated; }
        }

        public int SolvedCost
        {
            get { return solvedCost; }
            set { solvedCost = value; }
        }

        public long SolvedGenerated
        {
            get { return solvedGenerated; }
            set { solvedGenerated = value; }
        }

        public long SolvedExpanded
        {
            get { return solvedExpanded; }
            set { solvedExpanded = value; }
        }

    }

    class SummaryStatistics
    {
        double mean;
        double sdModel;
        double sdNoise;

        public SummaryStatistics(double mean, double sdModel, double sdNoise)
        {
            this.mean = mean;
            this.sdModel = sdModel;
            this.sdNoise = sdNoise;

        }

        public double Mean
        {
            get { return mean; }
        }

        public double SDModel
        {
            get { return sdModel; }
        }

        public double SDNoise
        {
            get { return sdNoise; }
        }

    }

    [Serializable]
    class NNFetch
    {
        List<double>[] preds;
        List<double>[] sigmas;
        double cost;

        public NNFetch(int n)
        {
            preds = new List<double>[n];

            for (int i = 0; i < preds.Length; i++)
            {
                preds[i] = new List<double>();
            }

            sigmas = new List<double>[n];

            for (int i = 0; i < sigmas.Length; i++)
            {
                sigmas[i] = new List<double>();
            }

            cost = 0;
        }

        public void AddSampleToPreds(double[] p)
        {
            for (int i = 0; i < p.Length; i++)
            {
                preds[i].Add(p[i]);
            }
        }

        public void AddSampleToSigmas(double[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                sigmas[i].Add(s[i]);
            }
        }

        public void SetCost(double cost)
        {
            this.cost = cost;
        }

        public List<double>[] Preds
        {
            get { return preds; }
        }

        public List<double>[] Sigmas
        {
            get { return sigmas; }
        }

        public double Cost
        {
            get { return cost; }
        }

    }

     class TrainingInstance
    {
        IState state;
        double response;

        public TrainingInstance(IState state, double response)
        {
            this.state = state;
            this.response = response;
        }


        public IState State
        {
            get { return state; }
        }

        public double Response
        {
            get { return response; }
        }

        public override int GetHashCode()
        {
            return new Tuple<int, int>(state.GetHashCode(), response.GetHashCode()).GetHashCode();
        }

        public bool Equals(TrainingInstance trainingInstance)
        {
            return trainingInstance.State.Equals(state) && trainingInstance.response.Equals(response);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(TrainingInstance))
            {
                return false;
            }

            return Equals((TrainingInstance)obj);
        }
    }
}
