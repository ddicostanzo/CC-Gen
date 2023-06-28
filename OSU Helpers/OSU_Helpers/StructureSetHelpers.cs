using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace OSU_Helpers
{
    /// <summary>
    /// Class to hold extension methods for ESAPI <c>StructureSet</c>
    /// </summary>
    public static class StructureSetHelpers
    {
        /// <summary>
        /// Extension for ESAPI StructureSet that returns bool type identifying if the Structure Set is null or empty
        /// </summary>
        /// <param name="ss">Extended Structure Set from ESAPI</param>
        /// <returns>Returns True if the structure set is Null or contains no structures.</returns>
        public static bool IsNullOrEmpty(this StructureSet ss)
        {
            if(ss == null || ss.Structures.ToList().Count == 0)
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// Returns the structure sets for the patient that meet the specifications included in the overloads
        /// </summary>
        /// <param name="patient">The patient extended to find the structure sets for</param>
        /// <param name="MinimumStructures">The minmum number of structures in the structure set in order to be returned.</param>
        /// <returns></returns>
        public static List<StructureSet> GetAllStructureSets(this Patient patient, int MinimumStructures = 0)
        {
            return patient.StructureSets.Where(a => a.Structures.Count() > MinimumStructures).ToList();

        }
    }
}
