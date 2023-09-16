using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class OrderProduct
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public Order Order { get; set; }
        public long OrderId { get; set; }
        public bool IsSent { get; set; }
        public long SellerId { get; set; }
        public int Amount { get; set; }
    }
}
