using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contracts.Models
{
    public class MeetingRecord
    {
        public int Id { get; set; }
        public int MeetingTypeIde { get; set; }
        public string Comment { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public bool Approved { get; set; }
        public int UserId { get; set; }
        public override string ToString()
        {
            return Begin.TimeOfDay.ToString("hh\\:mm") + " - " + End.TimeOfDay.ToString("hh\\:mm");
        }
    }
}
