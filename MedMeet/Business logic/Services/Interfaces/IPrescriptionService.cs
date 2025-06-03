using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Prescription;
using Business_logic.Data_Transfer_Object.For_Users;
using Business_logic.Filters;
using Business_logic.Sorting;

namespace Business_logic.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<PrescriptionReadDto>> GetAllAsync();
        Task<PrescriptionReadDto> GetByIdAsync(int id);
        Task<IEnumerable<PrescriptionReadDto>> GetByRecordIdAsync(int recordId);
        Task<IEnumerable<PrescriptionReadDto>> SearchByMedicationAsync(string medication);
        Task<PrescriptionReadDto> CreateAsync(PrescriptionCreateDto dto);
        Task<PrescriptionReadDto> UpdateAsync(int id, PrescriptionUpdateDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<PrescriptionReadDto>> GetFilteredAsync(PrescriptionFilterDto filter);
        Task<IEnumerable<PrescriptionReadDto>> GetPagedAsync(SortingParameters parameters);
    }
}
