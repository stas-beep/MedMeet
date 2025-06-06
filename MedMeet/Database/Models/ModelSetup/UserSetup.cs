﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.Models.ModelSetup
{
    public class UserSetup : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.HasOne(u => u.Specialty).WithMany(s => s.Doctors).HasForeignKey(u => u.SpecialtyId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(u => u.Cabinet).WithMany(c => c.Doctors).HasForeignKey(u => u.CabinetId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.RecordAsPatient).WithOne(r => r.Patient).HasForeignKey(r => r.PatientId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(u => u.RecordAsDoctor).WithOne(r => r.Doctor).HasForeignKey(r => r.DoctorId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}