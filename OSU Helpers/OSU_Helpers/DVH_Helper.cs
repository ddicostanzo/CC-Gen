using System;
using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace OSU_Helpers
{
    /// <summary>
    /// <c>DvhHelper</c> class to pull DVH metrics from plans or plan sums
    /// </summary>
    public static class DvhHelper
    {

        /// <summary>
        /// Get dose at volume for a given planning item, structure, at a specific volume
        /// </summary>
        /// <param name="pitem">The planning item asking for dose from</param>
        /// <param name="structure">The structure the requested dose metric is being requested from</param>
        /// <param name="volume">The specific volume of the structure to get dose for</param>
        /// <param name="volumePresentation">The volume presentation of the requested volume: absolute (cc) or relative (%)</param>
        /// <param name="requestedDosePresentation">The requested dose presentation for the output, if planning item is a plan sum, absolute dose is required</param>
        /// <returns>Returns a dose value with the requested dose presentation as the units.</returns>
        public static DoseValue GetDoseAtVolume(this PlanningItem pitem, Structure structure, double volume, VolumePresentation volumePresentation, DoseValuePresentation requestedDosePresentation)
        {
            if (pitem is PlanSetup)
            {
                return ((PlanSetup)pitem).GetDoseAtVolume(structure, volume, volumePresentation, requestedDosePresentation);
            }
            else
            {//PlanSum
                if (requestedDosePresentation != DoseValuePresentation.Absolute)
                {
                    throw new ApplicationException("Only absolute dose supported for Plan Sums");
                }

                DVHData dvh = pitem.GetDVHCumulativeData(structure, DoseValuePresentation.Absolute, volumePresentation, 0.001);
                if (dvh == null)
                {
                    throw new ApplicationException("DVH data does not exist. Script execution cancelled.");
                }
                return DoseAtVolume(dvh, volume);
            }
        }

        /// <summary>
        /// Retrieves a volume for a given planning item, structure, at a specific dose
        /// </summary>
        /// <param name="pitem">The planning item for which the volume is being retrieved</param>
        /// <param name="structure">The structure for the requested volume at given dose</param>
        /// <param name="dose">The dose value the specific volume of the structure is being requested</param>
        /// <param name="requestedVolumePresentation">The requested volume presentation of the output: absolute (cc) or relative (%)</param>
        /// <returns>Returns the volume of the specific structure at the requested dose value with the requested volume presentation</returns>
        public static double GetVolumeAtDose(this PlanningItem pitem, Structure structure, DoseValue dose, VolumePresentation requestedVolumePresentation)
        {
            //Ahmet
            //this method accepts DoseValue with either relative (%) or cGy ( as absolute). Gy as absolute does not work.
            if (pitem is PlanSetup)
            {
                return ((PlanSetup)pitem).GetVolumeAtDose(structure, dose, requestedVolumePresentation);
            }
            else
            {//must be plansum
                DVHData dvh = pitem.GetDVHCumulativeData(structure, DoseValuePresentation.Absolute, requestedVolumePresentation, 0.001);
                if (dvh == null)
                {
                    throw new ApplicationException("DVH data does not exist. Script execution cancelled.");
                }
                return VolumeAtDose(dvh, dose.Dose);
            }
        }

        /// <summary>
        /// Helps the other methods by returning a specific dose at a given volume
        /// </summary>
        /// <param name="dvhData">The <c>DVHData</c> requesting the volume from</param>
        /// <param name="volume">The volume a specific dose value is being requested for</param>
        /// <returns>Returns a <c>DoseValue</c> that corresponds to the specific volume provided</returns>
        public static DoseValue DoseAtVolume(DVHData dvhData, double volume)
        {
            if (dvhData == null || dvhData.CurveData.Count() == 0)
            {
                return DoseValue.UndefinedDose();
            }

            double absVolume = dvhData.CurveData[0].VolumeUnit == "%" ? volume * dvhData.Volume * 0.01 : volume;
            if (volume < 0.0 || absVolume > dvhData.Volume)
            {
                return DoseValue.UndefinedDose();
            }

            DVHPoint[] hist = dvhData.CurveData;
            for (int i = 0; i < hist.Length; i++)
            {
                if (hist[i].Volume < volume)
                {
                    return hist[i].DoseValue;
                }
            }
            return DoseValue.UndefinedDose();
        }

        /// <summary>
        /// Helps other methods by returning the specified volume at a requested dose
        /// </summary>
        /// <param name="dvhData">The <c>DVHData</c> requesting the volume from</param>
        /// <param name="dose">The specific dose as a double the volume is being requested from</param>
        /// <returns>Returns a double that is the volume at requested dose.</returns>
        public static double VolumeAtDose(DVHData dvhData, double dose)
        {
            if (dvhData == null)
            {
                return Double.NaN;
            }

            DVHPoint[] hist = dvhData.CurveData;
            int index = (int)(hist.Length * dose / dvhData.MaxDose.Dose);
            if (index < 0 || index > hist.Length)
            {
                return 0.0;//Double.NaN;
            }
            else
            {
                return hist[index].Volume;
            }
        }

        /// <summary>
        /// Returns the minimum dose for a given planning item, structure, in a specific dose presentation
        /// </summary>
        /// <param name="pitem">The planning item asking for dose from</param>
        /// <param name="structure">The structure the requested dose metric is being requested from</param>
        /// <param name="dvp">The output dose value presentation: absolute (cGy/Gy) or relative (%)</param>
        /// <param name="vp">The volume presentation being provided for DVH data: absolute (cc) or relative (%)</param>
        /// <returns>Returns a <c>DoseValue</c> with the minimum dose of the requested structure for a given plan</returns>
        public static DoseValue GetMinimumDose(this PlanningItem pitem, Structure structure, DoseValuePresentation dvp, VolumePresentation vp)
        {
            DVHData dvh = pitem.GetDVHCumulativeData(structure, dvp, vp, 0.01);
            if (dvh == null)
            {
                throw new ApplicationException("DVH data does not exist. Script execution cancelled.");
            }
            return dvh.MinDose;
        }

        /// <summary>
        /// Returns the maximum dose for a given planning item, structure, in a specific dose presentation
        /// </summary>
        /// <param name="pitem">The planning item asking for dose from</param>
        /// <param name="structure">The structure the requested dose metric is being requested from</param>
        /// <param name="dvp">The output dose value presentation: absolute (cGy/Gy) or relative (%)</param>
        /// <param name="vp">The volume presentation being provided for DVH data: absolute (cc) or relative (%)</param>
        /// <returns>Returns a <c>DoseValue</c> with the maximum dose of the requested structure for a given plan</returns>
        public static DoseValue GetMaximumDose(this PlanningItem pitem, Structure structure, DoseValuePresentation dvp, VolumePresentation vp)
        {
            DVHData dvh = pitem.GetDVHCumulativeData(structure, dvp, vp, 0.01);
            if (dvh == null)
            {
                throw new ApplicationException("DVH data does not exist. Script execution cancelled.");
            }
            return dvh.MaxDose;
        }

        /// <summary>
        /// Returns the mean dose for a given planning item, structure, in a specific dose presentation
        /// </summary>
        /// <param name="pitem">The planning item asking for dose from</param>
        /// <param name="structure">The structure the requested dose metric is being requested from</param>
        /// <param name="dvp">The output dose value presentation: absolute (cGy/Gy) or relative (%)</param>
        /// <param name="vp">The volume presentation being provided for DVH data: absolute (cc) or relative (%)</param>
        /// <returns>Returns a <c>DoseValue</c> with the mean dose of the requested structure for a given plan</returns>
        public static DoseValue GetMeanDose(this PlanningItem pitem, Structure structure, DoseValuePresentation dvp, VolumePresentation vp)
        {
            DVHData dvh = pitem.GetDVHCumulativeData(structure, dvp, vp, 0.01);
            if (dvh == null)
            {
                throw new ApplicationException("DVH data does not exist. Script execution cancelled.");
            }
            return dvh.MeanDose;
        }

        /// <summary>
        /// Returns the median dose for a given planning item, structure, in a specific dose presentation
        /// </summary>
        /// <param name="pitem">The planning item asking for dose from</param>
        /// <param name="structure">The structure the requested dose metric is being requested from</param>
        /// <param name="dvp">The output dose value presentation: absolute (cGy/Gy) or relative (%)</param>
        /// <param name="vp">The volume presentation being provided for DVH data: absolute (cc) or relative (%)</param>
        /// <returns>Returns a <c>DoseValue</c> with the median dose of the requested structure for a given plan</returns>
        public static DoseValue GetMedianDose(this PlanningItem pitem, Structure structure, DoseValuePresentation dvp, VolumePresentation vp)
        {
            DVHData dvh = pitem.GetDVHCumulativeData(structure, dvp, vp, 0.01);
            if (dvh == null)
            {
                throw new ApplicationException("DVH data does not exist. Script execution cancelled.");
            }
            return dvh.MedianDose;
        }

        /// <summary>
        /// An extension method of <c>PlanSetup</c> to calculate the gEUD of a specific structure for a specific parameter a
        /// </summary>
        /// <param name="pitem">The plan that the gEUD is to be calculated from</param>
        /// <param name="structure">The structure the gEUD should be calculated for</param>
        /// <param name="a">THe parameter A for the gEUD</param>
        /// <param name="binwidth">The sampling width of the DVH for calculation of the gEUD</param>
        /// <returns>The gEUD for the specific structure from the specified plan for a specific "Ba" parameter </returns>
        public static double gEUD(this PlanSetup pitem, Structure structure, double a, double binwidth = 0.1)
        {
            pitem.DoseValuePresentation = DoseValuePresentation.Absolute;

            var dvh = pitem.GetDVHCumulativeData(structure, DoseValuePresentation.Relative, VolumePresentation.Relative, binwidth);

            double gEUD = 0;

            for (int i = 0; i < dvh.CurveData.Length - 1; ++i)
            {
                gEUD += (dvh.CurveData[i].Volume - dvh.CurveData[i + 1].Volume) * Math.Pow((i * binwidth + binwidth / 2), a) / 100;
            }

            gEUD = Math.Pow(gEUD, 1 / a);
            
            var preDose = pitem.TotalDose.Dose;
            var doseUnit = pitem.TotalDose.UnitAsString;
            var preDoseInGy = doseUnit == "cGy" ? preDose : preDose / 100;

            gEUD = gEUD * preDoseInGy / 100;

            return gEUD;
        }

        /// <summary>
        /// An extension method of <c>PlanSum</c> to calculate the gEUD of a specific structure for a specific parameter a
        /// </summary>
        /// <param name="pitem">The plan that the gEUD is to be calculated from</param>
        /// <param name="structure">The structure the gEUD should be calculated for</param>
        /// <param name="a">THe parameter A for the gEUD</param>
        /// <param name="binwidth">The sampling width of the DVH for calculation of the gEUD</param>
        /// <returns>The gEUD for the specific structure from the specified plan for a specific "Ba" parameter </returns>
        public static double gEUD(this PlanSum pitem, Structure structure, double a, double binwidth = 0.1)
        {
            pitem.DoseValuePresentation = DoseValuePresentation.Absolute;

            var dvh = pitem.GetDVHCumulativeData(structure, DoseValuePresentation.Relative, VolumePresentation.Relative, binwidth);

            double gEUD = 0;

            for (int i = 0; i < dvh.CurveData.Length - 1; ++i)
            {
                gEUD += (dvh.CurveData[i].Volume - dvh.CurveData[i + 1].Volume) * Math.Pow((i * binwidth + binwidth / 2), a) / 100;
            }

            gEUD = Math.Pow(gEUD, 1 / a);

            var preDose = pitem.PlanSetups.Sum(b=>b.TotalDose.Dose);
            var doseUnit = pitem.PlanSetups.First().TotalDose.UnitAsString;
            var preDoseIncGy = doseUnit == "cGy" ? preDose : preDose / 100;

            gEUD = gEUD * preDoseIncGy / 100;

            return gEUD;
        }

        /// <summary>
        /// The extension method for planning items that allows calling the differential DVH
        /// </summary>B
        /// <param name="pitem">The planning item being used to call the differential DVH</param>
        /// <param name="structure">The structure the differential DVH should be calculated for</param>
        /// <param name="dvp">The dose presentaiton: relative or absolute</param>
        /// <param name="vp">The volume presentation: relative or absolute</param>
        /// <param name="binwidth">The bin width for each bin of the differential DVH</param>
        /// <returns>Returns a list of <c>DVHPoint</c>s that contain the differential DVH</returns>
        public static List<DVHPoint> GetDVHDifferentialData(this PlanningItem pitem, Structure structure, DoseValuePresentation dvp, VolumePresentation vp, double binwidth = 0.01)
        {
            DVHData dvh = pitem.GetDVHCumulativeData(structure, dvp, vp, binwidth);
            List<DVHPoint> ll = new List<DVHPoint>();//dose,volume

            DoseValue.DoseUnit du = (dvp == DoseValuePresentation.Absolute) ? DoseValue.DoseUnit.cGy : DoseValue.DoseUnit.Percent;
            string vu = (vp == VolumePresentation.AbsoluteCm3) ? "cc" : "%";

            int N = dvh.CurveData.Count();
            double BIN_DOSE = Math.Round((dvh.CurveData[1].DoseValue.Dose - dvh.CurveData[0].DoseValue.Dose), 2);

            for (int i = 0; i < dvh.CurveData.Count() - 1; i++)
            {
                double V = dvh.CurveData[i].Volume;
                double dV = (dvh.CurveData[i].Volume - dvh.CurveData[i + 1].Volume) / BIN_DOSE;
                double dD = dvh.CurveData[i].DoseValue.Dose + BIN_DOSE / 2.0;
                DoseValue doseval = new DoseValue(dD, du);
                ll.Add(new DVHPoint(doseval, dV, vu));
            }
            //enter the last element
            DVHPoint lastelement = new DVHPoint(new DoseValue(dvh.CurveData[dvh.CurveData.Count() - 1].DoseValue.Dose + BIN_DOSE / 2, du)
                                                , dvh.CurveData[dvh.CurveData.Count() - 1].Volume , vu);
            ll.Add(lastelement);

            return ll;
        }

    }
}
