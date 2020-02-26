using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Models
{
    public class WorkingTime
    {
        public int Id { get; set; }
        public DateTime WorkTimeBegin { get; set; }
        public DateTime WorkTimeEnd { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsSaturdayOff { get; set; }
        public bool IsSundayOff { get; set; }
    }
}
