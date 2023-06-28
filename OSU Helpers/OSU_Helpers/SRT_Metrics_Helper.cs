using System;
using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace OSU_Helpers
{
    /// <summary>
    /// Helper methods for SRT Metrics computation
    /// </summary>
    public static class SRT_Metrics_Helper
    {
        /// <summary>
        /// Extension method for structure to determine whether a structure is covered by the calculation box. Calculation is double.IsNaN(1 / structure.Volume) 
        /// </summary>
        /// <param name="structure">The structure to verify calculation box coverage</param>
        /// <returns>A boolean is returned: true = calc box covers structure, false in all other cases</returns>
        private static bool DetermineIfVolumesAreIncludedInCalculationBox(this Structure structure)
        {
            if (double.IsNaN(1 / structure.Volume))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Extension method of <c>PlanSetup</c> the volume of a given structure that is receiving 20% of the prescribed dose.
        /// </summary>
        /// <param name="ps">Extension of PlanSetup</param>
        /// <param name="str">The structure this metric should be calculated for.</param>
        /// <returns>Returns a double that is the absolute volume of the structure receiving at least 20% of the prescribed dose.</returns>
        public static double VolumeAtDose20(this PlanSetup ps, Structure str)
        {
            return VolumeAtDoseX(ps, str, 0.2);
        }

        /// <summary>
        /// Extension method of <c>PlanSetup</c> the volume of a given structure that is receiving 50% of the prescribed dose.
        /// </summary>
        /// <param name="ps">Extension of PlanSetup</param>
        /// <param name="str">The structure this metric should be calculated for.</param>
        /// <returns>Returns a double that is the absolute volume of the structure receiving at least 50% of the prescribed dose.</returns>
        public static double VolumeAtDose50(this PlanSetup ps, Structure str)
        {
            return VolumeAtDoseX(ps, str, 0.5);
        }

        /// <summary>
        /// Extension method of <c>PlanSetup</c> the volume of a given structure that is receiving 100% of the prescribed dose.
        /// </summary>
        /// <param name="ps">Extension of PlanSetup</param>
        /// <param name="str">The structure this metric should be calculated for.</param>
        /// <returns>Returns a double that is the absolute volume of the structure receiving at least 100% of the prescribed dose.</returns>
        public static double VolumeAtDose100(this PlanSetup ps, Structure str)
        {
            return VolumeAtDoseX(ps, str, 1);
        }

        /// <summary>
        /// Extension method of <c>PlanSetup</c> the volume of a given structure that is receiving a user defined percentage of the prescribed dose.
        /// </summary>
        /// <param name="ps">Extension of PlanSetup</param>
        /// <param name="str">The structure this metric should be calculated for.</param>
        /// <param name="total_dose_percent">The percent of the prescribed dose the volume is being requested for. Allowed values are 0-1.5 which corresponds to 0-150%</param>
        /// <returns>Returns a double that is the absolute volume of the structure receiving at least the user defined percentage of the prescribed dose.</returns>
        public static double VolumeAtDoseX(this PlanSetup ps, Structure str, double total_dose_percent)
        {
            total_dose_percent = (total_dose_percent > 1.5) ? (total_dose_percent / 100) : total_dose_percent;
            return ps.GetVolumeAtDose(str, ps.TotalDose * total_dose_percent, VolumePresentation.AbsoluteCm3);
        }

        /// <summary>
        /// The planned ratio of the corresponding 20% isodose line to the 100% isodose line with regards to a specific structure.
        /// </summary>
        /// <param name="ps">Extension of PlanSetup</param>
        /// <param name="str">The structure this metric should be calculated for.</param>
        /// <param name="decimals">An optional parameter that defaults to 2, but the number of decimal places being requested.</param>
        /// <returns>Returns a double that is the ratio of the volume of the 20% IDL to the 100% IDL</returns>
        public static double PlannedGradient20(this PlanSetup ps, Structure str, int decimals = 2)
        {
            return PlannedGradientX(ps, str, 0.2);
        }

        /// <summary>
        /// The planned ratio of the corresponding 50% isodose line to the 100% isodose line with regards to a specific structure.
        /// </summary>
        /// <param name="ps">Extension of PlanSetup</param>
        /// <param name="str">The structure this metric should be calculated for.</param>
        /// <param name="decimals">An optional parameter that defaults to 2, but the number of decimal places being requested.</param>
        /// <returns>Returns a double that is the ratio of the volume of the 50% IDL to the 100% IDL</returns>
        public static double PlannedGradient50(this PlanSetup ps, Structure str, int decimals = 2)
        {
            return PlannedGradientX(ps, str, 0.5);
        }

        /// <summary>
        /// The planned ratio of the corresponding user defined isodose line to the 100% isodose line with regards to a specific structure.
        /// </summary>
        /// <param name="ps">Extension of PlanSetup</param>
        /// <param name="str">The structure this metric should be calculated for.</param>
        /// <param name="total_dose_percent">The percent of the prescribed dose the volume is being requested for. Allowed values are 0-1.5 which corresponds to 0-150%</param>
        /// <param name="decimals">An optional parameter that defaults to 2, but the number of decimal places being requested.</param>
        /// <returns>Returns a double that is the ratio of the volume of the user defined IDL to the 100% IDL</returns>
        public static double PlannedGradientX(this PlanSetup ps, Structure str, double total_dose_percent, int decimals = 2)
        {
            total_dose_percent = (total_dose_percent > 1.5) ? (total_dose_percent / 100) : total_dose_percent;
            return (Math.Round(ps.VolumeAtDoseX(str, total_dose_percent) / ps.VolumeAtDose100(str), decimals));
        }

        /// <summary>
        /// Structure list extension to provide the total volume of all structures in the list
        /// </summary>
        /// <param name="structures">A generic collection of structures.</param>
        /// <returns>A double is returned that is the summation of all structure volumes in the list</returns>
        public static double TotalVolume(this IEnumerable<Structure> structures)
        {
            return structures.Sum(a => a.Volume);
        }

        /// <summary>
        /// First attempt a filtering all structures in a structure set list for only PTVs. Utilizes multiple layers of filters.
        /// </summary>
        /// <param name="structures">Generic collection of structures.</param>
        /// <returns>Returns a double that is the total volume of all found PTVs</returns>
        public static double TotalPTVVolume(this IEnumerable<Structure> structures)
        {
            //Add the string split for _ and check for double.
            int t = -1;
            return structures.Where(a => a.DicomType == "PTV" && !(
            a.Id.StartsWith("z") || a.Id.ToUpper().Contains("ALL") || a.Id.ToUpper().Contains("TOTAL") 
            || a.Id.ToUpper().Contains("3000") || a.Id.ToUpper().Contains("2500") || a.Id.ToUpper().Contains("2400")
            || a.Id.ToUpper().Contains("2200") || a.Id.ToUpper().Contains("2100") || a.Id.ToUpper().Contains("2000") 
            || a.Id.ToUpper().Contains("1800") || a.Id.StartsWith("_") || int.TryParse(a.Id.First().ToString(), out t)
            )).Sum(a => a.Volume);
        }

        /// <summary>
        /// Extension method of <c>PlanSetup</c> that pulls the volume of a structure at the Rx dose.
        /// </summary>
        /// <param name="ps">The extended PlanSetup</param>
        /// <param name="str">The structure requesting the volume for</param>
        /// <returns>Returns a double that is the volume of the structure receiving at least the Rx dose.</returns>
        public static double VolumeAtRxDose(this PlanSetup ps, Structure str)
        {
            return ps.GetVolumeAtDose(str, ps.TotalDose, VolumePresentation.AbsoluteCm3);
        }

        /// <summary>
        /// Extension method of <c>PlanSetup</c> that calculates the dose at a specific volume of a structure in absolute or relative dose and volume.
        /// </summary>
        /// <param name="ps">The extended PlanSetup</param>
        /// <param name="str">The structure requesting the dose at volume information from</param>
        /// <param name="vp">The volume presentation: absolute or relative</param>
        /// <param name="dp">The dose presentation: absolute or relative</param>
        /// <param name="PercentOfVolume">The percent of the volume to return the dose for. Values of 0-1.5 are valid (0-150%)</param>
        /// <returns>Returns a DoseValue at the specified volume of the specific structure.</returns>
        public static DoseValue DoseAtVolumeOfStructure(this PlanSetup ps, Structure str, VolumePresentation vp, DoseValuePresentation dp, double PercentOfVolume)
        {
            PercentOfVolume = (PercentOfVolume > 1.5) ? (PercentOfVolume / 100) : PercentOfVolume;
            return ps.GetDoseAtVolume(str, str.Volume * PercentOfVolume, vp, dp);
        }

        /// <summary>
        /// Extension method of <c>PlanSetup</c> that calculates the dose at 98% volume of a structure in absolute or relative dose and volume.
        /// </summary>
        /// <param name="ps">The extended PlanSetup</param>
        /// <param name="str">The structure requesting the dose at volume information from</param>
        /// <returns>Returns a DoseValue at the 98% of the volume of the specific structure.</returns>
        public static DoseValue DoseAt98Percent(this PlanSetup ps, Structure str)
        {
            return DoseAtVolumeOfStructure(ps, str, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute, 0.98);
        }

        /// <summary>
        /// Extension method of <c>PlanSetup</c> that calculates the dose at 50% volume of a structure in absolute or relative dose and volume.
        /// </summary>
        /// <param name="ps">The extended PlanSetup</param>
        /// <param name="str">The structure requesting the dose at volume information from</param>
        /// <returns>Returns a DoseValue at the 50% of the volume of the specific structure.</returns>
        public static DoseValue DoseAt50Percent(this PlanSetup ps, Structure str)
        {
            return DoseAtVolumeOfStructure(ps, str, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute, 0.5);
        }

        /// <summary>
        /// Extension method of <c>PlanSetup</c> that calculates the dose at 2% volume of a structure in absolute or relative dose and volume.
        /// </summary>
        /// <param name="ps">The extended PlanSetup</param>
        /// <param name="str">The structure requesting the dose at volume information from</param>
        /// <returns>Returns a DoseValue at the 2% of the volume of the specific structure.</returns>
        public static DoseValue DoseAt2Percent(this PlanSetup ps, Structure str)
        {
            return DoseAtVolumeOfStructure(ps, str, VolumePresentation.AbsoluteCm3, DoseValuePresentation.Absolute, 0.02);
        }

        /// <summary>
        /// This extension method of <c>PlanSetup</c> that calculates and returns the Paddick Conformity Index for a given structure. 
        /// </summary>
        /// <param name="ps">The extended PlanSetup</param>
        /// <param name="str">The structure requesting the calculation of the Paddick CI.</param>
        /// <param name="TotalPTVVolume">The Total PTV Volume of the plan. If null is passed, the total volume will be calculated by the TotalPTVVolume extension method.</param>
        /// <param name="decimals">Optional parameter for rounding purposes. Defaults to 2 decimal places if not passed.</param>
        /// <returns>Returns a double that is the Paddick Conformity Index to the specifed number of decimal places.</returns>
        public static double PaddickConformityIndex(this PlanSetup ps, Structure str, double? TotalPTVVolume, int decimals = 2)
        {
            TotalPTVVolume = (TotalPTVVolume == null) ? ps.StructureSet.Structures.TotalPTVVolume() : TotalPTVVolume;
            return (Math.Round(Math.Pow(VolumeAtRxDose(ps, str), 2) / ((double)TotalPTVVolume / ps.VolumeAtDose100(str)), decimals));
        }

        /// <summary>
        /// This extension method of <c>PlanSetup</c> that calculates and returns the Conformity Index for a given structure. 
        /// </summary>
        /// <param name="ps">The extended PlanSetup</param>
        /// <param name="str">The structure requesting the calculation of the CI for.</param>
        /// <param name="TotalPTVVolume">The Total PTV Volume of the plan. If null is passed, the total volume will be calculated by the TotalPTVVolume extension method.</param>
        /// <param name="decimals">Optional parameter for rounding purposes. Defaults to 2 decimal places if not passed.</param>
        /// <returns>Returns a double that is the Conformity Index to the specifed number of decimal places.</returns>
        public static double ConformityIndex(this PlanSetup ps, Structure str, double? TotalPTVVolume, int decimals = 2)
        {
            TotalPTVVolume = (TotalPTVVolume == null) ? ps.StructureSet.Structures.TotalPTVVolume() : TotalPTVVolume;
            return Math.Round(ps.VolumeAtDose100(str) / (double)TotalPTVVolume, decimals);
        }

        /// <summary>
        /// The Selectivity is calculated for a given structure by dividing the volume of the PTV receiving the Rx dose by the total volume of the 100% isodose cloud.
        /// </summary>
        /// <param name="ps">The extended PlanSetup</param>
        /// <param name="str">The structure requesting the calculation of the volume receiving Rx dose.</param>
        /// <param name="body">The structure to calculate the 100% isodose cloud.</param>
        /// <param name="decimals">Optional parameter for rounding purposes. Defaults to 2 decimal places if not passed.</param>
        /// <returns>Returns a double that is the Selectivity for the specified structure with precision specified by the decimals parameter.</returns>
        public static double Selectivity(this PlanSetup ps, Structure str, Structure body, int decimals = 2)
        {
            return Math.Round(ps.VolumeAtRxDose(str) / ps.VolumeAtDose100(body), decimals);
        }

        /// <summary>
        /// The Heterogeneity Index is calculated for a given structure by the formalism in ICRU 83.
        /// </summary>
        /// <param name="ps">The extended PlanSetup</param>
        /// <param name="str">The structure requesting the calculation of the HI.</param>
        /// <param name="decimals">Optional parameter for rounding purposes. Defaults to 2 decimal places if not passed.</param>
        /// <returns>Returns a double that is the Heterogeneity Indext for the structure with precision specified by the decimals parameter.</returns>
        public static double HeterogeneityIndex(this PlanSetup ps, Structure str, int decimals = 2)
        {
            return Math.Round((ps.DoseAt2Percent(str).Dose - ps.DoseAt98Percent(str).Dose) / ps.DoseAt50Percent(str).Dose, decimals);
        }

    }
}
