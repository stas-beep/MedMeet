using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Generic_Repository.Interfaces
{
    public interface ISpecialtyRepository : IGenericRepository<Specialty>
    {
        Task<Specialty> GetWithDoctorsAsync(int id);
        Task<IEnumerable<Specialty>> SearchByNameAsync(string name);
    }
}
