using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition.DTO
{
    public class OrderProductDTO
    {
        public long ProductId { get; set; }
        public long OrderId { get; set; }
        public ProductDTO Product { get; set; }
        public int Amount { get; set; }
        public bool IsSent { get; set; }
        public long SellerId { get; set; }
    }
}
