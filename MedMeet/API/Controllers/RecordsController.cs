using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Record;
using Business_logic.Filters;
using Business_logic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/records")]
    public class RecordsController: ControllerBase
    {
        private readonly IRecordService _recordService;

        public RecordsController(IRecordService recordService)
        {
            _recordService = recordService;
        }

        [HttpGet]
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
        
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<RecordReadDto>>> GetByPatientId(int patientId)
        {
            var records = await _recordService.GetByPatientIdAsync(patientId);
            return Ok(records);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<RecordReadDto>>> GetByDoctorId(int doctorId)
        {
            var records = await _recordService.GetByDoctorIdAsync(doctorId);
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
        public async Task<ActionResult<RecordReadDto>> Create([FromBody] RecordCreateDto dto)
        {
            var createdRecord = await _recordService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdRecord.Id }, createdRecord);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RecordReadDto>> Update(int id, [FromBody] RecordUpdateDto dto)
        {
            try
            {
                var updatedRecord = await _recordService.UpdateAsync(id, dto);
                return Ok(updatedRecord);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти запис з таким id ({id}).");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _recordService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти запис з таким id ({id}).");

            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<RecordReadDto>>> GetPaged([FromQuery] QueryParameters parameters)
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
