using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Record;
using Business_logic.Filters;
using Business_logic.Services.Interfaces;
using Database.Generic_Repository.Interfaces;
using Database.Models;

namespace Business_logic.Services.Implementation
{
    public class RecordService : IRecordService
    {
        private IRecordRepository repository;

        public RecordService(IRecordRepository recordRepository)
        {
            repository = recordRepository;
        }

        public async Task<IEnumerable<RecordReadDto>> GetAllAsync()
        {
            var records = await repository.GetAllWithDetailsAsync();
            return records.Select(p => new RecordReadDto
            {
                Id = p.Id,
                PatientId = p.PatientId,
                PatientName = p.Patient.FullName,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor.FullName,
                AppointmentDate = p.AppointmentDate,
                Status = p.Status,
                Notes = p.Notes,
            }).ToList();
        }

        public async Task<RecordReadDto> GetByIdAsync(int id)
        {
            var record = await repository.GetWithDetailsAsync(id);
            if (record == null)
            {
                throw new KeyNotFoundException($"Запис з таким id ({id}) не знайдено.");
            }

            return new RecordReadDto
            {
                Id = record.Id,
                PatientId = record.PatientId,
                PatientName = record.Patient.FullName,
                DoctorId = record.DoctorId,
                DoctorName = record.Doctor.FullName,
                AppointmentDate = record.AppointmentDate,
                Status = record.Status,
                Notes = record.Notes
            };
        }

        public async Task<IEnumerable<RecordReadDto>> GetByPatientIdAsync(int patientId)
        {
            var records = await repository.GetByPatientIdAsync(patientId);
            return records.Select(p => new RecordReadDto
            {
                Id = p.Id,
                PatientId = p.PatientId,
                PatientName = p.Patient.FullName,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor.FullName,
                AppointmentDate = p.AppointmentDate,
                Status = p.Status,
                Notes = p.Notes,
            }).ToList();
        }

        public async Task<IEnumerable<RecordReadDto>> GetByDoctorIdAsync(int doctorId)
        {
            var records = await repository.GetByDoctorIdAsync(doctorId);
            return records.Select(p => new RecordReadDto
            {
                Id = p.Id,
                PatientId = p.PatientId,
                PatientName = p.Patient.FullName,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor.FullName,
                AppointmentDate = p.AppointmentDate,
                Status = p.Status,
                Notes = p.Notes,
            }).ToList();
        }

        public async Task<IEnumerable<RecordReadDto>> GetByStatusAsync(string status)
        {
            var records = await repository.GetByStatusAsync(status);
            return records.Select(p => new RecordReadDto
            {
                Id = p.Id,
                PatientId = p.PatientId,
                PatientName = p.Patient.FullName,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor.FullName,
                AppointmentDate = p.AppointmentDate,
                Status = p.Status,
                Notes = p.Notes,
            }).ToList();
        }

        public async Task<IEnumerable<RecordReadDto>> GetFilteredAsync(RecordFilterDto filter)
        {
            var allRecords = await repository.GetAllAsync();
            var filtered = allRecords.AsQueryable();

            if (filter.PatientId.HasValue)
            {
                filtered = filtered.Where(r => r.PatientId == filter.PatientId.Value);
            }

            if (filter.DoctorId.HasValue)
            {
                filtered = filtered.Where(r => r.DoctorId == filter.DoctorId.Value);
            }

            if (filter.AppointmentDateFrom.HasValue)
            {
                filtered = filtered.Where(r => r.AppointmentDate >= filter.AppointmentDateFrom.Value);
            }

            if (filter.AppointmentDateTo.HasValue)
            {
                filtered = filtered.Where(r => r.AppointmentDate <= filter.AppointmentDateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                filtered = filtered.Where(r => r.Status.Contains(filter.Status, StringComparison.OrdinalIgnoreCase));
            }

            return Paginate(filtered, filter.Page, filter.PageSize);
        }

        public async Task<IEnumerable<RecordReadDto>> GetUpcomingAsync(DateTime from, DateTime? to = null)
        {
            var records = await repository.GetUpcomingAsync(from, to);
            return records.Select(p => new RecordReadDto
            {
                Id = p.Id,
                PatientId = p.PatientId,
                PatientName = p.Patient.FullName,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor.FullName,
                AppointmentDate = p.AppointmentDate,
                Status = p.Status,
                Notes = p.Notes,
            }).ToList();
        }

        public async Task<RecordReadDto> CreateAsync(RecordCreateDto dto)
        {
            Record record = new Record
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                Status = dto.Status,
                Notes = dto.Notes
            };

            await repository.AddAsync(record);
            await repository.SaveAsync();

            var saved = await repository.GetWithDetailsAsync(record.Id);
            return new RecordReadDto
            {
                Id = saved.Id,
                PatientId = saved.PatientId,
                PatientName = saved.Patient.FullName,
                DoctorId = saved.DoctorId,
                DoctorName = saved.Doctor.FullName,
                AppointmentDate = saved.AppointmentDate,
                Status = saved.Status,
                Notes = saved.Notes
            };
        }

        public async Task<RecordReadDto> UpdateAsync(int id, RecordUpdateDto dto)
        {
            var record = await repository.GetByIdAsync(id);
            if (record == null)
            {
                throw new KeyNotFoundException($"Запис з таким id ({id}) не знайдено.");

            }

            record.AppointmentDate = dto.AppointmentDate;
            record.Status = dto.Status;
            record.Notes = dto.Notes;

            await repository.UpdateAsync(record);
            await repository.SaveAsync();

            var updated = await repository.GetWithDetailsAsync(record.Id);
            return new RecordReadDto
            {
                Id = record.Id,
                PatientId = record.PatientId,
                PatientName = record.Patient.FullName,
                DoctorId = record.DoctorId,
                DoctorName = record.Doctor.FullName,
                AppointmentDate = record.AppointmentDate,
                Status = record.Status,
                Notes = record.Notes
            };
        }

        public async Task<IEnumerable<RecordReadDto>> GetPagedAsync(QueryParameters parameters)
        {
            var allRecords = await repository.GetAllWithDetailsAsync();
            return Paginate(allRecords, parameters.Page, parameters.PageSize);
        }

        public async Task DeleteAsync(int id)
        {
            var record = await repository.GetByIdAsync(id);
            if (record == null)
            {
                throw new KeyNotFoundException($"Запис з таким id ({id}) не знайдено.");
            }


            await repository.DeleteAsync(record);
            await repository.SaveAsync();
        }

        private List<RecordReadDto> Paginate(IEnumerable<Record> records, int page, int pageSize)
        {
            return records
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(record => new RecordReadDto
                {
                    Id = record.Id,
                    PatientId = record.PatientId,
                    PatientName = record.Patient?.FullName,
                    DoctorId = record.DoctorId,
                    DoctorName = record.Doctor?.FullName,
                    AppointmentDate = record.AppointmentDate,
                    Status = record.Status,
                    Notes = record.Notes
                })
                .ToList();
        }

    }
}