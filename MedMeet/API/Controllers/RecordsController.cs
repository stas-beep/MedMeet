using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Record;
using Business_logic.Filters;
using Business_logic.Services.Interfaces;
using Business_logic.Sorting;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/records")]
    [Authorize]
    public class RecordsController : ControllerBase
    {
        private IRecordService recordService;

        public RecordsController(IRecordService recordService)
        {
            this.recordService = recordService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RecordReadDto>>> GetAll()
        {
            var records = await recordService.GetAllAsync();
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecordReadDto>> GetById(int id)
        {
            try
            {
                var record = await recordService.GetByIdAsync(id);
                return Ok(record);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти запис з таким id ({id}).");
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<RecordReadDto>>> GetMyRecordsAsPatient()
        {
            var records = await recordService.GetMyPatientRecordsAsync();
            return Ok(records);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("doctor/my")]
        public async Task<ActionResult<IEnumerable<RecordReadDto>>> GetMyRecordsAsDoctor()
        {
            var records = await recordService.GetMyDoctorRecordsAsync();
            return Ok(records);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<RecordReadDto>>> GetByStatus(string status)
        {
            var records = await recordService.GetByStatusAsync(status);
            return Ok(records);
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<List<RecordReadDto>>> GetUpcoming([FromQuery] DateTime from, [FromQuery] DateTime? to)
        {
            var records = await recordService.GetUpcomingAsync(from, to);
            return Ok(records);
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<RecordReadDto>> Create([FromBody] RecordCreateDto dto)
        {
            string userID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userID) || !int.TryParse(userID, out var userId))
            {
                return Unauthorized("Не вдалося визначити ID користувача");
            }

            var createdRecord = await recordService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = createdRecord.Id }, createdRecord);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<ActionResult<RecordReadDto>> Update(int id, [FromBody] RecordUpdateDto dto)
        {
            try
            {
                string userID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (!int.TryParse(userID, out var userId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }

                RecordReadDto record = await recordService.GetByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"Не можемо знайти запис з таким id ({id}).");
                }
                if (role == "Doctor" && record.DoctorId != userId)
                {
                    return Forbid("Ви можете редагувати тільки свої записи як лікар");
                }
                if (role == "Patient" && record.PatientId != userId)
                {
                    return Forbid("Ви можете редагувати тільки свої записи як пацієнт");
                }
                if (role == "Patient")
                {
                    if (dto.Status != "Cancelled")
                    {
                        return BadRequest("Пацієнт може тільки скасовувати записи (статус 'Cancelled')");
                    }
                    if (record.AppointmentDate < DateTime.Now)
                    {
                        return BadRequest("Неможливо скасувати запис, який вже відбувся");
                    }
                    if (record.Status == "Cancelled")
                    {
                        return BadRequest("Запис вже скасований");
                    }
                    var cancelledRecord = await recordService.CancelRecordAsync(id, userId);
                    return Ok(cancelledRecord);
                }
                var updated = await recordService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти запис з таким id ({id}).");
            }
        }

        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<RecordReadDto>> CancelRecord(int id)
        {
            try
            {
                string userID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userID, out var userId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }

                RecordReadDto record = await recordService.GetByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"Не можемо знайти запис з таким id ({id}).");
                }

                if (record.PatientId != userId)
                {
                    return Forbid("Ви можете скасовувати тільки свої записи");
                }

                if (record.AppointmentDate < DateTime.Now)
                {
                    return BadRequest("Неможливо скасувати запис, який вже відбувся");
                }

                if (record.Status == "Cancelled")
                {
                    return BadRequest("Запис вже скасований");
                }

                RecordReadDto cancelledRecord = await recordService.CancelRecordAsync(id, userId);
                return Ok(cancelledRecord);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти запис з таким id ({id}).");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                string userID = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (!int.TryParse(userID, out var userId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }

                var record = await recordService.GetByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"Не можемо знайти запис з таким id ({id}).");
                }

                if (role == "Doctor" && record.DoctorId != userId)
                {
                    return Forbid();
                }

                await recordService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти запис з таким id ({id}).");
            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<RecordReadDto>>> GetPaged([FromQuery] SortingParameters parameters)
        {
            var pagedResult = await recordService.GetPagedAsync(parameters);
            return Ok(pagedResult);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] RecordFilterDto filter)
        {
            var records = await recordService.GetFilteredAsync(filter);
            return Ok(records);
        }
    }
}