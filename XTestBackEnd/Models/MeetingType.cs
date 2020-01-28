using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTestBackEnd.Models
{
    public class MeetingType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PlannedDurationMinutes { get; set; }
    }
}
