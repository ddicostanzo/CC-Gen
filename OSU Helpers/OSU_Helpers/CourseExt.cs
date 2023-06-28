using System;
using System.Collections.Generic;
using VMS.TPS.Common.Model.API;

namespace OSU_Helpers
{
    /// <summary>
    /// The extension of the ESAPI <c>Course</c> class that allows for data manipulation
    /// </summary>
    public class CourseExt
    {
        /// <summary>
        /// A collection of proton plans for the course. 
        /// </summary>
        public IEnumerable<IonPlanSetup> IonPlanSetups { get; }
        /// <summary>
        /// All treatment phases in the course. 
        /// </summary>
        public IEnumerable<TreatmentPhase> TreatmentPhases { get; }
        /// <summary>
        /// Treatment sessions of the course. 
        /// </summary>
        public IEnumerable<TreatmentSession> TreatmentSessions { get; }
        /// <summary>
        /// A collection of brachytherapy plans for the course. 
        /// </summary>
        public IEnumerable<BrachyPlanSetup> BrachyPlanSetups { get; set; }
        /// <summary>
        /// The date and time when the course was completed. 
        /// </summary>
        public DateTime? CompletedDateTime { get; set; }
        /// <summary>
        /// The diagnoses that are attached to the course. 
        /// </summary>
        public IEnumerable<Diagnosis> Diagnoses { get; set; }
        /// <summary>
        /// A collection of external beam plans for the course. 
        /// </summary>
        public IEnumerable<ExternalPlanSetup> ExternalPlanSetups { get; set; }
        /// <summary>
        /// The identifier of the Course. 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The intent of the course. 
        /// </summary>
        public string Intent { get; set; }
        /// <summary>
        /// Patient in which the course is defined. 
        /// </summary>
        public Patient Patient { get; set; }
        /// <summary>
        /// A collection of plans for the course. The plans can be of any type (external beam or brachytherapy). 
        /// </summary>
        public IEnumerable<PlanSetup> PlanSetups { get; set; }
        /// <summary>
        /// A collection of plan sums for the course. 
        /// </summary>
        public IEnumerable<PlanSum> PlanSums { get; set; }
        /// <summary>
        /// The date and time when the course was started. 
        /// </summary>
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// Parent ESAPI <c>Course</c> for navigation
        /// </summary>
        public Course _parent_course { get; set; }

        /// <summary>
        /// Constructor of the CourseExt
        /// </summary>
        /// <param name="c">Required is the ESAPI <c>Course</c></param>
        public CourseExt(Course c)
        {
            _parent_course = c;
            CompletedDateTime = new DateTime();
            StartDateTime = new DateTime();

            BrachyPlanSetups = c.BrachyPlanSetups;
            CompletedDateTime = c.CompletedDateTime;
            Diagnoses = c.Diagnoses;
            ExternalPlanSetups = c.ExternalPlanSetups;
            Id = c.Id;
            IonPlanSetups = c.IonPlanSetups;
            Intent = c.Intent;
            Patient = c.Patient;
            PlanSetups = c.PlanSetups;
            PlanSums = c.PlanSums;
            StartDateTime = c.StartDateTime;
            TreatmentPhases = c.TreatmentPhases;
            TreatmentSessions = c.TreatmentSessions;
        }

    }
}
