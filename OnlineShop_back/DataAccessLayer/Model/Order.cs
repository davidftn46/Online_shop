using DataAccesLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class Order
    {
        public long Id { get; set; }
        public User User { get; set; }
        public long UserId { get; set; }
        public string Comment { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public List<OrderProduct> OrderProducts { get; set; }

        public bool Shipped { get; set; }
        public bool Canceled { get; set; }
    }
}
