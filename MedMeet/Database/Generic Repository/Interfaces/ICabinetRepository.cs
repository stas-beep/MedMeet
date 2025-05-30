﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Generic_Repository.Interfaces
{
    public interface ICabinetRepository : IGenericRepository<Cabinet>
    {
        Task<Cabinet> GetByNameAsync(string name);
        Task<IEnumerable<Cabinet>> GetAllWithDoctorsAsync();
    }
}
