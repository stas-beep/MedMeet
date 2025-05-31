using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Cabinets;
using Business_logic.Services.Interfaces;
using Database.Generic_Repository.Interfaces;
using Database.Models;

namespace Business_logic.Services.Implementation
{
    public class CabinetService : ICabinetService
    {
        private ICabinetRepository repository;

        public CabinetService(ICabinetRepository cabinetRepository)
        {
            repository = cabinetRepository;
        }

        public async Task<IEnumerable<CabinetReadDto>> GetAllAsync()
        {
            var cabinets = await repository.GetAllWithDoctorsAsync();
            return cabinets.Select(cabinet => new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name });
        }

        public async Task<IEnumerable<CabinetReadDto>> GetAllWithDoctorsAsync()
        {
            var cabinets = await repository.GetAllWithDoctorsAsync();
            return cabinets.Select(cabinet => new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name });
        }

        public async Task<CabinetReadDto> GetByIdAsync(int id)
        {
            var cabinet = await repository.GetByIdAsync(id);
            if (cabinet == null)
            {
                return null;
            }

            return new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name };
        }

        public async Task<CabinetReadDto> GetByNameAsync(string name)
        {
            var cabinet = await repository.GetByNameAsync(name);
            if (cabinet == null)
            {
                return null;
            }

            return new CabinetReadDto{ Id = cabinet.Id, Name = cabinet.Name };
        }

        public async Task CreateAsync(CabinetCreateDto dto)
        {
            Cabinet cabinet = new Cabinet { Name = dto.Name };
            await repository.AddAsync(cabinet);
        }

        public async Task UpdateAsync(int id, CabinetUpdateDto updateDtoObject)
        {
            var cabinet = await repository.GetByIdAsync(id);
            if (cabinet == null)
            {
                throw new KeyNotFoundException($"Cabinet with id {id} not found");
            }

            cabinet.Name = updateDtoObject.Name;
            await repository.UpdateAsync(cabinet);
        }

        public async Task DeleteAsync(int id)
        {
            var cabinet = await repository.GetByIdAsync(id);
            if (cabinet == null)
            {
                throw new KeyNotFoundException($"Cabinet with id {id} not found");
            }

            await repository.DeleteAsync(cabinet);
        }
    }
}
