using DataAccessLayer.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Context.Configurations
{
    public class OrderProductConfiguration
    {
        public void Configure(EntityTypeBuilder<OrderProduct> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Order).WithMany(x => x.OrderProducts).HasForeignKey(x => x.Id);
        }
    }
}
