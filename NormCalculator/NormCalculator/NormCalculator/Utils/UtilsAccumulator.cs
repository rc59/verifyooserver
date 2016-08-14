using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormCalculator.Utils
{
    class UtilsAccumulator
    {
        private int n = 0;          // number of data values
        private double sum = 0.0;   // sample variance * (n-1)
        private double mu = 0.0;    // sample mean

        /**
         * Initializes an accumulator.
         */
        public UtilsAccumulator()
        {
        }

        /**
         * Adds the specified data value to the accumulator.
         * @param  x the data value
         */
        public void AddDataValue(double x)
        {
            n++;
            double delta = x - mu;
            mu += delta / n;
            sum += (double)(n - 1) / n * delta * delta;
        }

        /**
         * Returns the mean of the data values.
         * @return the mean of the data values
         */
        public double Mean()
        {
            return mu;
        }

        /**
         * Returns the sample variance of the data values.
         * @return the sample variance of the data values
         */
        public double Var()
        {
            return sum / (n - 1);
        }

        /**
         * Returns the sample standard deviation of the data values.
         * @return the sample standard deviation of the data values
         */
        public double Stddev()
        {
            return Math.Sqrt(this.Var());
        }

        /**
         * Returns the number of data values.
         * @return the number of data values
         */
        public int Count()
        {
            return n;
        }
    }
}
