using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Prescription;
using Business_logic.Filters;
using Business_logic.Services.Interfaces;
using Database.Generic_Repository.Interfaces;
using Database.Models;

namespace Business_logic.Services.Implementation
{
    public class PrescriptionService : IPrescriptionService
    {
        private IPrescriptionRepository repository;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository)
        {
            repository = prescriptionRepository;
        }

        public async Task<IEnumerable<PrescriptionReadDto>> GetAllAsync()
        {
            var prescriptions = await repository.GetAllAsync();
            return prescriptions.Select(p => new PrescriptionReadDto { Id = p.Id, RecordId = p.RecordId, Medication = p.Medication, Dosage = p.Dosage, Instructions = p.Instructions });
        }

        public async Task<PrescriptionReadDto> GetByIdAsync(int id)
        {
            var prescription = await repository.GetByIdAsync(id);
            if (prescription == null)
            {
                throw new KeyNotFoundException($"Призначення з таким {id} не знайдено.");
            }

            return new PrescriptionReadDto { Id = prescription.Id, RecordId = prescription.RecordId, Medication = prescription.Medication, Dosage = prescription.Dosage, Instructions = prescription.Instructions };
        }

        public async Task<IEnumerable<PrescriptionReadDto>> GetByRecordIdAsync(int recordId)
        {
            var prescriptions = await repository.GetByRecordIdAsync(recordId);
            return prescriptions.Select(p => new PrescriptionReadDto { Id = p.Id, RecordId = p.RecordId, Medication = p.Medication, Dosage = p.Dosage, Instructions = p.Instructions });
        }

        public async Task<IEnumerable<PrescriptionReadDto>> SearchByMedicationAsync(string medication)
        {
            var prescriptions = await repository.SearchByMedicationAsync(medication);
            return prescriptions.Select(p => new PrescriptionReadDto { Id = p.Id, RecordId = p.RecordId, Medication = p.Medication, Dosage = p.Dosage, Instructions = p.Instructions });
        }

        public async Task<PrescriptionReadDto> CreateAsync(PrescriptionCreateDto dto)
        {
            Prescription prescription = new Prescription { RecordId = dto.RecordId, Medication = dto.Medication, Dosage = dto.Dosage, Instructions = dto.Instructions };

            await repository.AddAsync(prescription);
            await repository.SaveAsync();

            return new PrescriptionReadDto { Id = prescription.Id, RecordId = prescription.RecordId, Medication = prescription.Medication, Dosage = prescription.Dosage, Instructions = prescription.Instructions };
        }

        public async Task<PrescriptionReadDto> UpdateAsync(int id, PrescriptionUpdateDto dto)
        {
            var prescription = await repository.GetByIdAsync(id);
            if (prescription == null)
            {
                throw new KeyNotFoundException($"Призначення з таким {id} не знайдено.");
            }

            prescription.Medication = dto.Medication;
            prescription.Dosage = dto.Dosage;
            prescription.Instructions = dto.Instructions;

            await repository.UpdateAsync(prescription);
            await repository.SaveAsync();

            return new PrescriptionReadDto
            {
                Id = prescription.Id,
                RecordId = prescription.RecordId,
                Medication = prescription.Medication,
                Dosage = prescription.Dosage,
                Instructions = prescription.Instructions
            };
        }

        public async Task DeleteAsync(int id)
        {
            var prescription = await repository.GetByIdAsync(id);
            if (prescription == null)
            {
                throw new KeyNotFoundException($"Призначення з таким {id} не знайдено.");
            }

            await repository.DeleteAsync(prescription);
            await repository.SaveAsync();
        }

        public async Task<IEnumerable<PrescriptionReadDto>> GetPagedAsync(QueryParameters parameters)
        {
            var allPrescriptions = await repository.GetAllAsync();
            return Paginate(allPrescriptions, parameters.Page, parameters.PageSize);
        }

        public async Task<IEnumerable<PrescriptionReadDto>> GetFilteredAsync(PrescriptionFilterDto filter)
        {
            var allPrescriptions = await repository.GetAllAsync();
            var filtered = allPrescriptions.AsQueryable();

            if (filter.RecordId.HasValue)
            {
                filtered = filtered.Where(p => p.RecordId == filter.RecordId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Medication))
            {
                filtered = filtered.Where(p => p.Medication.Contains(filter.Medication, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Dosage))
            {
                filtered = filtered.Where(p => p.Dosage.Contains(filter.Dosage, StringComparison.OrdinalIgnoreCase));
            }

            var paged = Paginate(filtered, filter.Page, filter.PageSize);
            return paged;
        }

        private List<PrescriptionReadDto> Paginate(IEnumerable<Prescription> prescriptions, int page, int pageSize)
        {
            return prescriptions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PrescriptionReadDto
                {
                    Id = p.Id,
                    RecordId = p.RecordId,
                    Medication = p.Medication,
                    Dosage = p.Dosage,
                    Instructions = p.Instructions
                })
                .ToList();
        }

    }
}