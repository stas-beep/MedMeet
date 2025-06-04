using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Record;
using Business_logic.Data_Transfer_Object.For_Users;
using Business_logic.Filters;
using Business_logic.Sorting;

namespace Business_logic.Services.Interfaces
{
    public interface IRecordService
    {
        Task<IEnumerable<RecordReadDto>> GetAllAsync();
        Task<RecordReadDto> GetByIdAsync(int id);
        Task<IEnumerable<RecordReadDto>> GetMyPatientRecordsAsync();
        Task<IEnumerable<RecordReadDto>> GetMyDoctorRecordsAsync();
        Task<IEnumerable<RecordReadDto>> GetByStatusAsync(string status);
        Task<IEnumerable<RecordReadDto>> GetUpcomingAsync(DateTime from, DateTime? to = null);
        Task<RecordReadDto> CreateAsync(RecordCreateDto dto, int patientId);
        Task<RecordReadDto> UpdateAsync(int id, RecordUpdateDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<RecordReadDto>> GetFilteredAsync(RecordFilterDto filter);
        Task<IEnumerable<RecordReadDto>> GetPagedAsync(SortingParameters parameters);
    }
}
