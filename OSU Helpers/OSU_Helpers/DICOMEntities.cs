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

        /// <summary>
        /// Dev_Physics DICOM entity
        /// </summary>
        /// <returns>DICOMEntityConnection with Dev_Physics settings</returns>
        public static DICOMEntityConnections Dev_Physics()
        {
            DICOMEntityConnections de = new DICOMEntityConnections()
            {
                Port = 54000,
                IP = "10.65.160.105",
                AE_Title = "DCM_PHYSICS"
            };
            return de;
        }

        /// <summary>
        /// Dev_Script DICOM entity
        /// </summary>
        /// <returns>DICOMEntityConnection with Dev_Script settings</returns>
        public static DICOMEntityConnections Dev_Script()
        {
            DICOMEntityConnections de = new DICOMEntityConnections()
            {
                Port = 54000,
                IP = "10.216.14.135",
                AE_Title = "DCM_SCRIPT"
            };
            return de;
        }

        /// <summary>
        /// Production DICOM entity
        /// </summary>
        /// <returns>DICOMEntityConnection with Production settings</returns>
        public static DICOMEntityConnections Production()
        {
            DICOMEntityConnections de = new DICOMEntityConnections()
            {
                Port = 54000,
                IP = "10.95.25.9",
                AE_Title = "VMSDB_OSU"
            };
            return de;
        }

        /// <summary>
        /// Dev_Physics_FS DICOM entity
        /// </summary>
        /// <returns>DICOMEntityConnection with Dev_Physics File Service settings</returns>
        public static DICOMEntityConnections Dev_Physics_FS()
        {
            DICOMEntityConnections de = new DICOMEntityConnections()
            {
                Port = 104,
                IP = "10.65.160.105",
                AE_Title = "VMSFSD"
            };
            return de;
        }
    }
}
