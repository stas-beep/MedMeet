﻿using Business_logic.Data_Transfer_Object.For_Pagination;
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
        private readonly ISpecialtyService _specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<SpecialtyReadDto>>> GetAll()
        {
            var specialties = await _specialtyService.GetAllAsync();
            return Ok(specialties);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<SpecialtyReadDto>> GetById(int id)
        {
            try
            {
                var specialty = await _specialtyService.GetByIdAsync(id);
                return Ok(specialty);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Спеціальність з такою id ({id}) не знайдена.");
            }
        }

        [HttpGet("search")]
        [AllowAnonymous] 
        public async Task<ActionResult<IEnumerable<SpecialtyReadDto>>> SearchByName([FromQuery] string name)
        {
            var specialties = await _specialtyService.SearchByNameAsync(name);
            return Ok(specialties);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SpecialtyReadDto>> Create([FromBody] SpecialtyCreateDto dto)
        {
            var created = await _specialtyService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SpecialtyReadDto>> Update(int id, [FromBody] SpecialtyUpdateDto dto)
        {
            try
            {
                var updated = await _specialtyService.UpdateAsync(id, dto);
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
                await _specialtyService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Спеціальність з такою id ({id}) не знайдена.");
            }
        }
    }
}
