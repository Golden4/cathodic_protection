using System;

namespace ParallelPipesIntervals.Core
{
    [Serializable]
    public class LinearSystem
    {
        [field: NonSerialized()]
        private double[,] aMatrix;
        [field: NonSerialized()]
        private double[] bVector;
        [field: NonSerialized()]
        private double eps;
        [field: NonSerialized()]
        private int size;

        private double[] xVector;

        public LinearSystem(double[,] aMatrix, double[] bVector)
            : this(aMatrix, bVector, 1e-16)
        {
        }

        public LinearSystem(double[,] aMatrix, double[] bVector, double eps)
        {
            int bLength = bVector.Length;
            int aLength = aMatrix.Length;
            this.aMatrix = aMatrix;
            this.bVector = bVector;
            this.xVector = new double[bLength];
            this.size = bLength;
            this.eps = eps;
            GaussSolve();
        }

        public double[] XVector => xVector;

        private int[] InitIndex()
        {
            int[] index = new int[size];
            for (int i = 0; i < index.Length; ++i)
                index[i] = i;
            return index;
        }

        private double FindR(int row, int[] index)
        {
            int maxIndex = row;
            double max = aMatrix[row, index[maxIndex]];
            double maxAbs = Math.Abs(max);
            for (int curIndex = row + 1; curIndex < size; ++curIndex)
            {
                double cur = aMatrix[row, index[curIndex]];
                double curAbs = Math.Abs(cur);
                if (curAbs > maxAbs)
                {
                    maxIndex = curIndex;
                    max = cur;
                    maxAbs = curAbs;
                }
            }
            if (maxAbs < eps)
            {
                if (Math.Abs(bVector[row]) > eps)
                    throw new Exception("Система уравнений несовместна.");
                else
                    throw new Exception("Система уравнений имеет множество решений..");
            }
            int temp = index[row];
            index[row] = index[maxIndex];
            index[maxIndex] = temp;
            return max;
        }

        private void GaussSolve()
        {
            int[] index = InitIndex();
            GaussForwardStroke(index);
            GaussBackwardStroke(index);
        }

        private void GaussForwardStroke(int[] index)
        {
            for (int i = 0; i < size; ++i)
            {
                double r = FindR(i, index);
                for (int j = 0; j < size; ++j)
                    aMatrix[i, j] /= r;
                bVector[i] /= r;
                for (int k = i + 1; k < size; ++k)
                {
                    double p = aMatrix[k, index[i]];
                    for (int j = i; j < size; ++j)
                        aMatrix[k, index[j]] -= aMatrix[i, index[j]] * p;
                    bVector[k] -= bVector[i] * p;
                    aMatrix[k, index[i]] = 0.0;
                }
            }
        }

        private void GaussBackwardStroke(int[] index)
        {
            for (int i = size - 1; i >= 0; --i)
            {
                double xi = bVector[i];
                for (int j = i + 1; j < size; ++j)
                {
                    xi -= xVector[index[j]] * aMatrix[i, index[j]];
                }

                xVector[index[i]] = xi;
            }
        }
    }
}