using DataAccessLayer.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Context.Configurations
{
    public class OrderConfiguration
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.UserId).IsRequired();

            builder.HasOne(x => x.User).WithMany(x => x.Orders).HasForeignKey(x => x.UserId);

        }
    }
}
