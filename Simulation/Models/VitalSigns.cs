using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation.Models
{
    internal class VitalSigns
    {
        public int HeartRate { get; set; }
        public int SystolicBP { get; set; }
        public int DiastolicBP { get; set; }
        public int RespiratoryRate { get; set; }
        public int OxygenSaturation { get; set; }
        public double Temperature { get; set; }
    }
}
