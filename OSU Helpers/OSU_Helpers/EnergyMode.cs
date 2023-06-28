using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSU_Helpers
{
    /// <summary>
    /// The EnergyModeDisplayName string manipulation helpers
    /// </summary>
    public static class EnergyMode
    {
        /// <summary>
        /// Returns the energy only of the specified energy will truncate FFF or SRS from the energy mode
        /// </summary>
        /// <param name="b">Input energy mode display name from a beam</param>
        /// <returns>String that is only the energy of the input energy mode</returns>
        public static string EnergyModeDisplay(string b)
        {
            if (b.Contains("-") || b.Contains("FFF"))
            {
                string[] sp = b.Split('-');
                return sp[0];
            }
            else
            {
                return b;
            }
        }

        /// <summary>
        /// Returns the fluence mode of the input energy mode, or an empty string if none is present
        /// </summary>
        /// <param name="b">Input energy mode display name from a beam</param>
        /// <returns>Returns fluence mode for input energy mode display name or empty string</returns>
        public static string FluenceMode(string b)
        {
            if (b.Contains("-") || b.Contains("FFF"))
            {
                string[] sp = b.Split('-');
                return sp[1];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
