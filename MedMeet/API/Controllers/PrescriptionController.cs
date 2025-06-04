using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Prescription;
using Business_logic.Filters;
using Business_logic.Services.Interfaces;
using Business_logic.Sorting;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/prescription")]
    [Authorize]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _service;

        public PrescriptionController(IPrescriptionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrescriptionReadDto>>> GetAll()
        {
            var prescriptions = await _service.GetAllAsync();
            return Ok(prescriptions); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionReadDto>> GetById(int id)
        {
            try
            {
                PrescriptionReadDto prescription = await _service.GetByIdAsync(id);
                return Ok(prescription);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти призначення з таким id ({id})");
            }
        }

        [HttpGet("record/{recordId}")]
        public async Task<ActionResult<List<PrescriptionReadDto>>> GetByRecordId(int recordId)
        {
            var prescriptions = await _service.GetByRecordIdAsync(recordId);
            return Ok(prescriptions);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<PrescriptionReadDto>>> SearchByMedication([FromQuery] string medication)
        {
            var prescriptions = await _service.SearchByMedicationAsync(medication);
            return Ok(prescriptions);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<PrescriptionReadDto>> Create([FromBody] PrescriptionCreateDto dto)
        {
            var createdPrescription = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdPrescription.Id }, createdPrescription);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<PrescriptionReadDto>> Update(int id, [FromBody] PrescriptionUpdateDto dto)
        {
            try
            {
                var updatedPrescription = await _service.UpdateAsync(id, dto);
                return Ok(updatedPrescription);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти призначення з таким id ({id})");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent(); 
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти призначення з таким id ({id})");

            }
        }

        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<PrescriptionReadDto>>> GetPaged([FromQuery] SortingParameters parameters)
        {
            var result = await _service.GetPagedAsync(parameters);
            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFiltered([FromQuery] PrescriptionFilterDto filter)
        {
            var prescriptions = await _service.GetFilteredAsync(filter);
            return Ok(prescriptions);
        }
    }
}
