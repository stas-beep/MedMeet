using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Models.ModelSetup
{
    public class CabinetSetup : IEntityTypeConfiguration<Cabinet>
    {
        public void Configure(EntityTypeBuilder<Cabinet> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);

            builder.HasMany(c => c.Doctors).WithOne(u => u.Cabinet).HasForeignKey(u => u.CabinetId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
