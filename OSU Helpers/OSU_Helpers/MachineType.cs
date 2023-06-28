using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using static OSU_Helpers.Enumerables;

namespace OSU_Helpers
{
    /// <summary>
    /// Machine Type enumeration assignment based upon passed beam
    /// </summary>
    public static class MachineTypes
    {
        /// <summary>
        /// Returns the machine type of specified beam
        /// </summary>
        /// <param name="b">Input beam for selection of machine</param>
        /// <returns></returns>
        public static MachineType MachineTypeFromBeam(Beam b)
        {
            if (b.TreatmentUnit.Id.Contains("HD"))
            {
                return MachineType.HD;
            }
            else
            {
                return MachineType.SD;
            }
        }

    }
}
