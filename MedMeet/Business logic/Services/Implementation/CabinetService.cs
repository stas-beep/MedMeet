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
            var allCabinets = await repository.GetAllAsync();
            return allCabinets.Select(cabinet => new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name });
        }

        public async Task<CabinetReadDto> GetByIdAsync(int id)
        {
            Cabinet cabinet = await repository.GetByIdAsync(id);
            if (cabinet == null)
            {
                return null;
            }

            CabinetReadDto result = new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name };

            return result;
        }

        public async Task<IEnumerable<CabinetReadDto>> GetByNameAsync(string name)
        {
            var allCabinets = await repository.GetByNameAsync(name);

            return allCabinets.Select(cabinet => new CabinetReadDto { Id = cabinet.Id, Name = cabinet.Name });
        }

        public async Task<CabinetReadDto> CreateAsync(CabinetCreateDto dto)
        {
            if (await repository.ExistsByNameAsync(dto.Name))
            {
                throw new InvalidOperationException($"Кабінет з ім'ям {dto.Name} вже існує.");
            }

            Cabinet result = new Cabinet { Name = dto.Name };
            await repository.AddAsync(result);
            await repository.SaveAsync();

            return new CabinetReadDto { Id = result.Id, Name = result.Name };
        }

        public async Task<CabinetReadDto> UpdateAsync(int id, CabinetUpdateDto dto)
        {
            Cabinet result = await repository.GetByIdAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException($"Кабінет з таким id ({id}) не знайдено");
            }

            if (await repository.ExistsByNameExceptIdAsync(dto.Name, id))
            {
                throw new InvalidOperationException($"Кабінет з ім'ям {dto.Name} вже існує.");
            }

            result.Name = dto.Name;
            await repository.UpdateAsync(result);
            await repository.SaveAsync();

            return new CabinetReadDto { Id = result.Id, Name = result.Name };
        }


        public async Task DeleteAsync(int id)
        {
            Cabinet result = await repository.GetByIdAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException($"Кабінет з таким id ({id}) не знайдено");
            }

            await repository.DeleteAsync(result);
            await repository.SaveAsync();
        }
    }
}
