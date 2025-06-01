using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Record;

namespace Business_logic.Services.Interfaces
{
    public interface IRecordService
    {
        Task<IEnumerable<RecordReadDto>> GetAllAsync();
        Task<RecordReadDto> GetByIdAsync(int id);
        Task<IEnumerable<RecordReadDto>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<RecordReadDto>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<RecordReadDto>> GetByStatusAsync(string status);
        Task<IEnumerable<RecordReadDto>> GetUpcomingAsync(DateTime from, DateTime? to = null);
        Task<RecordReadDto> CreateAsync(RecordCreateDto dto);
        Task<RecordReadDto> UpdateAsync(int id, RecordUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
