using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Speciality;

namespace Business_logic.Services.Interfaces
{
    public interface ISpecialtyService
    {
        Task<IEnumerable<SpecialtyReadDto>> GetAllAsync();
        Task<SpecialtyReadDto> GetByIdAsync(int id);
        Task<IEnumerable<SpecialtyReadDto>> SearchByNameAsync(string name);
        Task<SpecialtyReadDto> CreateAsync(SpecialtyCreateDto dto);
        Task<SpecialtyReadDto> UpdateAsync(int id, SpecialtyUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
