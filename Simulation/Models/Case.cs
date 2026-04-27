using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation.Models
{
    public class Case
    {
        public string Id { get; set; }
        public Patient Patient { get; set; }
        public VitalSigns CurrentVitals { get; set; }
        public List<Medication> Medications { get; set; }
        public List<string> Allergies { get; set; }
    }
}
