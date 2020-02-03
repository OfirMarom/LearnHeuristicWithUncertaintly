using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicSearch.FastNN.Optimizers
{
    [Serializable]
    class Adam : IOptimizer
    {
        float learningRate;
        float beta1;
        float beta2;
        float eps;
        int t;
        bool first;
        Matrix<float>[] Wms;
        Matrix<float>[] Wvs;
        Matrix<float>[] Wms_hat;
        Matrix<float>[] Wvs_hat;
        Matrix<float>[] Wgs;
        Matrix<float>[] Wsteps;
        Matrix<float>[] bms;
        Matrix<float>[] bvs;
        Matrix<float>[] bms_hat;
        Matrix<float>[] bvs_hat;
        Matrix<float>[] bgs;
        Matrix<float>[] bsteps;


        public Adam(float learningRate, float beta1 = 0.9F, float beta2 = 0.999F, float eps = 1e-8F)
        {
            this.learningRate = learningRate;
            this.beta1 = beta1;
            this.beta2 = beta2;
            this.eps = eps;
            this.t = 0;
            first = true;
        }

        private void InitContainers(DenseLayer[] layers)
        {
            Wms = new Matrix<float>[layers.Length];
            Wvs = new Matrix<float>[layers.Length];
            Wms_hat = new Matrix<float>[layers.Length];
            Wvs_hat = new Matrix<float>[layers.Length];
            Wgs = new Matrix<float>[layers.Length];
            Wsteps = new Matrix<float>[layers.Length];
            bms = new Matrix<float>[layers.Length];
            bvs = new Matrix<float>[layers.Length];
            bms_hat = new Matrix<float>[layers.Length];
            bvs_hat = new Matrix<float>[layers.Length];
            bgs = new Matrix<float>[layers.Length];
            bsteps = new Matrix<float>[layers.Length];
            
            for (int i = 0; i < layers.Length; i++)
            {
                DenseLayer layer = layers[i];
                Matrix<float> Wg = layer.GradWeights.Vals;
                Matrix<float> bg = layer.GradBias.Vals;
                Wms[i] = DenseMatrix.Create(Wg.RowCount, Wg.ColumnCount, 0);
                Wvs[i] = DenseMatrix.Create(Wg.RowCount, Wg.ColumnCount, 0);
                Wms_hat[i] = DenseMatrix.Create(Wg.RowCount, Wg.ColumnCount, 0);
                Wvs_hat[i] = DenseMatrix.Create(Wg.RowCount, Wg.ColumnCount, 0);
                Wgs[i] = DenseMatrix.Create(Wg.RowCount, Wg.ColumnCount, 0);
                Wsteps[i] = DenseMatrix.Create(Wg.RowCount, Wg.ColumnCount, 0);
                bms[i] = DenseMatrix.Create(bg.RowCount, bg.ColumnCount, 0);
                bvs[i] = DenseMatrix.Create(bg.RowCount, bg.ColumnCount, 0);
                bms_hat[i] = DenseMatrix.Create(bg.RowCount, bg.ColumnCount, 0);
                bvs_hat[i] = DenseMatrix.Create(bg.RowCount, bg.ColumnCount, 0);
                bgs[i] = DenseMatrix.Create(bg.RowCount, bg.ColumnCount, 0);
                bsteps[i] = DenseMatrix.Create(bg.RowCount, bg.ColumnCount, 0);

            }
        }

        private void UpdateM(Matrix<float> m, Matrix<float> g)
        {
            m.Multiply(beta1, m);
            g.Multiply(1 - beta1, g);
            m.Add(g, m);
        }

        private void UpdateV(Matrix<float> v,Matrix<float> g)
        {
            v.Multiply(beta2, v);
            g.PointwiseMultiply(g, g);
            g.Multiply(1 - beta2, g);
            v.Add(g, v);
        }

        private void UpdateMHat(Matrix<float> m_hat, Matrix<float> m)
        {
            m.CopyTo(m_hat);
            m_hat.Divide(1 - (float)Math.Pow(beta1, t), m_hat);
        }

        private void UpdateVHat(Matrix<float> v_hat, Matrix<float> v)
        {
            v.CopyTo(v_hat);
            v_hat.Divide(1 - (float)Math.Pow(beta2, t), v_hat);
        }

        private void UpdateStep(Matrix<float> step, Matrix<float> m_hat,Matrix<float> v_hat)
        {
            m_hat.Multiply(learningRate, m_hat);
            v_hat.PointwiseSqrt(v_hat);
            v_hat.Add(eps, v_hat);
            m_hat.CopyTo(step);
            step.PointwiseDivide(v_hat, step);
        }

        public void UpdateParams(DenseLayer[] layers)
        {

            if(first)
            {
                InitContainers(layers);
                first = false;
            }

            t++;

            for (int i = 0; i < layers.Length; i++)
            {
                DenseLayer layer = layers[i];
                Matrix<float> W = layer.Weights.Vals;
                Matrix<float> b = layer.Bias.Vals;
                Matrix<float> WgOrig = layer.GradWeights.Vals;
                Matrix<float> bgOrig = layer.GradBias.Vals;

                Matrix<float> Wm = Wms[i];
                Matrix<float> Wv = Wvs[i];
                Matrix<float> Wm_hat = Wms_hat[i];
                Matrix<float> Wv_hat = Wvs_hat[i];
                Matrix<float> Wg = Wgs[i];
                Matrix<float> Wstep = Wsteps[i];
                Matrix<float> bm = bms[i];
                Matrix<float> bv = bvs[i];
                Matrix<float> bg = bgs[i];
                Matrix<float> bm_hat = bms_hat[i];
                Matrix<float> bv_hat = bvs_hat[i];
                Matrix<float> bstep = bsteps[i];

                WgOrig.CopyTo(Wg);
                UpdateM(Wm, Wg);

                WgOrig.CopyTo(Wg);
                UpdateV(Wv, Wg);

                bgOrig.CopyTo(bg);
                UpdateM(bm, bg);

                bgOrig.CopyTo(bg);
                UpdateV(bv,bg);


                UpdateMHat(Wm_hat, Wm);

                UpdateVHat(Wv_hat, Wv);

                UpdateMHat(bm_hat, bm);

                UpdateVHat(bv_hat, bv);


                UpdateStep(Wstep, Wm_hat, Wv_hat);

                UpdateStep(bstep, bm_hat, bv_hat);


                W.Subtract(Wstep, W);

                b.Subtract(bstep, b);
            }
        }
    }
}
