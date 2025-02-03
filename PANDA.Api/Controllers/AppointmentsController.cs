using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PANDA.Api.Models.DTOs;
using PANDA.Api.Services;

namespace PANDA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IStringLocalizer<AppointmentsController> _localizer;
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IValidator<string> _nhsNumberValidator;
        private readonly IValidator<AppointmentDto> _appointmentValidator;

        public AppointmentsController(
            ILogger<AppointmentsController> logger,
            IStringLocalizer<AppointmentsController> localizer,
            IAppointmentService appointmentService,
            IPatientService patientService,
            IValidator<string> nhsNumberValidator,
            IValidator<AppointmentDto> appointmentValidator)
        {
            _logger = logger;
            _localizer = localizer;
            _appointmentService = appointmentService;
            _patientService = patientService;
            _nhsNumberValidator = nhsNumberValidator;
            _appointmentValidator = appointmentValidator;
        }

        /// <summary>
        /// Lists all appointments for a given patient by NHS number.
        /// </summary>
        /// <param name="nhs_number">The NHS number of the patient.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing a list of appointments.</returns>
        /// <response code="200">Returns the list of appointments.</response>
        /// <response code="404">If the patient does not have any appointments.</response>
        [HttpGet("patient/{nhs_number}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByNhsNumber(string nhs_number)
        {
            _logger.LogInformation("Getting appointments for NHS number");

            var validationResult = await _nhsNumberValidator.ValidateAsync(nhs_number);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var appointments = await _appointmentService.GetAppointmentsByNhsNumberAsync(nhs_number);
            if (appointments == null || !appointments.Any())
            {
                _logger.LogWarning("No appointments found for NHS number");
                return NotFound();
            }

            return Ok(appointments);
        }

        /// <summary>
        /// Gets a single appointment by ID.
        /// </summary>
        /// <param name="id">The ID of the appointment.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing the appointment.</returns>
        /// <response code="200">Returns the appointment.</response>
        /// <response code="404">If the appointment is not found.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
        {
            _logger.LogInformation($"Getting appointment with ID {id}");

            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning($"Appointment with ID {id} not found");
                return NotFound();
            }

            return Ok(appointment);
        }

        /// <summary>
        /// Adds a new appointment.
        /// </summary>
        /// <param name="appointment">The appointment to add.</param>
        /// <returns>An <see cref="ActionResult{T}"/> containing the newly created appointment.</returns>
        /// <response code="201">If the appointment is successfully created.</response>
        /// <response code="400">If there is an issue with the request data.</response>
        /// <response code="404">If the patient cannot be found.</response>
        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> PostAppointment([FromBody] AppointmentDto appointment)
        {
            _logger.LogInformation($"Adding new appointment with ID {appointment.Id}");

            var appointmentResult = await _appointmentValidator.ValidateAsync(appointment);
            if (!appointmentResult.IsValid)
            {
                return BadRequest(appointmentResult.Errors);
            }

            try
            {
                await _appointmentService.AddAppointmentAsync(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding appointment with ID {appointment.Id}");
                if (!await _patientService.PatientExistsAsync(appointment.Patient))
                {
                    return NotFound(_localizer["PatientNotFound"]);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.Id }, appointment);
        }

        /// <summary>
        /// Updates an existing appointment.
        /// </summary>
        /// <param name="id">The ID of the appointment to update.</param>
        /// <param name="appointment">The updated appointment object.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        /// <response code="204">If the appointment is successfully updated.</response>
        /// <response code="400">If the appointment ID does not match the provided ID.</response>
        /// <response code="404">If the patient cannot be found.</response>
        /// <response code="500">If an error occurs while updating the appointment.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(Guid id, AppointmentDto appointment)
        {
            var appointmentResult = await _appointmentValidator.ValidateAsync(appointment);
            if (!appointmentResult.IsValid)
            {
                return BadRequest(appointmentResult.Errors);
            }

            if (id != appointment.Id)
            {
                _logger.LogWarning($"Mismatched appointment ID in request: {id}");
                return BadRequest();
            }

            _logger.LogInformation($"Updating appointment with ID {id}");

            try
            {
                await _appointmentService.UpdateAppointmentAsync(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating appointment with ID {id}");
                if (!await _patientService.PatientExistsAsync(appointment.Patient))
                {
                    return NotFound(_localizer["PatientNotFound"]);
                }
                else
                {
                    return StatusCode(500, _localizer["ErrorUpdatingAppointment"]);
                }
            }

            return NoContent();
        }
    }
}
