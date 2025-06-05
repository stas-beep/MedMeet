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
        public CabinetRepository(MedMeetDbContext context) : base(context) {
        
        }
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await databaseSet.AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistsByNameExceptIdAsync(string name, int excludedId)
        {
            return await databaseSet
                .AnyAsync(s => s.Name.ToLower() == name.ToLower() && s.Id != excludedId);
        }


        public async Task<IEnumerable<Cabinet>> GetByNameAsync(string name)
        {
            return await databaseSet.Where(c => c.Name.ToLower().Contains(name.ToLower())).ToListAsync();
        }
    }
}
