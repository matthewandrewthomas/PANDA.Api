using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PANDA.Api.Controllers;
using PANDA.Api.Services;
using PANDA.Api.Models.DTOs;
using FluentValidation;

namespace PANDA.Api.Tests.Controllers
{
    [TestClass]
    public class PatientsControllerTests
    {
        private PatientsController _controller;
        private Mock<ILogger<PatientsController>> _mockLogger;
        private Mock<IStringLocalizer<PatientsController>> _mockLocalizer;
        private Mock<IPatientService> _mockPatientService;
        private Mock<IValidator<string>> _mockNhsNumberValidator;
        private Mock<IValidator<PatientDto>> _mockPatientValidator;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<PatientsController>>();
            _mockLocalizer = new Mock<IStringLocalizer<PatientsController>>();
            _mockPatientService = new Mock<IPatientService>();
            _mockNhsNumberValidator = new Mock<IValidator<string>>();
            _mockPatientValidator = new Mock<IValidator<PatientDto>>();

            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockNhsNumberValidator.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult); // Setup the validator to return a valid result

            _mockPatientValidator.Setup(v => v.ValidateAsync(It.IsAny<PatientDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult); // Setup the validator to return a valid result

            _controller = new PatientsController(
                _mockLogger.Object,
                _mockLocalizer.Object,
                _mockPatientService.Object,
                _mockNhsNumberValidator.Object,
                _mockPatientValidator.Object

            );
        }

        [TestMethod]
        public async Task GetPatients_ReturnsOk_WhenPatientsExist()
        {
            // Arrange
            var patients = new List<PatientDto>
            {
                new PatientDto { NhsNumber = "1234567890", Name = "John Doe" }
            };
            _mockPatientService.Setup(s => s.GetPatientsAsync())
                               .ReturnsAsync(patients);

            // Act
            var result = await _controller.GetPatients();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(patients, okResult.Value);
        }

        [TestMethod]
        public async Task GetPatient_ReturnsOk_WhenPatientExists()
        {
            // Arrange
            string nhsNumber = "1234567890";
            var patient = new PatientDto { NhsNumber = nhsNumber, Name = "Jane Doe" };
            _mockPatientService.Setup(s => s.GetPatientByIdAsync(nhsNumber))
                               .ReturnsAsync(patient);

            // Act
            var result = await _controller.GetPatient(nhsNumber);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(patient, okResult.Value);
        }

        [TestMethod]
        public async Task GetPatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            string nhsNumber = "1234567890";
            _mockPatientService.Setup(s => s.GetPatientByIdAsync(nhsNumber))
                               .ReturnsAsync((PatientDto)null);

            var localizedString = new LocalizedString("PatientNotFound", "Patient not found");
            _mockLocalizer.Setup(l => l["PatientNotFound"])
                          .Returns(localizedString);

            // Act
            var result = await _controller.GetPatient(nhsNumber);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            var value = notFoundResult.Value as LocalizedString;
            Assert.IsNotNull(value);
            Assert.AreEqual(localizedString.Value, value.Value);
        }

        [TestMethod]
        public async Task PostPatient_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var patientDto = new PatientDto { NhsNumber = "1234567890", Name = "John Doe" };

            // Act
            var result = await _controller.PostPatient(patientDto);

            // Assert
            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(_controller.GetPatient), createdAtActionResult.ActionName);
            Assert.AreEqual(patientDto.NhsNumber, createdAtActionResult.RouteValues["nhs_number"]);
        }

        [TestMethod]
        public async Task PutPatient_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            var patientDto = new PatientDto { NhsNumber = "1234567890", Name = "John Doe" };

            _mockPatientService.Setup(s => s.PatientExistsAsync(patientDto.NhsNumber))
                               .ReturnsAsync(true);

            // Act
            var result = await _controller.PutPatient(patientDto.NhsNumber, patientDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeletePatient_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            string nhsNumber = "1234567890";
            _mockPatientService.Setup(s => s.DeletePatientAsync(nhsNumber))
                               .ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePatient(nhsNumber);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
