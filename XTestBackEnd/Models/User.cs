using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XTestBackEnd.Models
{
    public class User
    {
        public int Id { get; set; }
        public string GoogleID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string EMail { get; set; }
        public string Description { get; set; }

    }
}
