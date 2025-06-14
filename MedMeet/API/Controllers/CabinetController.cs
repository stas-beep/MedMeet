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
    public class CabinetController : ControllerBase
    {
        private ICabinetService service;

        public CabinetController(ICabinetService cabinetService)
        {
            service = cabinetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var cabinets = await service.GetAllAsync();
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
            CabinetReadDto cabinet = await service.GetByIdAsync(id);
            if (cabinet == null)
            {
                return NotFound($"Кабінет з таким id ({id}) не знайдений!");
            }
            return Ok(cabinet);
        }

        [HttpGet("by-name/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var cabinet = await service.GetByNameAsync(name);
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
            CabinetReadDto createdCabinet = await service.CreateAsync(dto);
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
                var updatedCabinet = await service.UpdateAsync(id, dto);
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
                await service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}