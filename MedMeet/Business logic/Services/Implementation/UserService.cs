using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Users;
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

            return users.Select(u => new UserReadDto { Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role });
        }

        public async Task<UserReadDto> GetByIdAsync(int id)
        {
            var user = await repository.GetWithDetailsAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with id {id} not found.");
            }
            
            return new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = user.Role };
        }

        public async Task<IEnumerable<UserReadDto>> GetDoctorsAsync()
        {
            var doctors = await repository.GetDoctorsAsync();
            
            return doctors.Select(u => new UserReadDto { Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role });
        }

        public async Task<IEnumerable<UserReadDto>> GetPatientsAsync()
        {
            var patients = await repository.GetPatientsAsync();
            
            return patients.Select(u => new UserReadDto { Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role });
        }

        public async Task<UserReadDto> GetByEmailAsync(string email)
        {
            var user = await repository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found.");
            }
            
            return new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = user.Role };
        }

        public async Task<IEnumerable<UserReadDto>> SearchByNameAsync(string name)
        {
            var users = await repository.SearchByNameAsync(name);
            
            return users.Select(u => new UserReadDto { Id = u.Id, FullName = u.FullName, Email = u.Email, Role = u.Role });
        }

        public async Task<UserReadDto> CreateAsync(UserCreateDto dto)
        {
            User user = new User { FullName = dto.FullName, Email = dto.Email, Role = dto.Role, Password = dto.Password };
            
            await repository.AddAsync(user);
            await repository.SaveAsync();
            
            return new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = user.Role };
        }

        public async Task UpdateAsync(int id, UserUpdateDto dto)
        {
            var user = await repository.GetByIdAsync(id);
            
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id {id} not found.");
            }
            
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Role = dto.Role;
            
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = dto.Password;
            }
            await repository.UpdateAsync(user);
            await repository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await repository.GetByIdAsync(id);
            
            if (user == null)
            {
                throw new KeyNotFoundException($"User with id {id} not found.");
            }
            
            await repository.DeleteAsync(user);
            await repository.SaveAsync();
        }
    }
}
