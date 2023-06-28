using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace OSU_Helpers
{
    /// <summary>
    /// PlanSetup Extension class. This class is a replication of the ESAPI <c>PlanSetup</c> class to all for data manipulation and method extension. Note not all properties have been duplicated.
    /// </summary>
    public class PlanSetupExt
    {
        /// <summary>
        ///  The calculated fraction dose in the primary reference point.
        /// </summary>
        public DoseValue PlannedDosePerFraction { get; set; }
        /// <summary>
        ///     The display name of the user who approved the plan for planning.
        /// </summary>
        public string PlanningApproverDisplayName { get; set; }
        /// <summary>
        /// The prior revision of the plan
        /// </summary>
        public PlanSetup PredecessorPlan { get; set; }
        /// <summary>
        /// The number of fractions.
        /// </summary>
        public int? NumberOfFractions { get; set; }
        /// <summary>
        /// Collection of reference points in the plan.
        /// </summary>
        public IEnumerable<ReferencePoint> ReferencePoints { get; set; }
        /// <summary>
        /// Used for navigating to the linked prescription.
        /// </summary>
        public RTPrescription RTPrescription { get; set; }
        /// <summary>
        /// Planned total dose.
        /// </summary>
        public DoseValue TotalDose { get; set; }
        /// <summary>
        ///  The display name of the user who approved the plan for treatment.
        /// </summary>
        public string TreatmentApproverDisplayName { get; set; }
        /// <summary>
        /// The treatment percentage as a decimal number. For example, if the treatment percentage shown in the Eclipse user interface is 80%, returns 0.8.
        /// </summary>
        public double TreatmentPercentage { get; set; }
        /// <summary>
        /// Treatment sessions for the plan, either scheduled sessions or treated sessions.
        /// </summary>
        public IEnumerable<PlanTreatmentSession> TreatmentSessions { get; set; }
        /// <summary>
        /// Boolean to mark if gating is used in the plan.
        /// </summary>
        public bool UseGating { get; set; }
        /// <summary>
        /// Plan uncertainties defined for the plan.
        /// </summary>
        public IEnumerable<PlanUncertainty> PlanUncertainties { get; set; }
        /// <summary>
        ///     The list of structure IDs that are present in the plan objectives (prescriptions and indices).
        /// </summary>
        public IEnumerable<string> PlanObjectiveStructures { get; set; }
        /// <summary>
        /// Returns the approval history of the plan setup.
        /// </summary>
        public IEnumerable<ApprovalHistoryEntry> ApprovalHistory { get; set; }
        /// <summary>
        /// The log entries of the script executions that have modified the plan.
        /// </summary>
        public IEnumerable<ApplicationScriptLog> ApplicationScriptLogs { get; set; }
        /// <summary>
        /// The dose per fraction. 
        /// </summary>
        public DoseValue DosePerFraction { get; set; }
        /// <summary>
        /// Returns the approval history of the plan setup. 
        /// </summary>
        public PlanSetupApprovalStatus ApprovalStatus { get; set; }
        /// <summary>
        /// A collection of all the beams in the plan (including setup beams). Returns an empty collection if not applicable for the plan, for example, if the plan is a brachytherapy plan. 
        /// </summary>
        public IEnumerable<Beam> Beams { get; set; }
        /// <summary>
        /// A collection of the <c>BeamExt</c> class which contains the extension ESAPI <c>Beam</c> for each field in this plan 
        /// </summary>
        public List<BeamExt> BeamList { get; set; }
        /// <summary>
        /// Used for navigating to parent course. 
        /// </summary>
        public Course Course { get; set; }
        /// <summary>
        /// The name of the user who saved the plan for the first time. 
        /// </summary>
        public string CreationUserName { get; set; }
        /// <summary>
        /// The total dose. The total dose is the dose of all planned fractions together. 
        /// </summary>
        public PlanningItemDose Dose { get; set; }
        /// <summary>
        /// Returns a list of DVH estimate objects for this plan 
        /// </summary>
        public IEnumerable<EstimatedDVH> DVHEstimates { get; set; }
        /// <summary>
        /// The name of the electron calculation model. Not applicable to brachytherapy plans. 
        /// </summary>
        public string ElectronCalculationModel { get; set; }
        /// <summary>
        /// The electron calculation options. Not applicable to brachytherapy plans. 
        /// </summary>
        public Dictionary<string, string> ElectronCalculationOptions { get; set; }
        /// <summary>
        /// The identifier of the PlanSetup. 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Returns the value true if the plan dose is valid. This implies that the dose object returned from the dose property is not null and can therefore be used to query dose values. 
        /// </summary>
        public bool IsDoseValid { get; set; }
        /// <summary>
        /// Checks if the treatment plan has been delivered. 
        /// </summary>
        public bool IsTreated { get; set; }
        /// <summary>
        /// Provides access to optimization objectives and parameters.
        /// </summary>
        public OptimizationSetup OptimizationSetup { get; set; }
        /// <summary>
        /// The name of the proton calculation model. Not applicable to brachytherapy plans. 
        /// </summary>
        public string PhotonCalculationModel { get; set; }
        /// <summary>
        /// The proton calculation options. Not applicable to brachytherapy plans. 
        /// </summary>
        public Dictionary<string, string> PhotonCalculationOptions { get; set; }
        /// <summary>
        /// The plan intent as in DICOM, or an empty string. The defined terms are "CURATIVE", "PALLIATIVE", "PROPHYLACTIC", "VERIFICATION", "MACHINE_QA", "RESEARCH" and "SERVICE", but the value can be different for imported plans. 
        /// </summary>
        public string PlanIntent { get; set; }
        /// <summary>
        /// The date when the plan was approved for planning. 
        /// </summary>
        public string PlanningApprovalDate { get; set; }
        /// <summary>
        /// The identifier of the user who approved the plan for planning.
        /// </summary>
        public string PlanningApprover { get; set; }
        /// <summary>
        /// The user interface name for the current normalization method. 
        /// </summary>
        public string PlanNormalizationMethod { get; set; }
        /// <summary>
        /// The plan normalization point. 
        /// </summary>
        public VVector PlanNormalizationPoint { get; set; }
        /// <summary>
        /// The plan normalization value in percentage. The plan is normalized according to the plan normalization value, for instance, 200%. The value is Double.NaN if it is not defined.
        /// Remarks: The property setter changes also the plan normalization method to Plan Normalization Value.
        /// </summary>
        public double PlanNormalizationValue { get; set; }
        /// <summary>
        /// The plan type. 
        /// </summary>
        public PlanType PlanType { get; set; }
        /// <summary>
        /// The primary reference point. 
        /// </summary>
        public ReferencePoint PrimaryReferencePoint { get; set; }
        /// <summary>
        /// The protocol identifier. 
        /// </summary>
        public string ProtocolID { get; set; }
        /// <summary>
        /// The protocol phase identifier. 
        /// </summary>
        public string ProtocolPhaseID { get; set; }
        /// <summary>
        /// The name of the proton calculation model. Not applicable to brachytherapy plans.
        /// </summary>
        public string ProtonCalculationModel { get; set; }
        /// <summary>
        /// The proton calculation options. Not applicable to brachytherapy plans. 
        /// </summary>
        public Dictionary<string, string> ProtonCalculationOptions { get; set; }
        /// <summary>
        /// The series that contains this plan. Null if the plan is not connected to a series. 
        /// </summary>
        public Series Series { get; set; }
        /// <summary>
        /// The DICOM UID of the series that contains this plan. Empty string if the plan is not connected to a series. 
        /// </summary>
        public string SeriesUID { get; set; }
        /// <summary>
        /// The structure set. 
        /// </summary>
        public StructureSet StructureSet { get; set; }
        /// <summary>
        /// The target volume identifier. 
        /// </summary>
        public string TargetVolumeID { get; set; }
        /// <summary>
        /// The date when the plan was approved for treatment. 
        /// </summary>
        public string TreatmentApprovalDate { get; set; }
        /// <summary>
        /// The identifier of the user who approved the plan for treatment. 
        /// </summary>
        public string TreatmentApprover { get; set; }
        /// <summary>
        /// The orientation of the treatment. 
        /// </summary>
        public PatientOrientation TreatmentOrientation { get; set; }
        /// <summary>
        /// The DICOM UID of the plan. 
        /// </summary>
        public string UID { get; set; }
        /// <summary>
        /// Returns the verified plan if this is a verification plan, otherwise returns null. The verified plan is the clinical plan that was used to create the verification plan. 
        /// </summary>
        public PlanSetup VerifiedPlan { get; set; }
        /// <summary>
        /// The parent <c>CourseExt</c> associated to this <c>PlanSetupExt</c>
        /// </summary>
        public CourseExt _parent_course { get; set; }

        /// <summary>
        /// Constructor for <c>PlanSetupExt</c> which takes the original <c>PlanSetup</c> and <c>CourseExt</c>
        /// </summary>
        /// <param name="p">Input ESAPI <c>PlanSetup</c></param>
        /// <param name="_c"><c>CourseExt</c> of the <c>PlanSetup</c></param>
        public PlanSetupExt(PlanSetup p, CourseExt _c)
        {
            //MessageBox.Show("Starting PS Ext creation");
            _parent_course = _c;
            PlanNormalizationPoint = new VVector();
            BeamList = new List<BeamExt>();

            ApplicationScriptLogs = p.ApplicationScriptLogs;
            ApprovalStatus = p.ApprovalStatus;
            ApprovalHistory = p.ApprovalHistory;
            Beams = p.Beams;
            Course = p.Course;
            CreationUserName = p.CreationUserName;
            Dose = p.Dose;
            DosePerFraction = p.DosePerFraction;
            DVHEstimates = p.DVHEstimates;
            ElectronCalculationModel = p.ElectronCalculationModel;
            ElectronCalculationOptions = p.ElectronCalculationOptions;
            Id = p.Id;
            IsDoseValid = p.IsDoseValid;
            IsTreated = p.IsTreated;
            NumberOfFractions = p.NumberOfFractions;
            OptimizationSetup = p.OptimizationSetup;
            PhotonCalculationModel = p.PhotonCalculationModel;
            PhotonCalculationOptions = p.PhotonCalculationOptions;
            PlanIntent = p.PlanIntent;
            PlanObjectiveStructures = p.PlanObjectiveStructures;
            PlannedDosePerFraction = p.PlannedDosePerFraction;
            PlanningApprovalDate = p.PlanningApprovalDate;
            PlanningApprover = p.PlanningApprover;
            PlanningApproverDisplayName = p.PlanningApproverDisplayName;
            PlanNormalizationMethod = p.PlanNormalizationMethod;
            PlanNormalizationPoint = p.PlanNormalizationPoint;
            PlanNormalizationValue = p.PlanNormalizationValue;
            PlanUncertainties = p.PlanUncertainties;
            PlanType = p.PlanType;
            PredecessorPlan = p.PredecessorPlan;
            PrimaryReferencePoint = p.PrimaryReferencePoint;
            ProtocolID = p.ProtocolID;
            ProtocolPhaseID = p.ProtocolPhaseID;
            ProtonCalculationModel = p.ProtonCalculationModel;
            ProtonCalculationOptions = p.ProtonCalculationOptions;
            ReferencePoints = p.ReferencePoints;
            RTPrescription = p.RTPrescription;
            Series = p.Series;
            SeriesUID = p.SeriesUID;
            StructureSet = p.StructureSet;
            TargetVolumeID = p.TargetVolumeID;
            TotalDose = p.TotalDose;
            TreatmentApprovalDate = p.TreatmentApprovalDate;
            TreatmentApprover = p.TreatmentApprover;
            TreatmentApproverDisplayName = p.TreatmentApproverDisplayName;
            TreatmentPercentage = p.TreatmentPercentage;
            TreatmentOrientation = p.TreatmentOrientation;
            TreatmentSessions = p.TreatmentSessions;
            UseGating = p.UseGating;
            UID = p.UID;
            VerifiedPlan = p.VerifiedPlan;

            IEnumerable<Beam> b_sorted = p.Beams.OrderBy(a => a.BeamNumber).Select(a => a);
            foreach (Beam b in b_sorted)
            {
                BeamExt b1 = new BeamExt(b, this);
                BeamList.Add(b1);
            }
        }

        /// <summary>
        /// Constructor for <c>PlanSetupExt</c> which takes the original <c>PlanSetup</c> only. Useful if only working with single plan
        /// Generates list of <c>BeamExt</c> for all fields in plan
        /// </summary>
        /// <param name="p">Input ESAPI <c>PlanSetup</c></param>
        public PlanSetupExt(PlanSetup p)
        {
            //MessageBox.Show("Starting PS Ext creation");
            PlanNormalizationPoint = new VVector();
            BeamList = new List<BeamExt>();

            ApplicationScriptLogs = p.ApplicationScriptLogs;
            ApprovalStatus = p.ApprovalStatus;
            ApprovalHistory = p.ApprovalHistory;
            Beams = p.Beams;
            Course = p.Course;
            CreationUserName = p.CreationUserName;
            Dose = p.Dose;
            DosePerFraction = p.DosePerFraction;
            DVHEstimates = p.DVHEstimates;
            ElectronCalculationModel = p.ElectronCalculationModel;
            ElectronCalculationOptions = p.ElectronCalculationOptions;
            Id = p.Id;
            IsDoseValid = p.IsDoseValid;
            IsTreated = p.IsTreated;
            NumberOfFractions = p.NumberOfFractions;
            OptimizationSetup = p.OptimizationSetup;
            PhotonCalculationModel = p.PhotonCalculationModel;
            PhotonCalculationOptions = p.PhotonCalculationOptions;
            PlanIntent = p.PlanIntent;
            PlanObjectiveStructures = p.PlanObjectiveStructures;
            PlannedDosePerFraction = p.PlannedDosePerFraction;
            PlanningApprovalDate = p.PlanningApprovalDate;
            PlanningApprover = p.PlanningApprover;
            PlanningApproverDisplayName = p.PlanningApproverDisplayName;
            PlanNormalizationMethod = p.PlanNormalizationMethod;
            PlanNormalizationPoint = p.PlanNormalizationPoint;
            PlanNormalizationValue = p.PlanNormalizationValue;
            PlanUncertainties = p.PlanUncertainties;
            PlanType = p.PlanType;
            PredecessorPlan = p.PredecessorPlan;
            PrimaryReferencePoint = p.PrimaryReferencePoint;
            ProtocolID = p.ProtocolID;
            ProtocolPhaseID = p.ProtocolPhaseID;
            ProtonCalculationModel = p.ProtonCalculationModel;
            ProtonCalculationOptions = p.ProtonCalculationOptions;
            ReferencePoints = p.ReferencePoints;
            RTPrescription = p.RTPrescription;
            Series = p.Series;
            SeriesUID = p.SeriesUID;
            StructureSet = p.StructureSet;
            TargetVolumeID = p.TargetVolumeID;
            TotalDose = p.TotalDose;
            TreatmentApprovalDate = p.TreatmentApprovalDate;
            TreatmentApprover = p.TreatmentApprover;
            TreatmentApproverDisplayName = p.TreatmentApproverDisplayName;
            TreatmentPercentage = p.TreatmentPercentage;
            TreatmentOrientation = p.TreatmentOrientation;
            TreatmentSessions = p.TreatmentSessions;
            UseGating = p.UseGating;
            UID = p.UID;
            VerifiedPlan = p.VerifiedPlan;

            IEnumerable<Beam> b_sorted = p.Beams.OrderBy(a => a.BeamNumber).Select(a => a);
            foreach (Beam b in b_sorted)
            {
                BeamExt b1 = new BeamExt(b, this);
                BeamList.Add(b1);
            }
        }


    }
}
