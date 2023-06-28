using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSU_Helpers
{
    /// <summary>
    /// Calculates stardard devication of collection
    /// </summary>
    public static class StandardDeviation
    {
        /// <summary>
        /// Calculates standard deviation of a collection of numbers
        /// </summary>
        /// <param name="NumberCollection">Numbers the standard deviation is to be calculated from</param>
        /// <returns>Standard deviation of input</returns>
        public static double CalculateStdDev(IEnumerable<double> NumberCollection)
        {
            double ret = 0;
            if (NumberCollection.Count() > 0)
            {
                //Compute the Average      
                double avg = NumberCollection.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = NumberCollection.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (NumberCollection.Count() - 1));
            }
            return ret;
        }
    }
}
