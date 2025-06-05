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
    public class RecordRepository : GenericRepository<Record>, IRecordRepository
    {
        public RecordRepository(MedMeetDbContext context) : base(context) {
        
        }

        public async Task<IEnumerable<Record>> GetByCabinetIdAndDateRangeAsync(int cabinetId, DateTime from, DateTime to)
        {
            return await databaseSet.Include(r => r.Doctor) .Where(r => r.Doctor.CabinetId == cabinetId && r.AppointmentDate >= from && r.AppointmentDate <= to).ToListAsync();
        }


        public async Task<IEnumerable<Record>> GetByPatientIdAsync(int patientId)
        {
            return await databaseSet.Where(r => r.PatientId == patientId).Include(r => r.Doctor).ToListAsync();
        }

        public async Task<IEnumerable<Record>> GetByDoctorIdAsync(int doctorId)
        {
            return await databaseSet.Where(r => r.DoctorId == doctorId).Include(r => r.Patient).ToListAsync();
        }

        public async Task<IEnumerable<Record>> GetByStatusAsync(string status)
        {
            return await databaseSet.Where(r => r.Status == status).Include(r => r.Patient).Include(r => r.Doctor).ToListAsync();
        }

        public async Task<IEnumerable<Record>> GetUpcomingAsync(DateTime from, DateTime? to = null)
        {
            var query = databaseSet.Where(r => r.AppointmentDate >= from);

            if (to.HasValue)
            {
                query = query.Where(r => r.AppointmentDate <= to.Value);
            }

            return await query.Include(r => r.Patient).Include(r => r.Doctor).ToListAsync();
        }

        public async Task<IEnumerable<Record>> GetAllWithDetailsAsync()
        {
            return await databaseSet.Include(r => r.Patient).Include(r => r.Doctor).ToListAsync();
        }

        public async Task<Record> GetWithDetailsAsync(int id)
        {
            return await databaseSet.Where(r => r.Id == id).Include(r => r.Patient).Include(r => r.Doctor).Include(r => r.Prescriptions).FirstOrDefaultAsync();
        }
    }
}
