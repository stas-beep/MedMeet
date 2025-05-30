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
    public class CabinetRepository : GenericRepository<Cabinet>, ICabinetRepository
    {
        public CabinetRepository(DbContext context) : base(context) {
        
        }

        public async Task<Cabinet> GetByNameAsync(string name)
        {
            return await databaseSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<Cabinet>> GetAllWithDoctorsAsync()
        {
            return await databaseSet.Include(c => c.Doctors).ToListAsync();
        }
    }
}
