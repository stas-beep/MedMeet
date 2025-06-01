using Business_logic.Data_Transfer_Object.For_Speciality;
using Business_logic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SpecialtyReadDto>>> GetAll()
        {
            var specialties = await _specialtyService.GetAllAsync();
            return Ok(specialties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialtyReadDto>> GetById(int id)
        {
            try
            {
                var specialty = await _specialtyService.GetByIdAsync(id);
                return Ok(specialty);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Specialty with id {id} not found.");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SpecialtyReadDto>>> SearchByName([FromQuery] string name)
        {
            var specialties = await _specialtyService.SearchByNameAsync(name);
            return Ok(specialties);
        }

        [HttpPost]
        public async Task<ActionResult<SpecialtyReadDto>> Create([FromBody] SpecialtyCreateDto dto)
        {
            var created = await _specialtyService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SpecialtyReadDto>> Update(int id, [FromBody] SpecialtyUpdateDto dto)
        {
            try
            {
                var updated = await _specialtyService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Specialty with id {id} not found.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _specialtyService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Specialty with id {id} not found.");
            }
        }
    }
}
