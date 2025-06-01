using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Models.ModelSetup
{
    public class SpecialtySetup : IEntityTypeConfiguration<Specialty>
    {
        public void Configure(EntityTypeBuilder<Specialty> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);

            builder.HasMany(s => s.Doctors).WithOne(u => u.Specialty).HasForeignKey(u => u.SpecialtyId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
