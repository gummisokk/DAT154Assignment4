using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation.Models
{
    internal class CaseScenario
    {
        public Patient Patient { get; set; }

        public MedicalHistory MedicalHistory { get; set; }
        public VitalSigns CurrentVitals { get; set; }

        public List<Medication> Medications { get; set; }
        public List<Allergy> Allergies { get; set; }
        public List<LabResult> LabResults { get; set; }

        public List<Goal> Goals { get; set; }
    }
}
