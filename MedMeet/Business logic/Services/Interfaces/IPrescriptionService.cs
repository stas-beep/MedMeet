using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Prescription;

namespace Business_logic.Services.Interfaces
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<PrescriptionReadDto>> GetAllAsync();
        Task<PrescriptionReadDto> GetByIdAsync(int id);
        Task<IEnumerable<PrescriptionReadDto>> GetByRecordIdAsync(int recordId);
        Task<IEnumerable<PrescriptionReadDto>> SearchByMedicationAsync(string medication);
        Task<PrescriptionReadDto> CreateAsync(PrescriptionCreateDto dto);
        Task UpdateAsync(int id, PrescriptionUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
