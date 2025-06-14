using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Prescription;
using Business_logic.Filters;
using Business_logic.Services.Interfaces;
using Business_logic.Sorting;
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
            var allPrescriptions = await repository.GetAllAsync();
            return allPrescriptions.Select(p => new PrescriptionReadDto { Id = p.Id, RecordId = p.RecordId, Medication = p.Medication, Dosage = p.Dosage, Instructions = p.Instructions });
        }

        public async Task<PrescriptionReadDto> GetByIdAsync(int id)
        {
            Prescription prescription = await repository.GetByIdAsync(id);
            if (prescription == null)
            {
                throw new KeyNotFoundException($"Призначення з таким {id} не знайдено.");
            }

            PrescriptionReadDto result = new PrescriptionReadDto { Id = prescription.Id, RecordId = prescription.RecordId, Medication = prescription.Medication, Dosage = prescription.Dosage, Instructions = prescription.Instructions };
            return result;
        }

        public async Task<IEnumerable<PrescriptionReadDto>> GetByRecordIdAsync(int recordId)
        {
            var allPrescriptions = await repository.GetByRecordIdAsync(recordId);
            return allPrescriptions.Select(p => new PrescriptionReadDto { Id = p.Id, RecordId = p.RecordId, Medication = p.Medication, Dosage = p.Dosage, Instructions = p.Instructions });
        }

        public async Task<IEnumerable<PrescriptionReadDto>> SearchByMedicationAsync(string medication)
        {
            var allPrescriptions = await repository.SearchByMedicationAsync(medication);
            return allPrescriptions.Select(p => new PrescriptionReadDto { Id = p.Id, RecordId = p.RecordId, Medication = p.Medication, Dosage = p.Dosage, Instructions = p.Instructions });
        }

        public async Task<PrescriptionReadDto> CreateAsync(PrescriptionCreateDto dto)
        {
            Prescription prescription = new Prescription { RecordId = dto.RecordId, Medication = dto.Medication, Dosage = dto.Dosage, Instructions = dto.Instructions };

            await repository.AddAsync(prescription);
            await repository.SaveAsync();

            PrescriptionReadDto result = new PrescriptionReadDto { Id = prescription.Id, RecordId = prescription.RecordId, Medication = prescription.Medication, Dosage = prescription.Dosage, Instructions = prescription.Instructions };
            return result;
        }

        public async Task<PrescriptionReadDto> UpdateAsync(int id, PrescriptionUpdateDto dto)
        {
            Prescription prescription = await repository.GetByIdAsync(id);
            if (prescription == null)
            {
                throw new KeyNotFoundException($"Призначення з таким {id} не знайдено.");
            }

            prescription.Medication = dto.Medication;
            prescription.Dosage = dto.Dosage;
            prescription.Instructions = dto.Instructions;

            await repository.UpdateAsync(prescription);
            await repository.SaveAsync();

            PrescriptionReadDto result = new PrescriptionReadDto { Id = prescription.Id, RecordId = prescription.RecordId, Medication = prescription.Medication, Dosage = prescription.Dosage, Instructions = prescription.Instructions };
            return result;
        }

        public async Task DeleteAsync(int id)
        {
            Prescription prescription = await repository.GetByIdAsync(id);
            if (prescription == null)
            {
                throw new KeyNotFoundException($"Призначення з таким {id} не знайдено.");
            }

            await repository.DeleteAsync(prescription);
            await repository.SaveAsync();
        }

        public async Task<IEnumerable<PrescriptionReadDto>> GetPagedAsync(SortingParameters parameters)
        {
            var allPrescriptions = await repository.GetAllAsync();
            IQueryable<Prescription> sorted = allPrescriptions.AsQueryable();

            if (parameters.SortBy != null)
            {
                string sortBy = parameters.SortBy.ToLower();

                if (sortBy == "medication")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(p => p.Medication);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(p => p.Medication);
                    }
                }
                else if (sortBy == "dosage")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(p => p.Dosage);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(p => p.Dosage);
                    }
                }
                else if (sortBy == "recordid")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(p => p.RecordId);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(p => p.RecordId);
                    }
                }
                else if (sortBy == "instructions")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(p => p.Instructions);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(p => p.Instructions);
                    }
                }
                else
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(p => p.Id);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(p => p.Id);
                    }
                }
            }
            else
            {
                if (parameters.IsDescending)
                {
                    sorted = sorted.OrderByDescending(p => p.Id);
                }
                else
                {
                    sorted = sorted.OrderBy(p => p.Id);
                }
            }

            return Paginate(sorted, parameters.Page, parameters.PageSize);

        }

        public async Task<IEnumerable<PrescriptionReadDto>> GetFilteredAsync(PrescriptionFilterDto filter)
        {
            var allPrescriptions = await repository.GetAllAsync();
            IQueryable<Prescription> filtered = allPrescriptions.AsQueryable();

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
                .Select(p => new PrescriptionReadDto { Id = p.Id, RecordId = p.RecordId, Medication = p.Medication, Dosage = p.Dosage, Instructions = p.Instructions })
                .ToList();
        }

        public async Task<IEnumerable<PrescriptionReadDto>> GetByPatientIdAsync(int patientId)
        {
            var prescriptions = await repository.GetByPatientIdAsync(patientId);
            return prescriptions.Select(p => new PrescriptionReadDto{ Id = p.Id,RecordId = p.RecordId, Medication = p.Medication, Dosage = p.Dosage, Instructions = p.Instructions }).ToList();
        }


        public async Task<IEnumerable<PrescriptionReadDto>> GetByDoctorIdAsync(int doctorId)
        {
            var prescriptions = await repository.GetByDoctorIdAsync(doctorId);
            return prescriptions.Select(p => new PrescriptionReadDto { Id = p.Id, RecordId = p.RecordId, Medication = p.Medication, Dosage = p.Dosage, Instructions = p.Instructions}).ToList();
        }
    }
}