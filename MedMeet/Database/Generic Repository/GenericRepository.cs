using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Generic_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Generic_Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly MedMeetDbContext database;
        protected readonly DbSet<T> databaseSet;

        public GenericRepository(MedMeetDbContext context)
        {
            database = context;
            databaseSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await databaseSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await databaseSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await databaseSet.AddAsync(entity);
            await database.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            databaseSet.Update(entity);
            await database.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            databaseSet.Remove(entity);
            await database.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await database.SaveChangesAsync();
        }
    }
}
