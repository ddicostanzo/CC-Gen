using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.Types;
using static OSU_Helpers.Enumerables;

namespace OSU_Helpers
{
    /// <summary>
    /// MLC Leaf class that helps with calculations and manipulations of MLC leaves
    /// </summary>
    public class MLC_Leaf
    {
        private VRect<double> _jaw_settings { get; set; }
        /// <summary>
        /// MLC number in the bank: 1-60
        /// </summary>
        public int MLC_Number { get; set; }
        /// <summary>
        /// MLC index = MLC_Number - 1
        /// </summary>
        public int MLC_Index => MLC_Number - 1;
        /// <summary>
        /// MLC Bank enumerable for this MLC Leaf
        /// </summary>
        public MLCBank Bank { get; set; }
        /// <summary>
        /// Position of end of MLC leaf in mm
        /// </summary>
        public double Position_mm { get; set; }
        /// <summary>
        /// Position of end of MLC leaf in cm
        /// </summary>
        public double Position_cm => Position_mm / 10.0;
        /// <summary>
        /// Flag for MLC being completely blocked by jaw or not
        /// </summary>
        public bool Blocked_By_Jaw { get; private set; }
        /// <summary>
        /// MLC Parameters sub class
        /// </summary>
        public MLC_Parameters mlc_parameters { get; set; }
        /// <summary>
        /// Location of the bottom of the leaf in mm
        /// </summary>
        public double bottom_of_leaf_mm { get; set; }
        /// <summary>
        /// Location of the bottom of the leaf in cm
        /// </summary>
        public double bottom_of_leaf_cm => bottom_of_leaf_mm / 10;
        /// <summary>
        /// Location of the top of the leaf in mm
        /// </summary>
        public double top_of_leaf_mm { get; set; }
        /// <summary>
        /// Location of the top of the leaf in cm
        /// </summary>
        public double top_of_leaf_cm => top_of_leaf_mm / 10;
        /// <summary>
        /// Leaf size enumeration states whether large or small MLC
        /// </summary>
        public LeafSize leafSize { get; set; }
        /// <summary>
        /// Height of MLC leaf in mm
        /// </summary>
        private double leaf_height_mm => top_of_leaf_mm - bottom_of_leaf_mm;
        /// <summary>
        /// Height of MLC leaf in cm
        /// </summary>
        private double leaf_height_cm => leaf_height_mm / 10;
        /// <summary>
        /// Total length of leaf blocked by jaw in mm
        /// </summary>
        public double blocked_leaf_length_mm
        {
            get
            {
                if (Blocked_By_Jaw)
                {
                    return 150;
                }
                else
                {
                    if (Bank == MLCBank.B)
                    {
                        double result = Position_mm - _jaw_settings.X1;
                        return result;
                        //return (Position_mm - _jaw_settings.X1);
                    }
                    else
                    {
                        return (_jaw_settings.X2 - Position_mm);
                    }
                }
            }
        }
        /// <summary>
        /// Total area of leaf blocked by jaw in mm2
        /// </summary>
        public double blocked_leaf_area_mm
        {
            get
            {
                if (Blocked_By_Jaw)
                {
                    return 150;
                }
                else
                {
                    return (blocked_leaf_length_mm * Thickness_mm);
                }
            }
        }
        /// <summary>
        /// Total length of leaf blocked by jaw in cm
        /// </summary>
        public double blocked_leaf_length_cm => blocked_leaf_length_mm / 10;
        /// <summary>
        /// Thickness of MLC leaf in mm
        /// </summary>
        public double Thickness_mm => top_of_leaf_mm - bottom_of_leaf_mm;
        /// <summary>
        /// Thickness of MLC leaf in cm
        /// </summary>
        public double Thickness_cm => top_of_leaf_cm - bottom_of_leaf_cm;
        /// <summary>
        /// Area of blocked MLC leaf in mm
        /// </summary>
        public double Thickness_x_Blocked_Length_mm => blocked_leaf_length_mm * Thickness_mm;
        /// <summary>
        /// Area of blocked MLC leaf in cm
        /// </summary>
        public double Thickness_x_Blocked_Length_cm => Thickness_x_Blocked_Length_mm / 10;
        /// <summary>
        /// Used for navigating parent BeamExt of this MLC
        /// </summary>
        public BeamExt _parent_beam { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leaf_num"></param>
        /// <param name="bank"></param>
        /// <param name="position"></param>
        /// <param name="jaws"></param>
        /// <param name="mlc_id"></param>
        public MLC_Leaf(int leaf_num, int bank, double position, VRect<double> jaws, string mlc_id)
        {
            mlc_parameters = new MLC_Parameters(mlc_id);

            _jaw_settings = jaws;
            MLC_Number = leaf_num + 1;
            Bank = (MLCBank)bank;
            Position_mm = position;
            Set_LeafSize();
            Set_Top_and_Bottom();
            Blocked_By_Jaw = Is_Leaf_Behind_Jaw(jaws);
        }

        private void Set_Top_and_Bottom()
        {
            if (leafSize == LeafSize.Small)
            {
                bottom_of_leaf_mm = ((mlc_parameters.lower_mlc_limit + (mlc_parameters.OuterMLCWidth * mlc_parameters.int_large_leaves_above_below_iso)) + ((MLC_Index - mlc_parameters.int_large_leaves_above_below_iso) * mlc_parameters.InnerMLCWidth));

                top_of_leaf_mm = bottom_of_leaf_mm + mlc_parameters.InnerMLCWidth;
            }
            else
            {
                if (MLC_Number > mlc_parameters.int_large_leaves_above_below_iso)
                {
                    bottom_of_leaf_mm = mlc_parameters.upper_mlc_limit - ((60 - MLC_Index) * mlc_parameters.OuterMLCWidth);

                    top_of_leaf_mm = bottom_of_leaf_mm + mlc_parameters.OuterMLCWidth;
                }
                else
                {
                    bottom_of_leaf_mm = mlc_parameters.lower_mlc_limit + (MLC_Index * mlc_parameters.OuterMLCWidth);

                    top_of_leaf_mm = bottom_of_leaf_mm + mlc_parameters.OuterMLCWidth;
                }
            }
        }

        private void Set_LeafSize()
        {
            if (MLC_Number > mlc_parameters.int_large_leaves_above_below_iso && MLC_Index < (60 - mlc_parameters.int_large_leaves_above_below_iso))
            {
                leafSize = LeafSize.Small;
            }
            else
            {
                leafSize = LeafSize.Large;
            }
        }

        private bool Is_Leaf_Behind_Jaw(VRect<double> jaws)
        {
            if (top_of_leaf_mm > jaws.Y1 && bottom_of_leaf_mm < jaws.Y2)
            {
                //if (Position_mm > jaws.X1 && Position_mm < jaws.X2)
                //{
                //    return false;
                //}
                return false;
            }
            return true;
        }

        /// <summary>
        /// MLC_Parameter sub class to holds specific information regarding the MLCs of the TrueBeam
        /// </summary>
        public class MLC_Parameters
        {
            /// <summary>
            /// Constructor of the MLC parameters
            /// </summary>
            /// <param name="mlc">MLC ID in order to set parameters accurately for SD vs HD leaves</param>
            public MLC_Parameters(string mlc)
            {
                if (mlc.Contains("HD"))
                {
                    _MLC_Type = MLCType.HD;
                }
                else
                {
                    _MLC_Type = MLCType.SD;
                }
            }
            private MLCType _MLC_Type { get; set; }
            /// <summary>
            /// MLCType enumerable that states HD or SD leaves
            /// </summary>
            public MLCType MLC_Type => _MLC_Type;
            /// <summary>
            /// Width of outer MLCs in mm
            /// </summary>
            public double OuterMLCWidth
            {
                get
                {
                    if (_MLC_Type == MLCType.HD)
                    {
                        return 5;
                    }
                    else
                    {
                        return 10;
                    }
                }
            }
            /// <summary>
            /// Width of inner MLCs in mm
            /// </summary>
            public double InnerMLCWidth
            {
                get
                {
                    if (_MLC_Type == MLCType.HD)
                    {
                        return 2.5;
                    }
                    else
                    {
                        return 5.0;
                    }
                }
            }
            private const int number_of_mlcs_per_bank = 60;
            /// <summary>
            /// The number of large MLC leaves below isocenter
            /// </summary>
            public int int_large_leaves_above_below_iso
            {
                get
                {
                    if (_MLC_Type == MLCType.SD)
                    {
                        return 10;
                    }
                    else
                    {
                        return 14;
                    }
                }
            }
            /// <summary>
            /// The lower limit of field size covered by MLCs, -200mm for SD and -110mm for HD. Assumes IEC standard.
            /// </summary>
            public double lower_mlc_limit
            {
                get
                {
                    if (_MLC_Type == MLCType.HD)
                    {
                        return -110;
                    }
                    else
                    {
                        return -200;
                    }
                }
            }
            /// <summary>
            /// The upper limit of field size covered by MLCs, 200mm for SD and 110mm for HD. Assumes IEC standard.
            /// </summary>
            public double upper_mlc_limit => -lower_mlc_limit;

        }
    }
}
