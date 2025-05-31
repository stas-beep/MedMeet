using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Cabinets;
using Database.Models;

namespace Business_logic.Services.Interfaces
{
    public interface ICabinetService
    {
        Task<CabinetReadDto> GetByIdAsync(int id);
        Task<CabinetReadDto> GetByNameAsync(string name);
        Task<IEnumerable<CabinetReadDto>> GetAllWithDoctorsAsync();
        Task<IEnumerable<CabinetReadDto>> GetAllAsync();
        Task CreateAsync(CabinetCreateDto dto);
        Task UpdateAsync(int id, CabinetUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
