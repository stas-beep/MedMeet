using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Speciality;
using Business_logic.Services.Interfaces;
using Database.Generic_Repository;
using Database.Generic_Repository.Interfaces;
using Database.Models;

namespace Business_logic.Services.Implementation
{
    public class SpecialtyService : ISpecialtyService
    {
        private ISpecialtyRepository repository;

        public SpecialtyService(ISpecialtyRepository specialtyRepository)
        {
            repository = specialtyRepository;
        }

        public async Task<IEnumerable<SpecialtyReadDto>> GetAllAsync()
        {
            var specialties = await repository.GetAllAsync();
            
            return specialties.Select(s => new SpecialtyReadDto { Id = s.Id, Name = s.Name });
        }

        public async Task<SpecialtyReadDto> GetByIdAsync(int id)
        {
            var specialty = await repository.GetWithDoctorsAsync(id);
            if (specialty == null)
            {
                throw new KeyNotFoundException($"Specialty with id {id} not found.");
            }
            
            return new SpecialtyReadDto { Id = specialty.Id, Name = specialty.Name };
        }

        public async Task<IEnumerable<SpecialtyReadDto>> SearchByNameAsync(string name)
        {
            var specialties = await repository.SearchByNameAsync(name);
            
            return specialties.Select(s => new SpecialtyReadDto { Id = s.Id, Name = s.Name });
        }

        public async Task<SpecialtyReadDto> CreateAsync(SpecialtyCreateDto dto)
        {
            Specialty specialty = new Specialty { Name = dto.Name };
            
            await repository.AddAsync(specialty);
            await repository.SaveAsync();
            
            return new SpecialtyReadDto { Id = specialty.Id, Name = specialty.Name };
        }

        public async Task<SpecialtyReadDto> UpdateAsync(int id, SpecialtyUpdateDto dto)
        {
            var specialty = await repository.GetByIdAsync(id);

            if (specialty == null)
            {
                throw new KeyNotFoundException($"Specialty with id {id} not found.");
            }

            specialty.Name = dto.Name;

            await repository.UpdateAsync(specialty);
            await repository.SaveAsync();

            return new SpecialtyReadDto { Id = specialty.Id, Name = specialty.Name };
        }

        public async Task DeleteAsync(int id)
        {
            var specialty = await repository.GetByIdAsync(id);
            
            if (specialty == null)
            {
                throw new KeyNotFoundException($"Specialty with id {id} not found.");
            }
            
            await repository.DeleteAsync(specialty);
            await repository.SaveAsync();
        }
    }
}
