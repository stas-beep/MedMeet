using System.Security.Claims;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Users;
using Business_logic.Filters;
using Business_logic.Services.Implementation;
using Business_logic.Services.Interfaces;
using Business_logic.Sorting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserReadDto>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Користувач з таким id ({id}) не знайдений.");
            }
        }

        [HttpGet("doctors")]
        [Authorize]
        public async Task<ActionResult<List<UserReadDto>>> GetDoctors()
        {
            var doctors = await _userService.GetDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("patients")]
        [Authorize]
        public async Task<ActionResult<List<UserReadDto>>> GetPatients()
        {
            var patients = await _userService.GetPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<ActionResult<UserReadDto>> GetByEmail(string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Користувач з таким email ({email}) не знайдений.");
            }
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<ActionResult<List<UserReadDto>>> SearchByName([FromQuery] string name)
        {
            var users = await _userService.SearchByNameAsync(name);
            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<UserReadDto>> Create([FromBody] UserCreateDto dto)
        {
            var createdUser = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserReadDto>> Update(int id, [FromBody] UserUpdateDto dto)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var updated = await _userService.UpdateAsync(id, dto, currentUserId, currentUserRole);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("Ви не маєте прав оновлювати цього користувача.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Користувач з таким id ({id}) не знайдений.");
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Користувач з таким id ({id}) не знайдений.");
            }
        }

        [HttpGet("paged")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetPaged([FromQuery] SortingParameters parameters)
        {
            var pagedUsers = await _userService.GetPagedAsync(parameters);
            return Ok(pagedUsers);
        }

        [HttpGet("filter")]
        [Authorize]
        public async Task<IActionResult> GetFiltered([FromQuery] UserFilterDto filter)
        {
            var records = await _userService.GetFilteredAsync(filter);
            return Ok(records);
        }
    }
}