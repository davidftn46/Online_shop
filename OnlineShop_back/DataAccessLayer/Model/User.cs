using Addition;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccesLayer.Model
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[] Salt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public Roles.RolesType Role { get; set; }
        public bool IsVerified { get; set; }
        public Guid PasswordGuid { get; set; } 
        public string ProfileUrl { get; set; }

        public List<Product> Products { get; set; }
        public List<Order> Orders { get; set; }
    }
}