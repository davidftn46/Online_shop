using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addition.DTO
{
    public class ProductDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
    }
}
