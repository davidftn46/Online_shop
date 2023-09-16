using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition.DTO
{
    public class ProfileDTO
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string Token { get; set; }
        public Roles.RolesType Role { get; set; }
        public bool IsVerified { get; set; }
        public string Avatar { get; set; }
    }
}
