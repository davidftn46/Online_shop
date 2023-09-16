using DataAccessLayer.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Context.Configurations
{
    public class ProductConfiguration
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(250);
            builder.Property(x => x.Amount).IsRequired();

            builder.HasOne(x => x.User).WithMany(x => x.Products).HasForeignKey(x => x.UserId);
        }
    }
}
