using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Generic_Repository.Interfaces
{
    public interface IRecordRepository : IGenericRepository<Record>
    {
        Task<IEnumerable<Record>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Record>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Record>> GetByStatusAsync(string status);
        Task<IEnumerable<Record>> GetByCabinetIdAndDateRangeAsync(int cabId, DateTime from, DateTime to);
        Task<IEnumerable<Record>> GetUpcomingAsync(DateTime from, DateTime? to = null);
        Task<Record> GetWithDetailsAsync(int id);
        Task<IEnumerable<Record>> GetAllWithDetailsAsync();
    }
}