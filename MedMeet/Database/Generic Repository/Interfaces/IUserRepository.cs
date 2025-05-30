using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Generic_Repository.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<User>> GetDoctorsAsync();
        Task<IEnumerable<User>> GetPatientsAsync();
        Task<User> GetWithDetailsAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> SearchByNameAsync(string name);
    }
}
