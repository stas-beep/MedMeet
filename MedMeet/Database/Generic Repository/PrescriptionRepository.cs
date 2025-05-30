using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Generic_Repository.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Generic_Repository
{
    public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(DbContext context) : base(context) { 
        
        }

        public async Task<IEnumerable<Prescription>> GetByRecordIdAsync(int recordId)
        {
            return await databaseSet
                .Where(p => p.RecordId == recordId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> SearchByMedicationAsync(string medication)
        {
            return await databaseSet
                .Where(p => p.Medication.Contains(medication))
                .ToListAsync();
        }
    }
}
