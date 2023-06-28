using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace OSU_Helpers
{
    /// <summary>
    /// Contol point extension for Varian provided ControlPoint class in ESAPI
    /// </summary>
    public class ControlPointExt
    {
        /// <summary>
        /// Collimator angle of control point
        /// </summary>
        public double CollimatorAngle { get; set; }
        /// <summary>
        /// Gantry angle of control point
        /// </summary>
        public double GantryAngle { get; set; }
        /// <summary>
        /// Jaw positions as a VRect[double] where the order is X1, X2, Y1, Y2
        /// </summary>
        public VRect<double> JawPositions { get; set; }
        /// <summary>
        /// The positions of the beam collimator leaf pairs (in mm) in the IEC BEAMLIMITING DEVICE coordinate axis appropriate to the device type. For example, the X-axis for MLCX and the Y-axis for MLCY. The two-dimensional array is indexed [bank, leaf] where the bank is either 0 or 1. Bank 0 represents the leaf bank to the negative MLC X direction, and bank 1 to the positive MLC X direction. If there is no MLC, a (0,0)-length array is returned. 
        /// </summary>
        public float[,] LeafPositions { get; set; }
        /// <summary>
        /// The cumulative meterset weight to this control point. The cumulative meterset weight for the first item in a control point sequence is zero. 
        /// </summary>
        public double MetersetWeight { get; set; }
        /// <summary>
        /// The patient support angle. In other words, the orientation of the IEC PATIENT SUPPORT (turntable) coordinate system with respect to the IEC FIXED REFERENCE coordinate system (in degrees). 
        /// </summary>
        public double PatientSupportAngle { get; set; }
        /// <summary>
        /// Table top lateral position in millimeters, in the IEC TABLE TOP coordinate system. 
        /// </summary>
        public double TableTopLateralPosition { get; set; }
        /// <summary>
        /// Table top longitudinal position in millimeters, in the IEC TABLE TOP coordinate system. 
        /// </summary>
        public double TableTopLongitudinalPosition { get; set; }
        /// <summary>
        /// Table top vertical position in millimeters, in the IEC TABLE TOP coordinate system. 
        /// </summary>
        public double TableTopVerticalPosition { get; set; }
        /// <summary>
        /// A list of each MLC leaf that utilizes the helper MLC class
        /// </summary>
        public List<MLC_Leaf> MLCs { get; set; } = new List<MLC_Leaf>();
        /// <summary>
        /// All MLCS that are not completely blocked by jaws in order to expedite calculations
        /// </summary>
        public List<MLC_Leaf> MLCs_Not_Blocked_By_Jaw => MLCs.Where(a => !a.Blocked_By_Jaw).ToList();
        /// <summary>
        /// The parent BeamExt for navigation purposes
        /// </summary>
        public BeamExt _parent_beam { get; set; }
        /// <summary>
        /// The parent ESAPI <c>ControlPoint</c> for navigation purposes
        /// </summary>
        public ControlPoint _parent_cp { get; set; }

        /// <summary>
        /// The control point extension constructor
        /// </summary>
        /// <param name="c">The parent ESAPI <c>ControlPoint</c></param>
        /// <param name="beam">The parent <c>BeamExt</c></param>
        public ControlPointExt(ControlPoint c, BeamExt beam)
        {
            _parent_cp = c;
            _parent_beam = beam;
            JawPositions = new VRect<double>();

            CollimatorAngle = c.CollimatorAngle;
            GantryAngle = c.GantryAngle;
            JawPositions = c.JawPositions;
            LeafPositions = c.LeafPositions;
            MetersetWeight = c.MetersetWeight;
            PatientSupportAngle = c.PatientSupportAngle;
            TableTopLateralPosition = c.TableTopLateralPosition;
            TableTopLongitudinalPosition = c.TableTopLongitudinalPosition;
            TableTopVerticalPosition = c.TableTopVerticalPosition;

            string mlc_id = (beam.MLC == null) ? string.Empty : beam.MLC.Id;

            if (mlc_id != string.Empty)
            {
                for (int i = 0; i < LeafPositions.Length / 2; i++)
                {
                    MLCs.Add(new MLC_Leaf(i, 0, LeafPositions[0, i], JawPositions, mlc_id));
                    MLCs.Add(new MLC_Leaf(i, 1, LeafPositions[1, i], JawPositions, mlc_id));
                }
            }
        }
    }


}
