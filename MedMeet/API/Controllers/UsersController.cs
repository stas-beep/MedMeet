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
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
        {
            var users = await userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserReadDto>> GetById(int id)
        {
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userID, out var currentUserId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }
                if (userRole == "Admin")
                {
                    var user = await userService.GetByIdAsync(id);
                    return Ok(user);
                }
                if (currentUserId != id)
                {
                    return Forbid("Ви можете переглядати тільки свій профіль");
                }

                UserReadDto currentUser = await userService.GetByIdAsync(id);
                return Ok(currentUser);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Користувач з таким id ({id}) не знайдений.");
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserReadDto>> GetCurrentUser()
        {
            try
            {
                string currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(currentUserIdStr, out var currentUserId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }

                UserReadDto user = await userService.GetByIdAsync(currentUserId);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Ваш профіль не знайдений.");
            }
        }

        [HttpGet("doctors")]
        [Authorize]
        public async Task<ActionResult<List<UserReadDto>>> GetDoctors()
        {
            var doctors = await userService.GetDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("patients")]
        [Authorize(Roles = "Admin,Doctor")] 
        public async Task<ActionResult<List<UserReadDto>>> GetPatients()
        {
            var patients = await userService.GetPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("email/{email}")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<UserReadDto>> GetByEmail(string email)
        {
            try
            {
                UserReadDto user = await userService.GetByEmailAsync(email);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Користувач з таким email ({email}) не знайдений.");
            }
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<List<UserReadDto>>> SearchByName([FromQuery] string name)
        {
            var users = await userService.SearchByNameAsync(name);
            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserReadDto>> Create([FromBody] UserCreateDto dto)
        {
            UserReadDto createdUser = await userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserReadDto>> Update(int id, [FromBody] UserUpdateDto dto)
        {
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userID, out var currentUserId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }
                if (userRole == "Admin")
                {
                    var updated = await userService.UpdateAsync(id, dto, userID, userRole);
                    return Ok(updated);
                }
                if (currentUserId != id)
                {
                    return Forbid("Ви можете оновлювати тільки свій профіль");
                }

                var updatedUser = await userService.UpdateAsync(id, dto, userID, userRole);
                return Ok(updatedUser);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Користувач з таким id ({id}) не знайдений.");
            }
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<ActionResult<UserReadDto>> UpdateCurrentUser([FromBody] UserUpdateDto dto)
        {
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userID, out var currentUserId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }

                var updated = await userService.UpdateAsync(currentUserId, dto, userID, userRole);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Ваш профіль не знайдений.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await userService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Користувач з таким id ({id}) не знайдений.");
            }
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<IEnumerable<UserReadDto>>> GetPaged([FromQuery] SortingParameters parameters)
        {
            var pagedUsers = await userService.GetPagedAsync(parameters);
            return Ok(pagedUsers);
        }

        [HttpGet("filter")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetFiltered([FromQuery] UserFilterDto filter)
        {
            var records = await userService.GetFilteredAsync(filter);
            return Ok(records);
        }
    }
}