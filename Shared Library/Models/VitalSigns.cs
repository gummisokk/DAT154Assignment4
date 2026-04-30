using System;
using System.Collections.Generic;
using System.Text;

namespace Shared_Library.Models
{
    public class VitalSigns
    {
        public int SystolicBP { get; set; }
        public int DiastolicBP { get; set; }
        public int HeartRate { get; set; }
        public int RespiratoryRate { get; set; }
        public int SpO2 { get; set; }
        public double Temperature { get; set; }
    }
}
