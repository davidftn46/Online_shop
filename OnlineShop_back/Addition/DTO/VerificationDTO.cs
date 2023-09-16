using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition.DTO
{
    public class VerificationDTO
    {
        public string UserName { get; set; }
        public bool IsAccepted { get; set; }
        public string Reason { get; set; }
    }
}
