using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Generic_Repository.Interfaces;
using Database.Generic_Repository;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MedMeetDbContext databaseContext;

        public IUserRepository Users { get; private set; }
        public ICabinetRepository Cabinets { get; private set; }
        public IRecordRepository Records { get; private set; }
        public IPrescriptionRepository Prescriptions { get; private set; }
        public ISpecialtyRepository Specialties { get; private set; }

        public UnitOfWork(MedMeetDbContext context)
        {
            databaseContext = context;
            Users = new UserRepository(databaseContext);
            Cabinets = new CabinetRepository(databaseContext);
            Records = new RecordRepository(databaseContext);
            Prescriptions = new PrescriptionRepository(databaseContext);
            Specialties = new SpecialtyRepository(databaseContext);
        }

        public async Task<int> CommitAsync()
        {
            return await databaseContext.SaveChangesAsync();
        }

        public void Commit()
        {
            databaseContext.SaveChanges();
        }

        public void Dispose()
        {
            databaseContext.Dispose();
        }
    }
}
