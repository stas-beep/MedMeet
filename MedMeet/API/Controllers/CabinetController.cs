using Business_logic.Data_Transfer_Object.For_Cabinets;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Services.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CabinetController : ControllerBase
    {
        private ICabinetService _cabinetService;

        public CabinetController(ICabinetService cabinetService)
        {
            _cabinetService = cabinetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var cabinets = await _cabinetService.GetAllAsync();
                return Ok(cabinets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cabinet = await _cabinetService.GetByIdAsync(id);
            if (cabinet == null)
            {
                return NotFound($"Кабінет з таким id ({id}) не знайдений!");
            }
            return Ok(cabinet);
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var cabinet = await _cabinetService.GetByNameAsync(name);
            if (!cabinet.Any())
            {
                return NotFound($"Кабінет з таким іменем ({name}) не знайдений!");
            }
            return Ok(cabinet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CabinetCreateDto dto)
        {
            var createdCabinet = await _cabinetService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdCabinet.Id }, createdCabinet);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CabinetUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var updatedCabinet = await _cabinetService.UpdateAsync(id, dto);
                return Ok(updatedCabinet);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _cabinetService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}