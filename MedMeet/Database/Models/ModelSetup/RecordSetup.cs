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
    public class RecordSetup : IEntityTypeConfiguration<Record>
    {
        public void Configure(EntityTypeBuilder<Record> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Status).HasMaxLength(50);
            builder.Property(r => r.Notes).HasMaxLength(500);

            builder.HasOne(r => r.Patient).WithMany(u => u.RecordAsPatient).HasForeignKey(r => r.PatientId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(r => r.Doctor).WithMany(u => u.RecordAsDoctor).HasForeignKey(r => r.DoctorId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(r => r.Prescriptions).WithOne(p => p.Record).HasForeignKey(p => p.RecordId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
