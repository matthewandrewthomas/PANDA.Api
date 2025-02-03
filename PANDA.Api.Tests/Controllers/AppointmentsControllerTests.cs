using Moq;
using PANDA.Api.Controllers;
using PANDA.Api.Services;
using PANDA.Api.Models.DTOs;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using PANDA.Api.Enums;
using FluentValidation;

namespace PANDA.Api.Tests.Controllers
{
    [TestClass]
    public class AppointmentsControllerTests
    {
        private AppointmentsController _controller;
        private Mock<ILogger<AppointmentsController>> _mockLogger;
        private Mock<IStringLocalizer<AppointmentsController>> _mockLocalizer;
        private Mock<IAppointmentService> _mockAppointmentService;
        private Mock<IPatientService> _mockPatientService;
        private Mock<IValidator<string>> _mockNhsNumberValidator;
        private Mock<IValidator<AppointmentDto>> _mockAppointmentValidator;

        [TestInitialize]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<AppointmentsController>>();
            _mockLocalizer = new Mock<IStringLocalizer<AppointmentsController>>();
            _mockAppointmentService = new Mock<IAppointmentService>();
            _mockPatientService = new Mock<IPatientService>();
            _mockNhsNumberValidator = new Mock<IValidator<string>>();
            _mockAppointmentValidator = new Mock<IValidator<AppointmentDto>>();

            var validationResult = new FluentValidation.Results.ValidationResult();

            _mockNhsNumberValidator.Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult); // Setup the validator to return a valid result

            _mockAppointmentValidator.Setup(v => v.ValidateAsync(It.IsAny<AppointmentDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult); // Setup the validator to return a valid result

            _controller = new AppointmentsController(
                _mockLogger.Object,
                _mockLocalizer.Object,
                _mockAppointmentService.Object,
                _mockPatientService.Object,
                _mockNhsNumberValidator.Object,
                _mockAppointmentValidator.Object
            );
        }

        [TestMethod]
        public async Task GetAppointmentsByNhsNumber_ReturnsOk_WhenAppointmentsExist()
        {
            // Arrange
            string nhsNumber = "1234567890";
            var appointments = new List<AppointmentDto>
            {
                new AppointmentDto { Id = Guid.NewGuid(), Patient = nhsNumber }
            };
            _mockAppointmentService.Setup(s => s.GetAppointmentsByNhsNumberAsync(nhsNumber))
                                   .ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAppointmentsByNhsNumber(nhsNumber);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(appointments, okResult.Value);
        }

        [TestMethod]
        public async Task GetAppointmentById_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            // Arrange
            Guid appointmentId = Guid.NewGuid();
            _mockAppointmentService.Setup(s => s.GetAppointmentByIdAsync(appointmentId))
                                   .ReturnsAsync((AppointmentDto)null);

            // Act
            var result = await _controller.GetAppointmentById(appointmentId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostAppointment_ReturnsCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var appointmentDto = new AppointmentDto
            {
                Id = Guid.NewGuid(),
                Patient = "1234567890",
                Time = DateTimeOffset.Now.AddHours(1),
                Duration = "30",
                Clinician = "Dr. Smith",
                Department = "Cardiology",
                Postcode = "12345",
                Status = AppointmentStatus.Active
            };

            _mockPatientService.Setup(s => s.PatientExistsAsync(appointmentDto.Patient))
                               .ReturnsAsync(true);

            // Act
            var result = await _controller.PostAppointment(appointmentDto);

            // Assert
            var actionResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(nameof(_controller.GetAppointmentById), actionResult.ActionName);
            Assert.AreEqual(appointmentDto.Id, actionResult.RouteValues["id"]);
        }
    }
}
