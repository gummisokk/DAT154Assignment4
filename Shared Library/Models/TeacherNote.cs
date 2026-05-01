using System;
using System.Collections.Generic;
using System.Text;

namespace Teacher_Assessment.Models
{
    internal class TeacherNote
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Comment { get; set; }
        public string SimulationCaseId { get; set; }
    }
}
