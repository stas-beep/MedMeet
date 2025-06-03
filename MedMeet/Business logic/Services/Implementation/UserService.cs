using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Users;
using Business_logic.Filters;
using Business_logic.Services.Interfaces;
using Database.Generic_Repository.Interfaces;
using Database.Models;

namespace Business_logic.Services.Implementation
{
    public class UserService : IUserService
    {
        private IUserRepository repository;

        public UserService(IUserRepository userRepository)
        {
            repository = userRepository;
        }

        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await repository.GetAllAsync();

            return users.Select(u => new UserReadDto { Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role, SpecialtyId = u.SpecialtyId, CabinetId = u.CabinetId });
        }

        public async Task<UserReadDto> GetByIdAsync(int id)
        {
            var user = await repository.GetWithDetailsAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"Користувач з таким id ({id}) не знайдено.");
            }
            
            return new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = user.Role, SpecialtyId = user.SpecialtyId, CabinetId = user.CabinetId };
        }

        public async Task<IEnumerable<UserReadDto>> GetDoctorsAsync()
        {
            var doctors = await repository.GetDoctorsAsync();
            
            return doctors.Select(u => new UserReadDto {Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role, SpecialtyId = u.SpecialtyId, CabinetId = u.CabinetId });
        }

        public async Task<IEnumerable<UserReadDto>> GetPatientsAsync()
        {
            var patients = await repository.GetPatientsAsync();
            
            return patients.Select(u => new UserReadDto {Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role });
        }

        public async Task<UserReadDto> GetByEmailAsync(string email)
        {
            var user = await repository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"Користувач з таким email ({email}) не знайдено.");
            }

            return new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = user.Role, CabinetId = user.CabinetId, SpecialtyId = user.SpecialtyId };
        }

        public async Task<IEnumerable<UserReadDto>> SearchByNameAsync(string name)
        {
            var users = await repository.SearchByNameAsync(name);
            
            return users.Select(u => new UserReadDto {Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role, SpecialtyId = u.SpecialtyId, CabinetId = u.CabinetId });
        }

        public async Task<UserReadDto> CreateAsync(UserCreateDto dto)
        {
            User user = new User { FullName = dto.FullName, Email = dto.Email, Role = dto.Role, Password = dto.Password, SpecialtyId = dto.SpecialtyId, CabinetId = dto.CabinetId };
            
            await repository.AddAsync(user);
            await repository.SaveAsync();
            
            return new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = user.Role };
        }

        public async Task<UserReadDto> UpdateAsync(int id, UserUpdateDto dto)
        {
            var user = await repository.GetByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"Користувач з таким id ({id}) не знайдено.");
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Role = dto.Role;
            user.SpecialtyId = dto.SpecialtyId;
            user.CabinetId = dto.CabinetId;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = dto.Password;
            }

            await repository.UpdateAsync(user);
            await repository.SaveAsync();

            return new UserReadDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                CabinetId = user.CabinetId,
                SpecialtyId = user.SpecialtyId
            };
        }

        public async Task<IEnumerable<UserReadDto>> GetPagedAsync(QueryParameters parameters)
        {
            var allUsers = await repository.GetAllAsync();
            return Paginate(allUsers, parameters.Page, parameters.PageSize);
        }


        public async Task DeleteAsync(int id)
        {
            var user = await repository.GetByIdAsync(id);
            
            if (user == null)
            {
                throw new KeyNotFoundException($"Користувач з таким id ({id}) не знайдено.");
            }

            await repository.DeleteAsync(user);
            await repository.SaveAsync();
        }

        public async Task<IEnumerable<UserReadDto>> GetFilteredAsync(UserFilterDto filter)
        {
            var allUsers = await repository.GetAllAsync();
            var filtered = allUsers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                filtered = filtered.Where(u => u.FullName.Contains(filter.FullName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                filtered = filtered.Where(u => u.Email.Contains(filter.Email, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Role))
            {
                filtered = filtered.Where(u => u.Role == filter.Role);
            }

            if (filter.SpecialtyId.HasValue)
            {
                filtered = filtered.Where(u => u.SpecialtyId == filter.SpecialtyId.Value);
            }

            if (filter.CabinetId.HasValue)
            {
                filtered = filtered.Where(u => u.CabinetId == filter.CabinetId.Value);
            }

            return Paginate(filtered, filter.Page, filter.PageSize);
        }

        private List<UserReadDto> Paginate(IEnumerable<User> users, int page, int pageSize)
        {
            return users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role,
                    SpecialtyId = u.SpecialtyId,
                    CabinetId = u.CabinetId
                })
                .ToList();
        }
    }
}
