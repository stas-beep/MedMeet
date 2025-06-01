using Business_logic.Data_Transfer_Object.For_Users;
using Business_logic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserReadDto>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with id {id} not found.");
            }
        }

        [HttpGet("doctors")]
        public async Task<ActionResult<List<UserReadDto>>> GetDoctors()
        {
            var doctors = await _userService.GetDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("patients")]
        public async Task<ActionResult<List<UserReadDto>>> GetPatients()
        {
            var patients = await _userService.GetPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserReadDto>> GetByEmail(string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with email {email} not found.");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<UserReadDto>>> SearchByName([FromQuery] string name)
        {
            var users = await _userService.SearchByNameAsync(name);
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<UserReadDto>> Create([FromBody] UserCreateDto dto)
        {
            var createdUser = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserReadDto>> Update(int id, [FromBody] UserUpdateDto dto)
        {
            try
            {
                var updated = await _userService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with id {id} not found.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with id {id} not found.");
            }
        }
    }
}
