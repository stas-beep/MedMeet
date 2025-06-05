using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Cabinets;
using Business_logic.Data_Transfer_Object.For_Pagination;
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
            var cabinets = await repository.GetAllAsync();
            return cabinets.Select(cabinet => new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name });
        }

        public async Task<CabinetReadDto> GetByIdAsync(int id)
        {
            var cabinet = await repository.GetByIdAsync(id);
            if (cabinet == null)
            {
                return null;
            }

            CabinetReadDto result = new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name };

            return result;
        }

        public async Task<IEnumerable<CabinetReadDto>> GetByNameAsync(string name)
        {
            var cabinets = await repository.GetByNameAsync(name);

            return cabinets.Select(cabinet => new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name });
        }

        public async Task<CabinetReadDto> CreateAsync(CabinetCreateDto dto)
        {
            if (await repository.ExistsByNameAsync(dto.Name))
            {
                throw new InvalidOperationException($"Кабінет з ім'ям \"{dto.Name}\" вже існує.");
            }

            Cabinet cabinet = new Cabinet { Name = dto.Name };
            await repository.AddAsync(cabinet);
            await repository.SaveAsync();

            return new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name };
        }

        public async Task<CabinetReadDto> UpdateAsync(int id, CabinetUpdateDto dto)
        {
            var cabinet = await repository.GetByIdAsync(id);
            if (cabinet == null)
            {
                throw new KeyNotFoundException($"Кабінет з таким id ({id}) не знайдено");
            }

            if (await repository.ExistsByNameExceptIdAsync(dto.Name, id))
            {
                throw new InvalidOperationException($"Кабінет з ім'ям \"{dto.Name}\" вже існує.");
            }

            cabinet.Name = dto.Name;
            await repository.UpdateAsync(cabinet);
            await repository.SaveAsync();

            return new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name };
        }


        public async Task DeleteAsync(int id)
        {
            var cabinet = await repository.GetByIdAsync(id);
            if (cabinet == null)
            {
                throw new KeyNotFoundException($"Кабінет з таким id ({id}) не знайдено");
            }

            await repository.DeleteAsync(cabinet);
            await repository.SaveAsync();
        }
    }
}
