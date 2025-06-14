using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Speciality;
using Business_logic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/specialties")]
    public class SpecialtiesController : ControllerBase
    {
        private ISpecialtyService specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            this.specialtyService = specialtyService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SpecialtyReadDto>>> GetAll()
        {
            var specialties = await specialtyService.GetAllAsync();
            return Ok(specialties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpecialtyReadDto>> GetById(int id)
        {
            try
            {
                SpecialtyReadDto specialty = await specialtyService.GetByIdAsync(id);
                return Ok(specialty);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Спеціальність з такою id ({id}) не знайдена.");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SpecialtyReadDto>>> SearchByName([FromQuery] string name)
        {
            var specialties = await specialtyService.SearchByNameAsync(name);
            return Ok(specialties);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SpecialtyReadDto>> Create([FromBody] SpecialtyCreateDto dto)
        {
            SpecialtyReadDto created = await specialtyService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SpecialtyReadDto>> Update(int id, [FromBody] SpecialtyUpdateDto dto)
        {
            try
            {
                SpecialtyReadDto updated = await specialtyService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Спеціальність з такою id ({id}) не знайдена.");

            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await specialtyService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Спеціальність з такою id ({id}) не знайдена.");
            }
        }
    }
}
