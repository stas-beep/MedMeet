using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Generic_Repository.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Generic_Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MedMeetDbContext context) : base(context) { }

        public async Task<IEnumerable<User>> GetDoctorsAsync()
        {
            return await databaseSet.Where(u => u.Role == "Doctor").ToListAsync();
        }

        public async Task<IEnumerable<User>> GetPatientsAsync()
        {
            return await databaseSet.Where(u => u.Role == "Patient").ToListAsync();
        }

        public async Task<User> GetWithDetailsAsync(int id)
        {
            return await databaseSet.Include(u => u.Cabinet).Include(u => u.Specialty).Include(u => u.RecordAsDoctor).Include(u => u.RecordAsPatient).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await databaseSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> SearchByNameAsync(string name)
        {
            return await databaseSet.Where(u => u.FullName.Contains(name)).ToListAsync();
        }
    }
}
