using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition.DTO
{
    public class NewOrderDTO
    {
        public long UserId { get; set; }
        public string Comment { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public List<NewOrderProductDTO> Products { get; set; }
    }
}
