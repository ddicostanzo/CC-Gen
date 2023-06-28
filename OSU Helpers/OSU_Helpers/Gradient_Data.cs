using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OSU_Helpers.Enumerables;

namespace OSU_Helpers
{
    /// <summary>
    /// The Gradient Fit data that is needed to perform the model fits.
    /// </summary>
    public class Gradient_Data
    {
        /// <summary>
        /// The A parameter of the fit
        /// </summary>
        public double A;
        /// <summary>
        /// The B parameter of the fit
        /// </summary>
        public double B;
        /// <summary>
        /// The the number of PTVs from the fit data. Values 1-7
        /// </summary>
        public int NumberOfPTVs { get; set; }
        /// <summary>
        /// Enumerable for gradient designation. Options are 20, 50, or UserDefined
        /// </summary>
        public Gradient GradientDesignation { get; set; }

        /// <summary>
        /// Constructor for the Gradient Data object. Requires input from the fits.
        /// </summary>
        /// <param name="list_of_data">A string array from the fit data to parse into the model</param>
        /// <param name="gradient_desig">The specific gradient designation of this fit</param>
        public Gradient_Data(string[] list_of_data, Gradient gradient_desig)
        {
            NumberOfPTVs = int.Parse(list_of_data[0]);
            A = double.Parse(list_of_data[1]);
            B = double.Parse(list_of_data[2]);
            GradientDesignation = gradient_desig;
        }

        /// <summary>
        /// The predicted gradient volume based upon the fit 
        /// </summary>
        /// <param name="PTV_Volume">The total PTV Volume of all targets.</param>
        /// <returns>A double that is the predicted Gradient Volume based upon the fit</returns>
        public double PredictedGradientVolume(double PTV_Volume)
        {
            return (A * PTV_Volume + B); 
        }

        /// <summary>
        /// The predicted IDL ring radius for a specific PTV Volume
        /// </summary>
        /// <param name="PTV_Volume">The total PTV Volume of all targets.</param>
        /// <returns>A double that is the predicted IDL radius based upon the fit</returns>
        public double PredictedIDLRingRadius(double PTV_Volume)
        {
            return (Math.Pow(3.0 / 4.0 * 1 / Math.PI * PredictedGradientVolume(PTV_Volume), (1.0 / 3.0))); 
        }

        /// <summary>
        /// The predicted ring for controlling Rx dose based upon the fit 
        /// </summary>
        /// <param name="PTV_Volume">The total PTV Volume of all targets.</param>
        /// <returns>A double that is the radius of the ring for the Rx dose control structure based upon the fit</returns>
        public double PredictedRxRingRadius(double PTV_Volume)
        {
            return (Math.Pow(3.0 / 4.0 * 1 / Math.PI * PTV_Volume, (1.0 / 3.0))); 
        }

        /// <summary>
        /// The predicted distance the IDL ring should be from the Rx Ring based upon the model
        /// </summary>
        /// <param name="PTV_Volume">The total PTV Volume of all targets.</param>
        /// <returns>A double that is the predicted distance the IDL ring should be from the Rx Ring based upon the model</returns>
        public double PredictedRingDistanceFromTarget(double PTV_Volume)
        {
            return (PredictedIDLRingRadius(PTV_Volume) - PredictedRxRingRadius(PTV_Volume)); 
        }
    }
}
