using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Prescription;
using Business_logic.Services.Interfaces;
using Database.Generic_Repository.Interfaces;
using Database.Models;

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
            throw new KeyNotFoundException($"Prescription with id {id} not found.");
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

    public async Task UpdateAsync(int id, PrescriptionUpdateDto dto)
    {
        var prescription = await repository.GetByIdAsync(id);
        if (prescription == null)
        {
            throw new KeyNotFoundException($"Prescription with id {id} not found.");
        }

        prescription.Medication = dto.Medication;
        prescription.Dosage = dto.Dosage;
        prescription.Instructions = dto.Instructions;

        await repository.UpdateAsync(prescription);
        await repository.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var prescription = await repository.GetByIdAsync(id);
        if (prescription == null)
        {
            throw new KeyNotFoundException($"Prescription with id {id} not found.");
        }

        await repository.DeleteAsync(prescription);
        await repository.SaveAsync();
    }
}