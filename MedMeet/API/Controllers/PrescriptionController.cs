using System.Security.Claims;
using Business_logic.Data_Transfer_Object.For_Pagination;
using Business_logic.Data_Transfer_Object.For_Prescription;
using Business_logic.Data_Transfer_Object.For_Record;
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
        private IPrescriptionService service;
        private IRecordService recordService;

        public PrescriptionController(IPrescriptionService service, IRecordService recordService)
        {
            this.service = service;
            this.recordService = recordService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PrescriptionReadDto>>> GetAll()
        {
            var prescriptions = await service.GetAllAsync();
            return Ok(prescriptions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionReadDto>> GetById(int id)
        {
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userID, out var currentUserId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }

                PrescriptionReadDto prescription = await service.GetByIdAsync(id);
                if (userRole == "Admin")
                {
                    return Ok(prescription);
                }
                RecordReadDto record = await recordService.GetByIdAsync(prescription.RecordId);
                
                bool hasAccess;
                if (userRole == "Doctor")
                {
                    hasAccess = record.DoctorId == currentUserId;
                }
                else if (userRole == "Patient")
                {
                    hasAccess = record.PatientId == currentUserId;
                }
                else
                {
                    hasAccess = false;
                }

                if (!hasAccess)
                {
                    return Forbid("Ви можете переглядати тільки свої призначення");
                }

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
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                string userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (!int.TryParse(userID, out var currentUserId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }
                var record = await recordService.GetByIdAsync(recordId);
                if (userRole != "Admin")
                {
                    bool hasAccessToRecord;
                    if (userRole == "Doctor")
                    {
                        hasAccessToRecord = record.DoctorId == currentUserId;
                    }
                    else if (userRole == "Patient")
                    {
                        hasAccessToRecord = record.PatientId == currentUserId;
                    }
                    else
                    {
                        hasAccessToRecord = false;
                    }

                    if (!hasAccessToRecord)
                    {
                        return Forbid("Ви можете переглядати призначення тільки зі своїх записів");
                    }
                }

                var prescriptions = await service.GetByRecordIdAsync(recordId);
                return Ok(prescriptions);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти запис з таким id ({recordId})");
            }
        }

        [HttpGet("my/patient")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<List<PrescriptionReadDto>>> GetMyPrescriptionsAsPatient()
        {
            string currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentUserIdStr, out var currentUserId))
            {
                return Unauthorized("Невалідний ідентифікатор користувача");
            }

            var prescriptions = await service.GetByPatientIdAsync(currentUserId);
            return Ok(prescriptions);
        }

        [HttpGet("my/doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<List<PrescriptionReadDto>>> GetMyPrescriptionsAsDoctor()
        {
            string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userID, out var currentUserId))
            {
                return Unauthorized("Невалідний ідентифікатор користувача");
            }

            var prescriptions = await service.GetByDoctorIdAsync(currentUserId);
            return Ok(prescriptions);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Doctor")] 
        public async Task<ActionResult<List<PrescriptionReadDto>>> SearchByMedication([FromQuery] string medication)
        {
            var prescriptions = await service.SearchByMedicationAsync(medication);
            return Ok(prescriptions);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<PrescriptionReadDto>> Create([FromBody] PrescriptionCreateDto dto)
        {
            try
            {
                string userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userID, out var currentUserId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }
                var record = await recordService.GetByIdAsync(dto.RecordId);

                if (record.DoctorId != currentUserId)
                {
                    return Forbid("Ви можете створювати призначення тільки для своїх записів");
                }

                var createdPrescription = await service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdPrescription.Id }, createdPrescription);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти запис з таким id ({dto.RecordId})");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<PrescriptionReadDto>> Update(int id, [FromBody] PrescriptionUpdateDto dto)
        {
            try
            {
                string currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(currentUserIdStr, out var currentUserId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }
                PrescriptionReadDto prescription = await service.GetByIdAsync(id);
                RecordReadDto record = await recordService.GetByIdAsync(prescription.RecordId);

                if (record.DoctorId != currentUserId)
                {
                    return Forbid("Ви можете оновлювати тільки свої призначення");
                }

                var updatedPrescription = await service.UpdateAsync(id, dto);
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
                string currentUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(currentUserIdStr, out var currentUserId))
                {
                    return Unauthorized("Невалідний ідентифікатор користувача");
                }
                PrescriptionReadDto prescription = await service.GetByIdAsync(id);
                RecordReadDto record = await recordService.GetByIdAsync(prescription.RecordId);

                if (record.DoctorId != currentUserId)
                {
                    return Forbid("Ви можете видаляти тільки свої призначення");
                }

                await service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Не можемо знайти призначення з таким id ({id})");
            }
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PrescriptionReadDto>>> GetPaged([FromQuery] SortingParameters parameters)
        {
            var result = await service.GetPagedAsync(parameters);
            return Ok(result);
        }

        [HttpGet("filter")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetFiltered([FromQuery] PrescriptionFilterDto filter)
        {
            var prescriptions = await service.GetFilteredAsync(filter);
            return Ok(prescriptions);
        }
    }
}