using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition.DTO
{
    public class OrderDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public List<OrderProductDTO> OrderProducts { get; set; }
        public int Minutes { get; set; }
        public bool Canceled { get; set; }
        public bool Shipped { get; set; }
    }
}
