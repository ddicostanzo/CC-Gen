using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSU_Helpers
{
    /// <summary>
    /// OSU Helper Enumerables
    /// </summary>
    public static class Enumerables
    {
        /// <summary>
        /// Enumeration of dosimetric gradients for use with <c>SRT_Metrics</c>
        /// </summary>
        public enum Gradient
        {
            /// <summary>
            /// The Gradient for the 20% isodose line
            /// </summary>
            Gradient20,
            /// <summary>
            /// The Gradient for the 50% isodose line
            /// </summary>
            Gradient50,
            /// <summary>
            /// A user defined gradient
            /// </summary>
            UserDefined
        }

        /// <summary>
        /// MLC Bank enumeration
        /// </summary>
        public enum MLCBank
        {
            /// <summary>
            /// Bank A
            /// </summary>
            A = 1,
            /// <summary>
            /// Bank B
            /// </summary>
            B = 0
        }

        /// <summary>
        /// LeafSize enumeration for identifying small or large leaves
        /// </summary>
        public enum LeafSize
        {
            /// <summary>
            /// Small leaves are either 2.5mm in HD MLCs or 5.0mm in SD MLCs
            /// </summary>
            Small,
            /// <summary>
            /// Large leaves are either 5.0mm in HD MLCs or 10.0mm in SD MLCs
            /// </summary>
            Large
        }

        /// <summary>
        /// Identifies the MLC type associated with the machine
        /// </summary>
        public enum MLCType
        {
            /// <summary>
            /// HD MLC
            /// </summary>
            HD = 0,
            /// <summary>
            /// SD MLC
            /// </summary>
            SD = 1,
        }

        /// <summary>
        /// Plane directions for images
        /// </summary>
        public enum PlaneDirection
        {
            /// <summary>
            /// Axial direction: X-Y with static Z
            /// </summary>
            Axial,
            /// <summary>
            /// Coronal direction: X-Z with static Y
            /// </summary>
            Coronal,
            /// <summary>
            /// Sagittal direction: Y-Z with static X
            /// </summary>
            Sagittal,
        }

        /// <summary>
        /// Varian external beam machine types
        /// </summary>
        public enum MachineType
        {
            /// <summary>
            /// TrueBeam with HD MLC
            /// </summary>
            HD,
            /// <summary>
            /// TrueBeam with SD MLC
            /// </summary>
            SD,
            /// <summary>
            /// Halcyon machine
            /// </summary>
            Halcyon,
            /// <summary>
            /// ProBeam proton machine
            /// </summary>
            ProBeam
        }

        /// <summary>
        /// Axes enumerated in trajectory logs
        /// </summary>
        public enum AxisEnum
        {
            /// <summary>
            /// Collimator rotation
            /// </summary>
            Collimator_Rotation = 0,
            /// <summary>
            /// Gantry rotation
            /// </summary>
            Gantry_Rotation = 1,
            /// <summary>
            /// Y1 jaw
            /// </summary>
            Y1 = 2,
            /// <summary>
            /// Y2 jaw
            /// </summary>
            Y2 = 3,
            /// <summary>
            /// X1 jaw
            /// </summary>
            X1 = 4,
            /// <summary>
            /// X2 jaw
            /// </summary>
            X2 = 5,
            /// <summary>
            /// Couch vertical
            /// </summary>
            Couch_Vertical = 6,
            /// <summary>
            /// Couch longitudinal
            /// </summary>
            Couch_Longitudinal = 7,
            /// <summary>
            /// Couch lateral
            /// </summary>
            Couch_Lateral = 8,
            /// <summary>
            /// Couch rotation
            /// </summary>
            Couch_Rotation = 9,
            /// <summary>
            /// Couch pitch
            /// </summary>
            Couch_Pitch = 10,
            /// <summary>
            /// Couch roll
            /// </summary>
            Couch_Roll = 11,
            /// <summary>
            /// MU
            /// </summary>
            MU = 40,
            /// <summary>
            /// Beam hold enumeration
            /// </summary>
            Beam_Hold = 41,
            /// <summary>
            /// Control point
            /// </summary>
            Control_Point = 42,
            /// <summary>
            /// MLC enumeration, will contain all MLCs of log file
            /// </summary>
            MLC = 50,
            /// <summary>
            /// Target position
            /// </summary>
            TargetPosition = 60,
            /// <summary>
            /// Tracking target
            /// </summary>
            TrackingTarget = 61,
            /// <summary>
            /// Tracking base
            /// </summary>
            TrackingBase = 62,
            /// <summary>
            /// Tracking phase
            /// </summary>
            TrackingPhase = 63,
            /// <summary>
            /// Tracking conformity index
            /// </summary>
            TrackingConformityIndex = 64
        }

        /// <summary>
        /// Machine scale enumerated in trajectory logs
        /// </summary>
        public enum MachineScale
        {
            /// <summary>
            /// Machine scale
            /// </summary>
            Machine_Scale = 1,
            /// <summary>
            /// IEC Scale
            /// </summary>
            Modified_IEC_61217 = 2
        }
        
        /// <summary>
        /// MLC models enumerated in trajectory logs
        /// </summary>
        public enum MLCModel
        {
            /// <summary>
            /// New Delivery System 80-leaf
            /// </summary>
            NDS_80 = 0,
            /// <summary>
            /// New Delivery System 120-leaf
            /// </summary>
            NDS_120 = 2,
            /// <summary>
            /// New Delivery System 120-leaf HD
            /// </summary>
            NDS_120_HD = 3
        }

        /// <summary>
        /// Types of beam holds enumerated in trajectory logs
        /// </summary>
        public enum BeamHold
        {
            /// <summary>
            /// Beam hold normal
            /// </summary>
            Normal = 0,
            /// <summary>
            /// Beam hold frozen
            /// </summary>
            Freeze = 1,
            /// <summary>
            /// Beam is on hold 
            /// </summary>
            Hold = 2,
            /// <summary>
            /// Beam hold is disabled
            /// </summary>
            Diabled = 3
        }

        /// <summary>
        /// Energies enumerated in trajectory logs
        /// </summary>
        public enum Energy
        {
            /// <summary>
            /// 6X
            /// </summary>
            x06 = 0,
            /// <summary>
            /// 10X
            /// </summary>
            x10 = 1,
            /// <summary>
            /// 15X
            /// </summary>
            x15 = 2,
            /// <summary>
            /// 6FFF
            /// </summary>
            x06fff = 3,
            /// <summary>
            /// 10FFF
            /// </summary>
            x10fff = 4,
            /// <summary>
            /// 6E
            /// </summary>
            e06 = 5,
            /// <summary>
            /// 9E
            /// </summary>
            e09 = 6,
            /// <summary>
            /// 12E
            /// </summary>
            e12 = 7,
            /// <summary>
            /// 15E
            /// </summary>
            e15 = 8,
            /// <summary>
            /// 18E
            /// </summary>
            e18 = 9,
            /// <summary>
            /// 6E-HDTSE
            /// </summary>
            e06hdtse = 10,
            /// <summary>
            /// 9E-HDTSE
            /// </summary>
            e09hdtse = 11

        }

        /// <summary>
        /// Dose servo states enumerated in trajectory logs
        /// </summary>
        public enum DoseServo
        {
            /// <summary>
            /// Dose Servo Normal
            /// </summary>
            Normal = 0,
            /// <summary>
            /// Dose Servo Frozen
            /// </summary>
            Freeze = 1,
            /// <summary>
            /// Dose Servo on Hold
            /// </summary>
            Hold = 2,
            /// <summary>
            /// Dose Servo Disabled
            /// </summary>
            Disabled = 3
        }

        /// <summary>
        /// Trajectory log output file types enumeration
        /// </summary>
        public enum OutputType
        {
            /// <summary>
            /// Every snap shot from each axis
            /// </summary>
            AllData = 0,
            /// <summary>
            /// Max difference from each axis
            /// </summary>
            MaximumDifference = 1,
            /// <summary>
            /// Max difference along with the standard deviation of each axis
            /// </summary>
            MaximumAndStandardDeviation = 2,
            /// <summary>
            /// Average difference of each axis
            /// </summary>
            AverageDifference = 3,
            /// <summary>
            /// Average difference of each axis with the standard deviations of each axis
            /// </summary>
            AverageDifferenceAndStandardDeviation = 4,
            /// <summary>
            /// Average difference of each axis and the variance of each axis
            /// </summary>
            AverageDifferenceAndVariance = 5,
            /// <summary>
            /// Only the differences of each axis that were over the preset tolerance values
            /// </summary>
            DifferencesOverTolerance = 6,
            /// <summary>
            /// Every snap shot from each axis along with the calculated velocity based upon the sampling time
            /// </summary>
            AllDataWithVelocities = 7,
            /// <summary>
            /// Every snap shot from each axis along with the calculated velocity based upon the sampling time and the difference from ordered and actual position
            /// </summary>
            AllDataWithVelocitiesAndDifferences = 8,
        }

        /// <summary>
        /// Month enumerable
        /// </summary>
        public enum Month
        {
            /// <summary>
            /// January
            /// </summary>
            January = 1,
            /// <summary>
            /// February
            /// </summary>
            February = 2,
            /// <summary>
            /// March
            /// </summary>
            March = 3,
            /// <summary>
            /// April
            /// </summary>
            April = 4,
            /// <summary>
            /// May
            /// </summary>
            May = 5,
            /// <summary>
            /// June
            /// </summary>
            June = 6,
            /// <summary>
            /// July
            /// </summary>
            July = 7,
            /// <summary>
            /// August
            /// </summary>
            August = 8,
            /// <summary>
            /// September
            /// </summary>
            September = 9,
            /// <summary>
            /// October
            /// </summary>
            October = 10,
            /// <summary>
            /// November
            /// </summary>
            November = 11,
            /// <summary>
            /// December
            /// </summary>
            December = 12
        }

        /// <summary>
        /// Provides an enumerated anatomical direction
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Superior direction in HFS, positive Z
            /// </summary>
            PositiveZ = 5,
            /// <summary>
            /// Inferior direction in HFS, negative Z
            /// </summary>
            NegativeZ=2,
            /// <summary>
            /// Left direction in HFS, positive X 
            /// </summary>
            PositiveX=3,
            /// <summary>
            /// Right direction, negative X in HFS
            /// </summary>
            NegativeX=0,
            /// <summary>
            /// Anterior direction in HFS, positive Y 
            /// </summary>
            PositiveY=4,
            /// <summary>
            /// Posterior direction in HFS, negative Y 
            /// </summary>
            NegativeY=1
        }
    }
}
