using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Record;
using Business_logic.Filters;
using Business_logic.Services.Interfaces;
using Business_logic.Sorting;
using Database.Generic_Repository;
using Database.Generic_Repository.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business_logic.Services.Implementation
{
    public class RecordService : IRecordService
    {
        private IRecordRepository repository;
        private IHttpContextAccessor httpContextAccessor;
        private UserManager<User> userManager;
        private IUserRepository userRepository;
        private static readonly TimeSpan MaxAppointmentTime = new TimeSpan(17, 30, 0);

        public RecordService(IRecordRepository recordRepository, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IUserRepository userRepository)
        {
            repository = recordRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
            this.userRepository = userRepository;
        }

        private void ValidateAppointmentTime(DateTime appointmentDate)
        {
            TimeSpan appointmentTime = appointmentDate.TimeOfDay;
            if (appointmentTime > MaxAppointmentTime)
            {
                throw new InvalidOperationException("Записи на прийоми можна створювати тільки до 17:30. Останній прийом о 17:30.");
            }
        }

        private int GetCurrentUserId()
        {
            string userID = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userID, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Користувач не автентифікований.");
        }

        public async Task<IEnumerable<RecordReadDto>> GetAllAsync()
        {
            var allRecords = await repository.GetAllWithDetailsAsync();
            return allRecords.Select(p => new RecordReadDto{ Id = p.Id, PatientId = p.PatientId, DoctorId = p.DoctorId, DoctorName = p.Doctor.FullName, AppointmentDate = p.AppointmentDate, Status = p.Status, Notes = p.Notes, }).ToList();
        }

        public async Task<RecordReadDto> GetByIdAsync(int id)
        {
            Record record = await repository.GetWithDetailsAsync(id);
            if (record == null)
            {
                throw new KeyNotFoundException($"Запис з таким id ({id}) не знайдено.");
            }

            RecordReadDto result = new RecordReadDto { Id = record.Id, PatientId = record.PatientId, DoctorId = record.DoctorId, DoctorName = record.Doctor.FullName, AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes };

            return result;
        }

        public async Task<IEnumerable<RecordReadDto>> GetMyPatientRecordsAsync()
        {
            int userID = GetCurrentUserId();
            var records = await repository.GetByPatientIdAsync(userID);

            return records.Select(p => new RecordReadDto { Id = p.Id, PatientId = p.PatientId, DoctorId = p.DoctorId, DoctorName = p.Doctor.FullName, AppointmentDate = p.AppointmentDate, Status = p.Status, Notes = p.Notes, }).ToList();
        }

        public async Task<IEnumerable<RecordReadDto>> GetMyDoctorRecordsAsync()
        {
            int userID = GetCurrentUserId();
            var doctorRecords = await repository.GetByDoctorIdAsync(userID);

            return doctorRecords.Select(p => new RecordReadDto { Id = p.Id, PatientId = p.PatientId, DoctorId = p.DoctorId, DoctorName = p.Doctor.FullName, AppointmentDate = p.AppointmentDate, Status = p.Status, Notes = p.Notes, }).ToList();
        }

        public async Task<IEnumerable<RecordReadDto>> GetByStatusAsync(string status)
        {
            var allRecords = await repository.GetByStatusAsync(status);
            return allRecords.Select(p => new RecordReadDto { Id = p.Id, PatientId = p.PatientId, DoctorId = p.DoctorId, DoctorName = p.Doctor.FullName, AppointmentDate = p.AppointmentDate, Status = p.Status, Notes = p.Notes, }).ToList();
        }

        public async Task<IEnumerable<RecordReadDto>> GetFilteredAsync(RecordFilterDto filter)
        {
            var allRecords = await repository.GetAllAsync();
            IQueryable<Record> filtered = allRecords.AsQueryable();

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
            var allRecords = await repository.GetUpcomingAsync(from, to);
            return allRecords.Select(p => new RecordReadDto
            {
                Id = p.Id,
                PatientId = p.PatientId,
                DoctorId = p.DoctorId,
                DoctorName = p.Doctor.FullName,
                AppointmentDate = p.AppointmentDate,
                Status = p.Status,
                Notes = p.Notes,
            }).ToList();
        }

        public async Task<RecordReadDto> CreateAsync(RecordCreateDto dto, int patientId)
        {
            ValidateAppointmentTime(dto.AppointmentDate);

            User doctor = await userRepository.GetByIdAsync(dto.DoctorId);
            if (doctor == null)
            {
                throw new KeyNotFoundException($"Лікар з id {dto.DoctorId} не знайдений.");
            }

            if (!doctor.CabinetId.HasValue)
            {
                throw new InvalidOperationException("У лікаря не вказаний кабінет.");
            }

            int cabinetId = doctor.CabinetId.Value;
            DateTime start = dto.AppointmentDate.AddMinutes(-30);
            DateTime end = dto.AppointmentDate.AddMinutes(30);
            var conflictingRecords = await repository.GetByCabinetIdAndDateRangeAsync(cabinetId, start, end);

            if (conflictingRecords.Any())
            {
                throw new InvalidOperationException("У цей час кабінет вже зайнятий. Оберіть інший час.");
            }

            Record record = new Record { PatientId = patientId, DoctorId = dto.DoctorId, AppointmentDate = dto.AppointmentDate, Status = dto.Status, Notes = dto.Notes };

            await repository.AddAsync(record);
            await repository.SaveAsync();

            string patientName = await GetUserFullNameById(patientId);
            string doctorName = await GetUserFullNameById(dto.DoctorId);

            return new RecordReadDto { Id = record.Id, PatientId = patientId, DoctorId = dto.DoctorId, DoctorName = doctorName, AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes };
        }

        private async Task<string?> GetUserFullNameById(int userId)
        {
            User user = await userRepository.GetByIdAsync(userId);
            return user?.UserName;
        }

        public async Task<RecordReadDto> UpdateAsync(int id, RecordUpdateDto dto)
        {
            ValidateAppointmentTime(dto.AppointmentDate);

            Record record = await repository.GetByIdAsync(id);
            if (record == null)
            {
                throw new KeyNotFoundException($"Запис з таким id ({id}) не знайдено.");
            }

            User doctor = await userRepository.GetByIdAsync(record.DoctorId);
            if (doctor == null)
            {
                throw new KeyNotFoundException($"Лікар з id {record.DoctorId} не знайдений.");
            }

            if (!doctor.CabinetId.HasValue)
            {
                throw new InvalidOperationException("У лікаря не вказаний кабінет.");
            }

            int cabinetID = doctor.CabinetId.Value;
            DateTime start = dto.AppointmentDate.AddMinutes(-30);
            DateTime end = dto.AppointmentDate.AddMinutes(30);
            var conflictingRecords = await repository.GetByCabinetIdAndDateRangeAsync(cabinetID, start, end);
            
            if (conflictingRecords.Any(r => r.Id != id))
            {
                throw new InvalidOperationException("У цей час кабінет вже зайнятий. Оберіть інший час.");
            }

            record.AppointmentDate = dto.AppointmentDate;
            record.Status = dto.Status;
            record.Notes = dto.Notes;

            await repository.UpdateAsync(record);
            await repository.SaveAsync();
            Record updated = await repository.GetWithDetailsAsync(record.Id);
            RecordReadDto result = new RecordReadDto { Id = record.Id, PatientId = record.PatientId, DoctorId = record.DoctorId, DoctorName = record.Doctor.FullName, AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes };

            return result;
        }

        public async Task<IEnumerable<RecordReadDto>> GetPagedAsync(SortingParameters parameters)
        {
            var allRecords = await repository.GetAllWithDetailsAsync();
            IQueryable<Record> sorted = allRecords.AsQueryable();
            if (parameters.SortBy != null)
            {
                string sortBy = parameters.SortBy.ToLower();
                if (sortBy == "appointmentdate")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(r => r.AppointmentDate);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(r => r.AppointmentDate);
                    }
                }
                else if (sortBy == "status")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(r => r.Status);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(r => r.Status);
                    }
                }
                else if (sortBy == "doctorname")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(r => r.Doctor.FullName);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(r => r.Doctor.FullName);
                    }
                }
                else if (sortBy == "patientname")
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(r => r.Patient.FullName);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(r => r.Patient.FullName);
                    }
                }
                else
                {
                    if (parameters.IsDescending)
                    {
                        sorted = sorted.OrderByDescending(r => r.Id);
                    }
                    else
                    {
                        sorted = sorted.OrderBy(r => r.Id);
                    }
                }
            }
            else
            {
                if (parameters.IsDescending)
                {
                    sorted = sorted.OrderByDescending(r => r.Id);
                }
                else
                {
                    sorted = sorted.OrderBy(r => r.Id);
                }
            }

            return Paginate(sorted, parameters.Page, parameters.PageSize);

        }

        public async Task DeleteAsync(int id)
        {
            Record record = await repository.GetByIdAsync(id);
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
                .Select(record => new RecordReadDto{ Id = record.Id, PatientId = record.PatientId, DoctorId = record.DoctorId, DoctorName = record.Doctor?.FullName, AppointmentDate = record.AppointmentDate, Status = record.Status, Notes = record.Notes })
                .ToList();
        }

        public async Task<RecordReadDto> CancelRecordAsync(int recordId, int patientId)
        {
            Record record = await repository.GetByIdAsync(recordId);
            
            if (record == null)
            {
                throw new KeyNotFoundException($"Запис з таким id ({recordId}) не знайдено.");
            }

            if (record.PatientId != patientId)
            {
                throw new UnauthorizedAccessException("Ви можете скасовувати тільки свої записи.");
            }

            if (record.AppointmentDate < DateTime.Now)
            {
                throw new InvalidOperationException("Неможливо скасувати запис, який вже відбувся.");
            }

            if (record.Status == "Cancelled")
            {
                throw new InvalidOperationException("Запис вже скасований.");
            }

            record.Status = "Cancelled";

            await repository.UpdateAsync(record);
            await repository.SaveAsync();

            Record updated = await repository.GetWithDetailsAsync(record.Id);
            RecordReadDto result = new RecordReadDto
            {
                Id = updated.Id,
                PatientId = updated.PatientId,
                DoctorId = updated.DoctorId,
                DoctorName = updated.Doctor.FullName,
                AppointmentDate = updated.AppointmentDate,
                Status = updated.Status,
                Notes = updated.Notes
            };

            return result;
        }
    }
}