using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace OSU_Helpers
{
    /// <summary>
    /// ESAPI <c>Beam</c> extension for OSU use. Allows for writing to and manipulating properties and adding methods.
    /// </summary>
    public class BeamExt
    {
        /// <summary>
        /// An applicator is a specific add-on used in the beam. 
        /// </summary>
        public Applicator Applicator { get; set; }
        /// <summary>
        /// The arc length. 
        /// </summary>
        public double ArcLength { get; set; }
        /// <summary>
        /// The average Source-to-Skin Distance (SSD) of an arc beam. 
        /// </summary>
        public double AverageSSD { get; set; }
        /// <summary>
        /// DICOM RT Beam Number. The value is unique within the plan in which it is created. 
        /// </summary>
        public int BeamNumber { get; set; }
        /// <summary>
        /// A collection of installed blocks. 
        /// </summary>
        public IEnumerable<Block> Blocks { get; set; }
        /// <summary>
        /// A collection of beam boluses. 
        /// </summary>
        public IEnumerable<Bolus> Boluses { get; set; }
        /// <summary>
        /// A collection of beam calculation logs. 
        /// </summary>
        public IEnumerable<BeamCalculationLog> CalculationLogs { get; set; }
        /// <summary>
        /// The compensator.  
        /// </summary>
        public Compensator Compensator { get; set; }
        /// <summary>
        /// An enumerable sequence of machine parameters that describe the planned treatment beam. 
        /// </summary>
        public ControlPointCollection ControlPoints { get; set; }
        /// <summary>
        /// The extension of the <c>ControlPointCollection</c> which allows navigation of all <c>ControlPointExt</c> of this beam.
        /// </summary>
        public List<ControlPointExt> ControlPointList { get; set; }
        /// <summary>
        /// The date when this object was created. 
        /// </summary>
        public DateTime? CreationDateTime { get; set; }
        /// <summary>
        /// The dose for the beam. Returns null if the dose is not calculated. (AcurosXB may not calculate <c>BeamDose</c> if the Plan Dose calculation is enabled).
        /// </summary>
        public BeamDose Dose { get; set; }
        /// <summary>
        /// The dose rate of the treatment machine in MU/min. 
        /// </summary>
        public int DoseRate { get; set; }
        /// <summary>
        /// The dosimetric leaf gap that has been configured for the Dynamic Multileaf Collimator (DMLC) beams in the system. The dosimetric leaf gap is used for accounting for dose transmission through rounded MLC leaves. 
        /// </summary>
        public double DosimetricLeafGap { get; set; }
        /// <summary>
        /// The display name of the energy mode. For example '18E' or '6X-SRS'. Use the EnergyMode helper class to manipulate this string.
        /// </summary>
        public string EnergyModeDisplayName { get; set; }
        /// <summary>
        /// A collection of field reference points. 
        /// </summary>
        public IEnumerable<FieldReferencePoint> FieldReferencePoints { get; set; }
        /// <summary>
        /// The gantry rotation direction: clockwise (CW), counterclockwise (CCW), or none. 
        /// </summary>
        public GantryDirection GantryDirection { get; set; }
        /// <summary>
        /// The identifier of the Beam. 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The position of the isocenter. 
        /// </summary>
        public VVector IsocenterPosition { get; set; }
        /// <summary>
        /// Checks if a beam is a setup field. 
        /// </summary>
        public bool IsSetupField { get; set; }
        /// <summary>
        /// The meterset value of the beam. 
        /// </summary>
        public MetersetValue Meterset { get; set; }
        /// <summary>
        /// The calculated meterset/Gy value for the beam. 
        /// </summary>
        public double MetersetPerGy { get; set; }
        /// <summary>
        /// Returns a hardware description of the Multileaf Collimator (MLC) used in an MLC plan, or null if no MLC exists.
        /// </summary>
        public MLC MLC { get; set; }
        /// <summary>
        /// The type of the Multileaf Collimator (MLC) plan. 
        /// </summary>
        public MLCPlanType MLCPlanType { get; set; }
        /// <summary>
        /// The transmission factor of the Multileaf Collimator (MLC) material. 
        /// </summary>
        public double MLCTransmissionFactor { get; set; }
        /// <summary>
        /// DICOM (respiratory) signal source. Returns an empty string if motion compensation technique is not used. 
        /// </summary>
        public string MotionSignalSource { get; set; } // new in v15
        /// <summary>
        /// DICOM (respiratory) motion compensation technique. Returns an empty string if motion compensation technique is not used. 
        /// </summary>
        public string MotionCompensationTechnique { get; set; } // new in v15
        /// <summary>
        /// The beam normalization factor. 
        /// </summary>
        public double NormalizationFactor { get; set; }
        /// <summary>
        /// The beam normalization method. 
        /// </summary>
        public string NormalizationMethod { get; set; }
        /// <summary>
        /// Used for navigating to parent Plan 
        /// </summary>
        public PlanSetup Plan { get; set; } // new in v15
        /// <summary>
        /// The Source-to-Skin Distance (SSD) value defined by the user. 
        /// </summary>
        public double PlannedSSD { get; set; }
        /// <summary>
        /// The reference image of the beam. 
        /// </summary>
        public Image ReferenceImage { get; set; }
        /// <summary>
        /// The reference image of the beam. 
        /// </summary>
        public SetupTechnique SetupTechnique { get; set; }
        /// <summary>
        /// The Source-to-Skin Distance (SSD). For arc beams, the SSD at the start angle. This value is calculated from the geometrical setup of the beam. 
        /// </summary>
        public double SSD { get; set; }
        /// <summary>
        /// The Source-to-Skin Distance (SSD) at the stop angle of an arc beam. This value is calculated from the geometrical setup of the beam. 
        /// </summary>
        public double SSDAtStopAngle { get; set; }
        /// <summary>
        /// The technique used in the planning beam. 
        /// </summary>
        public Technique Technique { get; set; }
        /// <summary>
        /// User-defined label for the referenced tolerance table, or an empty string if there is no reference to a tolerance table. 
        /// </summary>
        public string ToleranceTableLabel { get; set; }
        /// <summary>
        /// A collection of installed trays. 
        /// </summary>
        public IEnumerable<Tray> Trays { get; set; }
        /// <summary>
        /// The external beam treatment unit associated with this beam. 
        /// </summary>
        public ExternalBeamTreatmentUnit TreatmentUnit { get; set; }
        /// <summary>
        /// The treatment time set for the beam in seconds. Plan Approval wizard sets this value by default. 
        /// </summary>
        public double TreatmentTime { get; set; } //new in v15
        /// <summary>
        /// A collection of installed wedges. 
        /// </summary>
        public IEnumerable<Wedge> Wedges { get; set; }
        /// <summary>
        /// The weight factor of the beam. 
        /// </summary>
        public double WeightFactor { get; set; }
        /// <summary>
        /// Either X for x-rays or E for electrons
        /// </summary>
        public string beam_ene { get; set; }
        /// <summary>
        /// The treatment technique list that can be used to check against the maximum MU for this beam
        /// </summary>
        public List<TreatmentTechnique> treatment_techniques;
        /// <summary>
        /// string of the MLCPlanType enumerable
        /// </summary>
        public string mlc_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private gantry_quad[] quad_list;
        /// <summary>
        /// 
        /// </summary>
        private double gantrystopangle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private double gantryangle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private string arc_direction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private string beamid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private List<string> field_acronyms;
        /// <summary>
        /// The treatment orientation of the plane
        /// </summary>
        public PatientOrientation TreatmentOrientation { get; set; }
        /// <summary>
        /// The PlanSetupExt parent of this beam. Useful for navigating.
        /// </summary>
        public PlanSetupExt _parent_plan { get; set; }

        /// <summary>
        /// Constructor of the BeamExt class that needs an ESAPI <c>Beam</c> and <c>PlanSetupExt</c> to generate
        /// </summary>
        /// <param name="b">ESAPI <c>Beam</c> required for copying the data from.</param>
        /// <param name="_plan">Parent <c>PlanSetupExt</c> that will be stored in this class.</param>
        public BeamExt(Beam b, PlanSetupExt _plan)
        {
            //Setup_Photos = new List<Setup_Photo>();
            //rails_exist_in_setup_notes = false;
            _parent_plan = _plan;

            setup_field_acronyms();

            DateTime? CreationDateTime = new DateTime();
            VVector IsocenterPosition = new VVector();
            treatment_techniques = new List<TreatmentTechnique>();
            ControlPointList = new List<ControlPointExt>();

            TreatmentOrientation = _plan.TreatmentOrientation;
            Applicator = b.Applicator;
            ArcLength = b.ArcLength;
            AverageSSD = b.AverageSSD;
            BeamNumber = b.BeamNumber;
            Blocks = b.Blocks;
            Boluses = b.Boluses;
            CalculationLogs = b.CalculationLogs;
            Compensator = b.Compensator;
            ControlPoints = b.ControlPoints;
            CreationDateTime = b.CreationDateTime;
            Dose = b.Dose;
            DoseRate = b.DoseRate;
            DosimetricLeafGap = b.DosimetricLeafGap;
            EnergyModeDisplayName = b.EnergyModeDisplayName;
            FieldReferencePoints = b.FieldReferencePoints;
            GantryDirection = b.GantryDirection;
            Id = b.Id;
            IsocenterPosition = b.IsocenterPosition;
            IsSetupField = b.IsSetupField;
            Meterset = b.Meterset;
            MetersetPerGy = b.MetersetPerGy;
            MLC = b.MLC;
            MLCPlanType = b.MLCPlanType;
            MLCTransmissionFactor = b.MLCTransmissionFactor;
            MotionSignalSource = b.MotionSignalSource;
            MotionCompensationTechnique = b.MotionCompensationTechnique;
            NormalizationFactor = b.NormalizationFactor;
            NormalizationMethod = b.NormalizationMethod;
            Plan = b.Plan;
            PlannedSSD = b.PlannedSSD;
            ReferenceImage = b.ReferenceImage;
            SetupTechnique = b.SetupTechnique;
            SSD = b.SSD;
            SSDAtStopAngle = b.SSDAtStopAngle;
            Technique = b.Technique;
            ToleranceTableLabel = b.ToleranceTableLabel;
            Trays = b.Trays;
            TreatmentTime = b.TreatmentTime;
            TreatmentUnit = b.TreatmentUnit;
            Wedges = b.Wedges;
            WeightFactor = b.WeightFactor;

            setup_treatment_techniques();

            set_treatment_energy();

            //set arc direction to the current field so next time through we can check against next field
            if (GantryDirection.ToString() != "None")
            {
                arc_direction = GantryDirection.ToString();
            }

            //Replace underscores if that was used in the field naming
            if (Id.IndexOf("_") != -1) { beamid = Id.Replace('_', '-'); } else { beamid = Id; }

            set_mlc_type();

            setup_gantry_quadrants();

            if (b.ControlPoints.Count > 0)
            {
                foreach (ControlPoint c in b.ControlPoints.Select(a => a))
                {
                    ControlPointExt c1 = new ControlPointExt(c, this);
                    ControlPointList.Add(c1);
                }
            }
        }

        private void setup_gantry_quadrants()
        {
            switch (TreatmentOrientation.ToString())
            {
                case "HeadFirstProne":
                    quad_list = new gantry_quad[8]
                    {
                        new gantry_quad("RAO",90.1, 179.9),
                        new gantry_quad("RPO",0.1, 89.9),
                        new gantry_quad("LAO",180.1, 269.9),
                        new gantry_quad("LPO",270.1, 359.9),
                        new gantry_quad("LTLAT",270, 270),
                        new gantry_quad("RTLAT",90, 90),
                        new gantry_quad("PA",0, 0),
                        new gantry_quad("AP",180, 180)
                    };
                    break;
                case "HeadFirstSupine":
                    quad_list = new gantry_quad[8]
                    {
                        new gantry_quad("RAO",270.1, 359.9),
                        new gantry_quad("RPO",180.1, 269.9),
                        new gantry_quad("LAO",0.1, 89.9),
                        new gantry_quad("LPO",90.1, 179.9),
                        new gantry_quad("LTLAT",90, 90),
                        new gantry_quad("RTLAT",270, 270),
                        new gantry_quad("PA",180, 180),
                        new gantry_quad("AP",0, 0)
                    };

                    break;
                case "FeetFirstSupine":
                    quad_list = new gantry_quad[8]
                    {
                        new gantry_quad("RAO",0.1, 89.9),
                        new gantry_quad("RPO",90.1, 179.9),
                        new gantry_quad("LAO",270.1, 359.9),
                        new gantry_quad("LPO",180.1, 269.9),
                        new gantry_quad("LTLAT",270, 270),
                        new gantry_quad("RTLAT",90, 90),
                        new gantry_quad("PA",180, 180),
                        new gantry_quad("AP",0, 0)
                    };
                    break;
                case "FeetFirstProne":
                    quad_list = new gantry_quad[8]
                    {
                        new gantry_quad("RAO",180.1, 269.9),
                        new gantry_quad("RPO",270.1, 359.9),
                        new gantry_quad("LAO",90.1, 179.9),
                        new gantry_quad("LPO",0.1, 89.9),
                        new gantry_quad("LTLAT",90, 90),
                        new gantry_quad("RTLAT",270, 270),
                        new gantry_quad("PA",0, 0),
                        new gantry_quad("AP",180, 180)
                    };
                    break;
                default:
                    quad_list = new gantry_quad[8]
                    {
                        new gantry_quad("RAO",270.1, 359.9),
                        new gantry_quad("RPO",180.1, 269.9),
                        new gantry_quad("LAO",0.1, 89.9),
                        new gantry_quad("LPO",90.1, 179.9),
                        new gantry_quad("LTLAT",90, 90),
                        new gantry_quad("RTLAT",270, 270),
                        new gantry_quad("PA",180, 180),
                        new gantry_quad("AP",0, 0)
                    };
                    //header.Items.Add(new TreeViewItem()
                    //{
                    //    Header = "Oritentation is different than programmed. Something is wrong. Please contact Dominic.",
                    //    FontStyle = FontStyles.Italic,
                    //    FontWeight = FontWeights.Bold
                    //});
                    break;
            }
        }

        private void set_treatment_energy()
        {
            if (EnergyModeDisplayName.Contains("X"))
            {
                beam_ene = "X";
            }
            else
            {
                beam_ene = "E";
            }
        }

        private void setup_treatment_techniques()
        {
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "STATIC", FieldDeliveryType = "Conformal", FieldEnergy = "X", MaxMU = 500 });
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "STATIC", FieldDeliveryType = "IMRT", FieldEnergy = "X", MaxMU = 999 });
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "ARC", FieldDeliveryType = "VMAT", FieldEnergy = "X", MaxMU = 1500 });
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "ARC", FieldDeliveryType = "Conformal", FieldEnergy = "X", MaxMU = 500 });
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "SRS STATIC", FieldDeliveryType = "IMRT", FieldEnergy = "X", MaxMU = 6000 });
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "SRS STATIC", FieldDeliveryType = "Conformal", FieldEnergy = "X", MaxMU = 6000 });
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "SRS ARC", FieldDeliveryType = "VMAT", FieldEnergy = "X", MaxMU = 6000 });
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "SRS ARC", FieldDeliveryType = "Conformal", FieldEnergy = "X", MaxMU = 6000 });
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "Total", FieldDeliveryType = "Conformal", FieldEnergy = "X", MaxMU = 6000 });
            treatment_techniques.Add(new TreatmentTechnique() { TechniqueId = "STATIC", FieldDeliveryType = "Conformal", FieldEnergy = "E", MaxMU = 999 });
        }

        private void set_mlc_type()
        {
            switch (MLCPlanType.ToString())
            {
                case "Static":
                    mlc_type = "Conformal";
                    break;
                case "DoseDynamic":
                    mlc_type = "IMRT";
                    break;
                case "ArcDynamic":
                    mlc_type = "Dynamic Conformal Arc";
                    break;
                case "VMAT":
                    mlc_type = "VMAT";
                    break;
                case "ProtonLayerStacking":
                    mlc_type = "Proton - What are you doing?";
                    break;
                default:
                    //jaws no mlcs
                    mlc_type = "Conformal";
                    break;
            }
        }

        private void setup_field_acronyms()
        {
            field_acronyms = new List<string>();
            field_acronyms.Add("RAO");
            field_acronyms.Add("RPO");
            field_acronyms.Add("LAO");
            field_acronyms.Add("LPO");
            field_acronyms.Add("AP");
            field_acronyms.Add("PA");
            field_acronyms.Add("RTLAT");
            field_acronyms.Add("RLAT");
            field_acronyms.Add("LTLAT");
            field_acronyms.Add("LLAT");
        }

        // Collection of gantry_quadrant objects. This class 
        // implements IEnumerable so that it can be used 
        // with ForEach syntax. 
        internal class gantry_quads : IEnumerable
        {
            private gantry_quad[] _quads;
            public gantry_quads(gantry_quad[] pArray)
            {
                _quads = new gantry_quad[pArray.Length];

                for (int i = 0; i < pArray.Length; i++)
                {
                    _quads[i] = pArray[i];
                }
            }

            // Implementation for the GetEnumerator method.
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public gantryenum GetEnumerator()
            {
                return new gantryenum(_quads);
            }
        }

        // When you implement IEnumerable, you must also implement IEnumerator. 
        internal class gantryenum : IEnumerator
        {
            public gantry_quad[] _quads;

            // Enumerators are positioned before the first element 
            // until the first MoveNext() call. 
            private int position = -1;

            public gantryenum(gantry_quad[] list)
            {
                _quads = list;
            }

            public bool MoveNext()
            {
                position++;
                return (position < _quads.Length);
            }

            public void Reset()
            {
                position = -1;
            }

            object IEnumerator.Current => Current;

            public gantry_quad Current
            {
                get
                {
                    try
                    {
                        return _quads[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        //Gantry quad class to set the start and end of each quadrant
        internal class gantry_quad
        {

            public string quad { get; set; }
            public double start { get; set; }
            public double end { get; set; }

            public gantry_quad(string newName, double new_start, double new_end)
            {
                quad = newName;
                start = new_start;
                end = new_end;
            }

            public void set_s_and_e(double s, double e)
            {
                start = s;
                end = e;

            }

            public void set_quad_name(string name)
            {
                quad = name;

            }

            public void set_start_angle(double s)
            {
                start = s;
            }

            public void set_end_angle(double e)
            {
                end = e;
            }

            public bool test_angles(double angle)
            {
                if (angle > start && angle < end)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

    }
}
