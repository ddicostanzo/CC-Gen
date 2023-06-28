using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSU_Helpers
{
    /// <summary>
    /// Techniques used during Plan Checker to evaluate maximum MU
    /// </summary>
    public class TreatmentTechnique
    {
        /// <summary>
        /// The technique id
        /// </summary>
        public string TechniqueId { get; set; }
        /// <summary>
        /// Field type as static, imrt, arc, etc.
        /// </summary>
        public string FieldDeliveryType { get; set; }
        /// <summary>
        /// Energy of the technique
        /// </summary>
        public string FieldEnergy { get; set; }
        /// <summary>
        /// MU limit of technique
        /// </summary>
        public double MaxMU { get; set; }
    }


}
