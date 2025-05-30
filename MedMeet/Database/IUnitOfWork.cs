using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Generic_Repository.Interfaces;

namespace Database
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ICabinetRepository Cabinets { get; }
        IRecordRepository Records { get; }
        IPrescriptionRepository Prescriptions { get; }
        ISpecialtyRepository Specialties { get; }

        Task<int> CommitAsync();
        void Commit();
    }
}
