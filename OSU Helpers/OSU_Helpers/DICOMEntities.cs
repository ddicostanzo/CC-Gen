using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSU_Helpers
{
    /// <summary>
    /// DICOM entity connections for use with EvilDICOM transactions
    /// </summary>
    public class DICOMEntityConnections
    {
        /// <summary>
        /// Port to connect
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// AE title of the entity
        /// </summary>
        public string AE_Title { get; set; }
        /// <summary>
        /// IP of the host
        /// </summary>
        public string IP { get; set; }

      
    }
}
