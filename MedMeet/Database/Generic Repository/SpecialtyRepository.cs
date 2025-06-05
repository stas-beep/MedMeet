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
    public class SpecialtyRepository : GenericRepository<Specialty>, ISpecialtyRepository
    {
        public SpecialtyRepository(MedMeetDbContext context) : base(context) { 
        
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await databaseSet
                .AnyAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> ExistsByNameExceptIdAsync(string name, int excludedId)
        {
            return await databaseSet.AnyAsync(s => s.Name.ToLower() == name.ToLower() && s.Id != excludedId);
        }

        public async Task<Specialty> GetWithDoctorsAsync(int id)
        {
            return await databaseSet.Include(s => s.Doctors).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Specialty>> SearchByNameAsync(string name)
        {
            return await databaseSet.Where(s => s.Name.Contains(name)).ToListAsync();
        }
    }
}
