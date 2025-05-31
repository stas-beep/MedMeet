using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Record;
using Business_logic.Services.Interfaces;
using Database.Generic_Repository.Interfaces;
using Database.Models;

public class RecordService : IRecordService
{
    private IRecordRepository repository;

    public RecordService(IRecordRepository recordRepository)
    {
        repository = recordRepository;
    }

    public async Task<IEnumerable<RecordReadDto>> GetAllAsync()
    {
        var records = await repository.GetAllAsync();
        return records.Select(record => new RecordReadDto { Id = record.Id, PatientId = record.PatientId, PatientName = record.Patient?.FullName ?? "Unknown", DoctorId = record.DoctorId, DoctorName = record.Doctor?.FullName ?? "Unknown", AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes });
    }

    public async Task<RecordReadDto> GetByIdAsync(int id)
    {
        var record = await repository.GetWithDetailsAsync(id);
        if (record == null)
        {
            throw new KeyNotFoundException($"Record with id {id} not found.");
        }

        return new RecordReadDto { Id = record.Id, PatientId = record.PatientId, PatientName = record.Patient?.FullName ?? "Unknown", DoctorId = record.DoctorId, DoctorName = record.Doctor?.FullName ?? "Unknown", AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes };
    }

    public async Task<IEnumerable<RecordReadDto>> GetByPatientIdAsync(int patientId)
    {
        var records = await repository.GetByPatientIdAsync(patientId);
        return records.Select(record => new RecordReadDto { Id = record.Id, PatientId = record.PatientId, PatientName = record.Patient?.FullName ?? "Unknown", DoctorId = record.DoctorId, DoctorName = record.Doctor?.FullName ?? "Unknown", AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes });
    }

    public async Task<IEnumerable<RecordReadDto>> GetByDoctorIdAsync(int doctorId)
    {
        var records = await repository.GetByDoctorIdAsync(doctorId);
        return records.Select(record => new RecordReadDto { Id = record.Id, PatientId = record.PatientId, PatientName = record.Patient?.FullName ?? "Unknown", DoctorId = record.DoctorId, DoctorName = record.Doctor?.FullName ?? "Unknown", AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes });
    }

    public async Task<IEnumerable<RecordReadDto>> GetByStatusAsync(string status)
    {
        var records = await repository.GetByStatusAsync(status);
        return records.Select(record => new RecordReadDto { Id = record.Id, PatientId = record.PatientId, PatientName = record.Patient?.FullName ?? "Unknown", DoctorId = record.DoctorId, DoctorName = record.Doctor?.FullName ?? "Unknown", AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes });
    }

    public async Task<IEnumerable<RecordReadDto>> GetUpcomingAsync(DateTime from, DateTime? to = null)
    {
        var records = await repository.GetUpcomingAsync(from, to);
        return records.Select(record => new RecordReadDto { Id = record.Id, PatientId = record.PatientId, PatientName = record.Patient?.FullName ?? "Unknown", DoctorId = record.DoctorId, DoctorName = record.Doctor?.FullName ?? "Unknown", AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes });
    }

    public async Task<RecordReadDto> CreateAsync(RecordCreateDto dto)
    {
        Record record = new Record { PatientId = dto.PatientId, DoctorId = dto.DoctorId, AppointmentDate = dto.AppointmentDate, Status = dto.Status, Notes = dto.Notes };

        await repository.AddAsync(record);
        await repository.SaveAsync();

        var saved = await repository.GetWithDetailsAsync(record.Id);
        return new RecordReadDto { Id = saved.Id, PatientId = saved.PatientId, PatientName = saved.Patient?.FullName ?? "Unknown", DoctorId = saved.DoctorId, DoctorName = saved.Doctor?.FullName ?? "Unknown", AppointmentDate = saved.AppointmentDate, Status = saved.Status, Notes = saved.Notes };
    }

    public async Task UpdateAsync(int id, RecordUpdateDto dto)
    {
        var record = await repository.GetByIdAsync(id);
        if (record == null)
        {
            throw new KeyNotFoundException($"Record with id {id} not found.");
        }

        record.AppointmentDate = dto.AppointmentDate;
        record.Status = dto.Status;
        record.Notes= dto.Notes;

        await repository.UpdateAsync(record);
        await repository.SaveAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var record = await repository.GetByIdAsync(id);
        if (record == null)
        {
            throw new KeyNotFoundException($"Record with id {id} not found.");
        }

        await repository.DeleteAsync(record);
        await repository.SaveAsync();
    }
}