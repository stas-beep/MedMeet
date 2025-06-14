using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Pagination;
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
            var allSpecialties = await repository.GetAllAsync();
            
            return allSpecialties.Select(s => new SpecialtyReadDto { Id = s.Id, Name = s.Name });
        }

        public async Task<SpecialtyReadDto> GetByIdAsync(int id)
        {
            Specialty specialty = await repository.GetWithDoctorsAsync(id);
            if (specialty == null)
            {
                throw new KeyNotFoundException($"Спеціальність з таким id ({id}) не знайдено.");
            }
            
            SpecialtyReadDto result = new SpecialtyReadDto { Id = specialty.Id, Name = specialty.Name };
            return result;
        }

        public async Task<IEnumerable<SpecialtyReadDto>> SearchByNameAsync(string name)
        {
            var allSpecialties = await repository.SearchByNameAsync(name);
            
            return allSpecialties.Select(s => new SpecialtyReadDto { Id = s.Id, Name = s.Name });
        }

        public async Task<SpecialtyReadDto> CreateAsync(SpecialtyCreateDto dto)
        {
            if (await repository.ExistsByNameAsync(dto.Name))
            {
                throw new InvalidOperationException($"Спеціальність з іменем {dto.Name} вже існує.");
            }

            Specialty specialty = new Specialty { Name = dto.Name };

            await repository.AddAsync(specialty);
            await repository.SaveAsync();

            return new SpecialtyReadDto { Id = specialty.Id, Name = specialty.Name };
        }

        public async Task<SpecialtyReadDto> UpdateAsync(int id, SpecialtyUpdateDto dto)
        {
            Specialty specialty = await repository.GetByIdAsync(id);

            if (specialty == null)
            {
                throw new KeyNotFoundException($"Спеціальність з таким id ({id}) не знайдено.");
            }

            if (await repository.ExistsByNameExceptIdAsync(dto.Name, id))
            {
                throw new InvalidOperationException($"Спеціальність з іменем {dto.Name} вже існує.");
            }

            specialty.Name = dto.Name;

            await repository.UpdateAsync(specialty);
            await repository.SaveAsync();

            return new SpecialtyReadDto { Id = specialty.Id, Name = specialty.Name };
        }


        public async Task DeleteAsync(int id)
        {
            Specialty specialty = await repository.GetByIdAsync(id);
            
            if (specialty == null)
            {
                throw new KeyNotFoundException($"Спеціальність з таким id ({id}) не знайдено.");

            }

            await repository.DeleteAsync(specialty);
            await repository.SaveAsync();
        }
    }
}
