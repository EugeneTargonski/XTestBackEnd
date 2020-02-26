using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Models
{
    public class UnplannedDay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsDayOff { get; set; }
    }
}
