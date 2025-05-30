using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Database.Models;
using Database.Models.ModelSetup;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class MedMeetDbContext : DbContext
    {
        public MedMeetDbContext(DbContextOptions<MedMeetDbContext> options): base(options)
        {

        }

        public DbSet<Cabinet> Cabinets { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Record> Records { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CabinetSetup());
            builder.ApplyConfiguration(new PrescriptionSetup());
            builder.ApplyConfiguration(new RecordSetup());
            builder.ApplyConfiguration(new SpecialtySetup());
            builder.ApplyConfiguration(new UserSetup());

            base.OnModelCreating(builder);
        }
    }
}
