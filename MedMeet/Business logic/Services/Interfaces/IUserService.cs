using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Users;

namespace Business_logic.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserReadDto>> GetAllAsync();
        Task<UserReadDto> GetByIdAsync(int id);
        Task<IEnumerable<UserReadDto>> GetDoctorsAsync();
        Task<IEnumerable<UserReadDto>> GetPatientsAsync();
        Task<UserReadDto> GetByEmailAsync(string email);
        Task<IEnumerable<UserReadDto>> SearchByNameAsync(string name);
        Task<UserReadDto> CreateAsync(UserCreateDto dto);
        Task<UserReadDto> UpdateAsync(int id, UserUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
