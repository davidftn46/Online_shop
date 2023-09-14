using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition
{
    public class Roles
    {
        [Flags]
        public enum RolesType : byte
        {
            Customer = 1,
            Seller = 2,
            Administrator = 3
        }
    }
}
