using HeuristicSearch.FastNN.ActivationFuncs;
using HeuristicSearch.FastNN.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN
{
    [Serializable]
    class DenseLayer
    {
        Weights weights;
        Bias bias;
        IActivationFunc activationFunc;
        Weights gradWeights;
        Bias gradBias;
       
        public DenseLayer(int numNeurons, int numNeuronsNext, IActivationFunc activationFunc, IInitialization initialization)
        {
            weights = new Weights(numNeurons, numNeuronsNext,initialization);
            bias = new Bias(numNeuronsNext);
            gradWeights = new Weights(numNeurons, numNeuronsNext,initialization);
            gradBias = new Bias(numNeuronsNext);
            this.activationFunc = activationFunc;
        }

        public DenseLayer()
        {

        }

        public DenseLayer CopyOnlyParams()
        {
            DenseLayer layer = new DenseLayer(); 
            layer.weights = new Weights(this.weights);
            layer.bias = new Bias(this.bias);
            layer.activationFunc = this.activationFunc;
            return layer;
        }

        public Weights Weights
        {
            get { return weights; }
            set { weights = value; }
        }
        
        public Bias Bias
        {
            get { return bias; }
            set { bias = value; }
        }


        public Weights GradWeights
        {
            get { return gradWeights; }
            set { gradWeights = value; }
        }

        public Bias GradBias
        {
            get { return gradBias; }
            set { gradBias = value; }
        }

        public IActivationFunc ActivationFunc
        {
            get { return activationFunc; }
        }

    }
}
