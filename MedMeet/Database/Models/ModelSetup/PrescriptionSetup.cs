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
    public class PrescriptionSetup : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Medication)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Dosage)
                   .HasMaxLength(50);

            builder.Property(p => p.Instructions)
                   .HasMaxLength(250);

            builder.HasOne(p => p.Record)
                   .WithMany(r => r.Prescriptions)
                   .HasForeignKey(p => p.RecordId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
