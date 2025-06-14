using Business_logic.Data_Transfer_Object.For_Users;
using Business_logic.Filters;
using Business_logic.Services.Interfaces;
using Business_logic.Sorting;
using Database.Generic_Repository.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business_logic.Services.Implementation
{
    public class UserService : IUserService
    {
        private UserManager<User> userManager;
        private IUserRepository repository;

        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            repository = userRepository;
            this.userManager = userManager;
        }

        public async Task<IEnumerable<UserReadDto>> GetAllAsync()
        {
            var users = await repository.GetAllAsync();
            List<User> userList = users.ToList();
            List<UserReadDto> userDtos = new List<UserReadDto>();

            foreach (User user in userList)
            {
                var roles = await userManager.GetRolesAsync(user);
                string? role = "";
                if (roles.Any())
                {
                    role = roles.FirstOrDefault();
                }
                else
                {
                    role = "Невказана роль";
                }

                userDtos.Add(new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = role, SpecialtyId = user.SpecialtyId, CabinetId = user.CabinetId });
            }

            return userDtos;
        }

        public async Task<UserReadDto> GetByIdAsync(int id)
        {
            User user = await repository.GetWithDetailsAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"Користувач з таким id ({id}) не знайдено.");
            }

            var roles = await userManager.GetRolesAsync(user);
            string? role = "";
            if (roles.Any())
            {
                role = roles.FirstOrDefault();
            }
            else
            {
                role = "Невказана роль";
            }

            return new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = role, SpecialtyId = user.SpecialtyId, CabinetId = user.CabinetId };
        }

        public async Task<IEnumerable<UserReadDto>> GetDoctorsAsync()
        {
            var allDoctors = await userManager.GetUsersInRoleAsync("Doctor");

            return allDoctors.Select(u => new UserReadDto { Id = u.Id, FullName = u.FullName, Email = u.Email, Role = "Doctor", SpecialtyId = u.SpecialtyId, CabinetId = u.CabinetId });
        }

        public async Task<IEnumerable<UserReadDto>> GetPatientsAsync()
        {
            var allPatients = await userManager.GetUsersInRoleAsync("Patient");

            return allPatients.Select(u => new UserReadDto { Id = u.Id, FullName = u.FullName, Email = u.Email, Role = "Patient" });
        }

        public async Task<UserReadDto> GetByEmailAsync(string email)
        {
            User user = await repository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException($"Користувач з таким email ({email}) не знайдено.");
            }

            var roles = await userManager.GetRolesAsync(user);

            string? role = "";
            if (roles.Any())
            {
                role = roles.FirstOrDefault();
            }
            else
            {
                role = "Невказана роль";
            }

            return new UserReadDto{ Id = user.Id, FullName = user.FullName, Email = user.Email, Role = role, CabinetId = user.CabinetId, SpecialtyId = user.SpecialtyId };
        }

        public async Task<IEnumerable<UserReadDto>> SearchByNameAsync(string name)
        {
            var users = await repository.SearchByNameAsync(name);
            List<UserReadDto> userDtos = new List<UserReadDto>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                string? role = "";
                if (roles.Any())
                {
                    role = roles.FirstOrDefault();
                }
                else
                {
                    role = "Невказана роль";
                }

                UserReadDto result = new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = role, SpecialtyId = user.SpecialtyId, CabinetId = user.CabinetId };
                userDtos.Add(result);
            }

            return userDtos;
        }

        public async Task<UserReadDto> CreateAsync(UserCreateDto dto)
        {
            User user = new User { FullName = dto.FullName, Email = dto.Email, UserName = dto.Email, SpecialtyId = dto.SpecialtyId, CabinetId = dto.CabinetId};
            var result = await userManager.CreateAsync(user, dto.Password);
            await repository.SaveAsync();

            if (!result.Succeeded)
            {
                throw new Exception("Щось пішло не так");
            }

            if (!string.IsNullOrEmpty(dto.Role))
            {
                var roleResult = await userManager.AddToRoleAsync(user, dto.Role);
            }

            var roles = await userManager.GetRolesAsync(user);
            string role = "";
            if (roles.Any())
            {
                role = roles.FirstOrDefault();
            }
            else
            {
                role = "Невказана роль";
            }

            UserReadDto res = new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = role, SpecialtyId = user.SpecialtyId, CabinetId = user.CabinetId };
            return res;
        }

        public async Task<UserReadDto> UpdateAsync(int id, UserUpdateDto dto, string currentUserId, string currentUserRole)
        {
            User user = await repository.GetByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"Користувача з id={id} не знайдено.");
            }

            if (currentUserRole != "Admin" && user.Id.ToString() != currentUserId)
            {
                throw new UnauthorizedAccessException("Немає прав оновлювати цього користувача.");
            }

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.SpecialtyId = dto.SpecialtyId.Value;
            user.CabinetId = dto.CabinetId.Value;

            if (currentUserRole == "Admin")
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                if (!currentRoles.Contains(dto.Role))
                {
                    var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        throw new Exception("Не вдалося видалити старі ролі.");
                    }

                    var addResult = await userManager.AddToRoleAsync(user, dto.Role);
                    if (!addResult.Succeeded)
                    {
                        throw new Exception("Не вдалося додати нову роль.");
                    }
                }
            }

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new Exception("Не вдалося оновити користувача.");
            }

            await repository.SaveAsync();

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return new UserReadDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = role,
                SpecialtyId = user.SpecialtyId,
                CabinetId = user.CabinetId
            };
        }


        public async Task<IEnumerable<UserReadDto>> GetPagedAsync(SortingParameters parameters)
        {
            var allUsers = await repository.GetAllAsync();
            IQueryable<User> sorted = allUsers.AsQueryable();

            if (parameters.SortBy != null)
            {
                var sortBy = parameters.SortBy.ToLower();

                if (sortBy == "fullname")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(u => u.FullName);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(u => u.FullName);
                    }
                }
                else if (sortBy == "email")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(u => u.Email);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(u => u.Email);
                    }
                }
                else if (sortBy == "specialtyid")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(u => u.SpecialtyId);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(u => u.SpecialtyId);
                    }
                }
                else if (sortBy == "cabinetid")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(u => u.CabinetId);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(u => u.CabinetId);
                    }
                }
                else
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(u => u.Id);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(u => u.Id);
                    }
                }
            }
            else
            {
                if (parameters.IsDescending)
                {
                    sorted = sorted.OrderByDescending(u => u.Id);
                }
                else
                {
                    sorted = sorted.OrderBy(u => u.Id);
                }
            }

            return await PaginateAsync(sorted, parameters.Page, parameters.PageSize);
        }
        public async Task DeleteAsync(int id)
        {
            User user = await repository.GetByIdAsync(id);

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
            IQueryable<User> filtered = allUsers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.FullName))
            {
                filtered = filtered.Where(u => u.FullName.Contains(filter.FullName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                filtered = filtered.Where(u => u.Email.Contains(filter.Email, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.SpecialtyId.HasValue)
            {
                filtered = filtered.Where(u => u.SpecialtyId == filter.SpecialtyId.Value);
            }

            if (filter.CabinetId.HasValue)
            {
                filtered = filtered.Where(u => u.CabinetId == filter.CabinetId.Value);
            }

            return await PaginateAsync(filtered, filter.Page, filter.PageSize);
        }

        private async Task<List<UserReadDto>> PaginateAsync(IEnumerable<User> users, int page, int pageSize)
        {
            var pagedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            List<UserReadDto> result = new List<UserReadDto>();

            foreach (var user in pagedUsers)
            {
                var roles = await userManager.GetRolesAsync(user);
                string role = "";
                if (roles.Any())
                {
                    role = roles.FirstOrDefault();
                }
                else
                {
                    role = "Невказана роль";
                }

                UserReadDto newUser = new UserReadDto { Id = user.Id, FullName = user.FullName, Email = user.Email, Role = role, SpecialtyId = user.SpecialtyId, CabinetId = user.CabinetId };
                result.Add(newUser);
            }

            return result;
        }
    }
}
