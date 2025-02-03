using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using PANDA.Api.Models.DTOs;
using PANDA.Api.Services;

namespace PANDA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly ILogger<PatientsController> _logger;
        private readonly IStringLocalizer<PatientsController> _localizer;
        private readonly IPatientService _patientService;
        private readonly IValidator<string> _nhsNumberValidator;
        private readonly IValidator<PatientDto> _patientValidator;

        public PatientsController(
            ILogger<PatientsController> logger,
            IStringLocalizer<PatientsController> localizer,
            IPatientService patientService,
            IValidator<string> nhsNumberValidator,
            IValidator<PatientDto> patientValidator)
        {
            _logger = logger;
            _localizer = localizer;
            _patientService = patientService;
            _nhsNumberValidator = nhsNumberValidator;
            _patientValidator = patientValidator;
        }

        // GET: api/Patients
        /// <summary>
        /// Retrieves all patients.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/patients
        ///
        /// </remarks>
        /// <returns>A list of patients.</returns>
        /// <response code="200">Returns the list of patients</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
        {
            _logger.LogInformation("Getting all patients");

            var patients = await _patientService.GetPatientsAsync();
            return Ok(patients);
        }

        // GET: api/Patients/0123456789
        /// <summary>
        /// Retrieves a single patient by NHS Number.
        /// </summary>
        /// <param name="nhs_number">The NHS number of the patient to be returned.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET: api/Patients/0123456789
        ///
        /// </remarks>
        /// <returns>A single patient.</returns>
        /// <response code="200">Returns a single patient</response>
        [HttpGet("{nhs_number}")]
        public async Task<ActionResult<PatientDto>> GetPatient(string nhs_number)
        {
            _logger.LogInformation("Getting single patient with NHS number");

            var validationResult = await _nhsNumberValidator.ValidateAsync(nhs_number);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var patient = await _patientService.GetPatientByIdAsync(nhs_number);
            if (patient == null)
            {
                _logger.LogWarning("Patient with NHS number not found");
                return NotFound(_localizer["PatientNotFound"]);
            }

            return Ok(patient);
        }

        /// <summary>
        /// Updates a patient record.
        /// </summary>
        /// <param name="nhs_number">The NHS number of the patient to be updated.</param>
        /// <param name="patient">The patient data transfer object containing updated details.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// </returns>
        /// <response code="204">If the update was successful.</response>
        /// <response code="400">If the NHS number in the URL does not match the NHS number in the patient DTO.</response>
        /// <response code="404">If the patient with the specified NHS number is not found.</response>
        [HttpPut("{nhs_number}")]
        public async Task<IActionResult> PutPatient(string nhs_number, PatientDto patient)
        {
            var nshNumberResult = await _nhsNumberValidator.ValidateAsync(nhs_number);
            if (!nshNumberResult.IsValid)
            {
                return BadRequest(nshNumberResult.Errors);
            }

            var patientResult = await _patientValidator.ValidateAsync(patient);
            if (!patientResult.IsValid)
            {
                return BadRequest(patientResult.Errors);
            }

            if (nhs_number != patient.NhsNumber)
            {
                _logger.LogWarning("Mismatched NHS number in request");
                return BadRequest(_localizer["PatientIdMismatch"]);
            }

            _logger.LogInformation("Updating patient with NHS number");

            try
            {
                await _patientService.UpdatePatientAsync(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient with NHS number");
                if (!await _patientService.PatientExistsAsync(nhs_number))
                {
                    return NotFound(_localizer["PatientNotFound"]);
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Adds a new patient record.
        /// </summary>
        /// <param name="patient">The patient data transfer object containing patient details.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing the newly created patient.
        /// </returns>
        /// <response code="201">If the patient is successfully created.</response>
        /// <response code="400">If there is an issue with the request data.</response>
        [HttpPost]
        public async Task<ActionResult<PatientDto>> PostPatient(PatientDto patient)
        {
            _logger.LogInformation("Adding new patient with NHS number");

            var patientResult = await _patientValidator.ValidateAsync(patient);
            if (!patientResult.IsValid)
            {
                return BadRequest(patientResult.Errors);
            }

            await _patientService.AddPatientAsync(patient);

            return CreatedAtAction(nameof(GetPatient), new { nhs_number = patient.NhsNumber }, patient);
        }

        /// <summary>
        /// Deletes a patient record.
        /// </summary>
        /// <param name="nhs_number">The NHS number of the patient to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// </returns>
        /// <response code="204">If the patient is successfully deleted.</response>
        /// <response code="404">If the patient with the specified NHS number is not found.</response>
        [HttpDelete("{nhs_number}")]
        public async Task<IActionResult> DeletePatient(string nhs_number)
        {
            _logger.LogInformation("Deleting patient with NHS number");

            var validationResult = await _nhsNumberValidator.ValidateAsync(nhs_number);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            return await _patientService.DeletePatientAsync(nhs_number)
                ? NoContent()
                : NotFound(_localizer["PatientNotFound"]);
        }
    }
}
