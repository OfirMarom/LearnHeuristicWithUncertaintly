using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra;
using HeuristicSearch.FastNN.ActivationFuncs;
using HeuristicSearch.FastNN.Initialization;
using HeuristicSearch.FastNN.Optimizers;

namespace HeuristicSearch.FastNN
{
    [Serializable]
    class FastNN
    {
        DenseLayer[] layers;
        DropoutLayer[] dropouts;
        bool computeSDOutput;
        int miniBatchSize;
        Matrix<float>[] activations;
        Matrix<float>[] preActivations;
        Matrix<float>[] deltas;
        Matrix<float>[] singleActivations;
        
        IOptimizer optimizer;
        bool l2loss;


        public FastNN CopyOnlyParams()
        {
            FastNN nn = new FastNN();

            nn.layers = new DenseLayer[this.layers.Length];

            nn.singleActivations = new Matrix<float>[this.singleActivations.Length];



            nn.computeSDOutput = this.computeSDOutput;

            nn.l2loss = this.l2loss;

            for(int i=0;i<nn.layers.Length;i++)
            {
                nn.layers[i] = this.layers[i].CopyOnlyParams();
            }

            for(int i=0;i<nn.singleActivations.Length;i++)
            {
                nn.singleActivations[i] = DenseMatrix.Create(this.singleActivations[i].RowCount, this.singleActivations[i].ColumnCount, 0);
            }

            return nn;

        }

        public FastNN()
        {
        }

        public FastNN(int[] numNeurons, int miniBatchSize, bool l2loss, float dropoutProb= 0)
        {
            this.l2loss = l2loss;

            layers = new DenseLayer[numNeurons.Length - 1];

            dropouts = new DropoutLayer[numNeurons.Length - 1];

            activations = new Matrix<float>[numNeurons.Length];

            singleActivations = new Matrix<float>[numNeurons.Length];

            preActivations = new Matrix<float>[numNeurons.Length-1];

            deltas = new Matrix<float>[numNeurons.Length - 1];
       
            this.miniBatchSize = miniBatchSize;

            optimizer = new Adam(0.001F);

            IActivationFunc activationFunc = new Relu();

            IInitialization initialization = new HeNormal();

            for (int i=0;i<numNeurons.Length;i++)
            {
                activations[i] = DenseMatrix.Create(miniBatchSize, numNeurons[i], 0);
                singleActivations[i] = DenseMatrix.Create(1, numNeurons[i], 0);

                if (i==0)
                {            
                    continue;
                }


                if (i == numNeurons.Length - 1)
                {
                    activationFunc = new Linear();
                }

                preActivations[i-1] = DenseMatrix.Create(miniBatchSize, numNeurons[i], 0);
                layers[i-1] = new DenseLayer(numNeurons[i- 1], numNeurons[i], activationFunc, initialization);
                deltas[i-1] = DenseMatrix.Create(miniBatchSize, numNeurons[i], 0);

                if(dropoutProb > 0 && i < numNeurons.Length-1)
                {
                    dropouts[i - 1] = new DropoutLayer(miniBatchSize, numNeurons[i], dropoutProb);
                }
              
            }


            computeSDOutput= false;

            if (numNeurons.Last()==2)
            {
               computeSDOutput = true;
            }    
        }


        public float[][] Forward(Matrix<float> X, bool pred)
        {
            Matrix<float>[] activations;

            if(pred== false)
            {
                activations = this.activations;
            }
            else
            {
                activations = this.singleActivations;
            }

            activations[0] = X;

            Matrix<float> a = activations[0];

            for (int i=0;i<layers.Length;i++)
            {
                DenseLayer layer = layers[i];
                Matrix<float> W = layer.Weights.Vals;
                Matrix<float> b = layer.Bias.Vals;
                IActivationFunc activationFunc = layer.ActivationFunc;

                DropoutLayer dropout=null;
                
                if(dropouts!=null)
                {
                    dropout =dropouts[i];
                }
              
                

                if (pred==false)
                {
                    b = layer.Bias.Broadcast(miniBatchSize);
                }

                Matrix<float> aNext = activations[i + 1];
                a.Multiply(W,aNext);
                aNext.Add(b, aNext);

                if (pred==false)
                {
                   aNext.CopyTo(preActivations[i]);
                }

                if(activationFunc.GetType() != typeof(Linear))
                {
                    aNext.MapInplace(activationFunc.F, Zeros.Include);
                }

                if (pred == false && dropout != null)
                {
                    dropout.Sample();
                    aNext.PointwiseMultiply(dropout.Vals, aNext);              
                }


                a = aNext;
            }

            return a.ToColumnArrays();
        }

        private void SetOutputDelta(Matrix<float> X, Vector<float> y)
        {
            float[][] forward = Forward(X, false);

            Matrix<float> delta = deltas.Last();

            for (int r = 0; r < delta.RowCount; r++)
            {
                if (computeSDOutput == false)
                {
                    if(l2loss==true)
                    {
                        delta[r, 0] = forward[0][r] - y[r];
                    }
                    else
                    {
                        delta[r, 0] = forward[0][r] > y[r] ? 1 : -1;
                    }
                }
                else
                {
                    if(l2loss==true)
                    {
                        float phi = forward[1][r];

                        phi = (float) Global.TrimPhi(phi);

                        float sigma = (float)Global.ToSigma(phi);

                        float obsDiff = forward[0][r] - y[r];

                        float expPhi = (float)Math.Exp(phi);

                        delta[r, 0] = obsDiff / (sigma * sigma);

                        delta[r, 1] = -(obsDiff * obsDiff * expPhi) / ((sigma * sigma * sigma) * (expPhi + 1)) + expPhi / (sigma * (expPhi + 1));

                    }
                    else
                    {
                        float ind = forward[0][r] > y[r] ? 1 : -1;

                        float phi = forward[1][r];

                        phi = (float)Global.TrimPhi(phi);

                        float sigma = (float)Global.ToSigma(phi);

                        float obsDiff = forward[0][r] - y[r];

                        float expPhi = (float)Math.Exp(phi);

                        delta[r, 0] = ind / sigma;

                        delta[r, 1] = -(Math.Abs(obsDiff) * expPhi) / ((sigma * sigma) * (expPhi + 1)) +  expPhi / (sigma * (expPhi + 1));
                    }
                }
            }
        }

        private void Backward(Matrix<float> X, Vector<float> y)
        {
            DenseLayer layer = layers.Last();

            Matrix<float> delta = deltas.Last();

            for (int i = layers.Length - 1; i >= 0; i--)
            {
             
                if (i==layers.Length-1)
                {
                    SetOutputDelta(X, y);
                    continue;
                }

                DenseLayer prevLayer = layers[i];
                Matrix<float> W = layer.Weights.Vals;
                Matrix<float> preActivation = preActivations[i];
                DropoutLayer dropout = dropouts[i];
                IActivationFunc activationFunc = prevLayer.ActivationFunc;
                Matrix<float> deltaNew = deltas[i];
                delta.TransposeAndMultiply(W, deltaNew);
                preActivation.MapInplace(activationFunc.dF, Zeros.Include);
                deltaNew.PointwiseMultiply(preActivation, deltaNew);
                delta = deltaNew;
                layer = prevLayer;
            }

            for(int i=layers.Length-1;i>=0;i--)
            {
                layer = layers[i];
                delta = deltas[i];
                Matrix<float> gradWeights = layer.GradWeights.Vals;
                Matrix<float> gradBias = layer.GradBias.Vals;
                Matrix<float> a = activations[i];
                a.TransposeThisAndMultiply(delta, gradWeights);
                gradWeights.Divide(miniBatchSize, gradWeights);
                Vector<float> gradBiasVect = delta.ColumnSums();
                gradBiasVect.Divide(miniBatchSize, gradBiasVect);
                gradBias.SetRow(0, gradBiasVect);
            }
        }

     
        public void Learn(Matrix<float> X, Vector<float> y)
        {
            Backward(X, y);
            optimizer.UpdateParams(layers);

        }

        public bool ComputeSDOutput
        {
            get { return computeSDOutput; }
        }

    }
}
