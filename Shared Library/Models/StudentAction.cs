using System;
using System.Collections.Generic;
using System.Text;

namespace Teacher_Assessment.Models
{
    internal class StudentAction
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
