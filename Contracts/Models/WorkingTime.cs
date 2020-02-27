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
        public bool IsMondayOff { get; set; }
        public bool IsTuesdayOff { get; set; }
        public bool IsWednesdayOff { get; set; }
        public bool IsThursdayOff { get; set; }
        public bool IsFridayOff { get; set; }
        public bool IsSaturdayOff { get; set; }
        public bool IsSundayOff { get; set; }
    }
}
