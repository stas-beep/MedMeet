using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Generic_Repository.Interfaces
{
    public interface IPrescriptionRepository : IGenericRepository<Prescription>
    {
        Task<IEnumerable<Prescription>> GetByRecordIdAsync(int recordId);
        Task<IEnumerable<Prescription>> SearchByMedicationAsync(string medication);
    }
}