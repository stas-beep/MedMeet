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
    public class RecordsController: ControllerBase
    {
        private readonly IRecordService _recordService;
        private readonly UserManager<User> _userManager;

        public RecordsController(IRecordService recordService, UserManager<User> userManager)
        {
            _recordService = recordService;
            _userManager = userManager;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RecordReadDto>>> GetAll()
        {
            var records = await _recordService.GetAllAsync();
            return Ok(records);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecordReadDto>> GetById(int id)
        {
            try
            {
                var record = await _recordService.GetByIdAsync(id);
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
            var records = await _recordService.GetMyPatientRecordsAsync();
            return Ok(records);
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("doctor/my")]
        public async Task<ActionResult<IEnumerable<RecordReadDto>>> GetMyRecordsAsDoctor()
        {
            var records = await _recordService.GetMyDoctorRecordsAsync();
            return Ok(records);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<List<RecordReadDto>>> GetByStatus(string status)
        {
            var records = await _recordService.GetByStatusAsync(status);
            return Ok(records);
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<List<RecordReadDto>>> GetUpcoming([FromQuery] DateTime from, [FromQuery] DateTime? to)
        {
            var records = await _recordService.GetUpcomingAsync(from, to);
            return Ok(records);
        }

        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<RecordReadDto>> Create([FromBody] RecordCreateDto dto)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized("Не вдалося визначити ID користувача");
            }

            var createdRecord = await _recordService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = createdRecord.Id }, createdRecord);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<RecordReadDto>> Update(int id, [FromBody] RecordUpdateDto dto)
        {
            try
            {
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (!int.TryParse(userIdStr, out var userId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }

                var record = await _recordService.GetByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"Не можемо знайти запис з таким id ({id}).");
                }

                if (role == "Doctor" && record.DoctorId != userId)
                {
                    return Forbid();
                }

                var updated = await _recordService.UpdateAsync(id, dto);
                return Ok(updated);
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
                var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (!int.TryParse(userIdStr, out var userId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }

                var record = await _recordService.GetByIdAsync(id);
                if (record == null)
                {
                    return NotFound($"Не можемо знайти запис з таким id ({id}).");
                }

                if (role == "Doctor" && record.DoctorId != userId)
                {
                    return Forbid();
                }

                await _recordService.DeleteAsync(id);
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
            var pagedResult = await _recordService.GetPagedAsync(parameters);
            return Ok(pagedResult);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] RecordFilterDto filter)
        {
            var records = await _recordService.GetFilteredAsync(filter);
            return Ok(records);
        }
    }
}
