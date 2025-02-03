using Moq;
using PANDA.Api.Services;
using PANDA.Api.Data;
using PANDA.Api.Models;
using PANDA.Api.Models.DTOs;
using AutoMapper;
using Microsoft.Extensions.Localization;
using Microsoft.EntityFrameworkCore;
using PANDA.Api.Enums;

namespace PANDA.Api.Tests.Services
{
    [TestClass]
    public class AppointmentServiceTests
    {
        private AppointmentService _service;
        private Mock<IMapper> _mockMapper;
        private Mock<IStringLocalizer<AppointmentService>> _mockLocalizer;
        private PandaContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Configure in-memory database
            var options = new DbContextOptionsBuilder<PandaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PandaContext(options);

            // Mock dependencies
            _mockMapper = new Mock<IMapper>();
            _mockLocalizer = new Mock<IStringLocalizer<AppointmentService>>();

            // Initialize service
            _service = new AppointmentService(_context, _mockMapper.Object, _mockLocalizer.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAppointmentByIdAsync_ReturnsAppointmentDto_WhenAppointmentExists()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var clinicianId = Guid.NewGuid();
            var organisationId = Guid.NewGuid();
            var departmentID = Guid.NewGuid();

            var patient = new Patient
            {
                NhsNumber = "1234567890",
                Name = "John Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Postcode = "AA1 1AA"
            };
            await _context.Patients.AddAsync(patient);

            var clinician = new Clinician
            {
                Id = clinicianId,
                Name = "Dr. Smith"
            };
            await _context.Clinicians.AddAsync(clinician);

            var organisation = new Organisation
            {
                Id = organisationId,
                Postcode = "AA1 1AA"
            };
            await _context.Organisations.AddAsync(organisation);

            var department = new Department
            {
                Id = departmentID,
                Name = "Cardiology"
            };

            var appointment = new Appointment
            {
                Id = appointmentId,
                Time = DateTimeOffset.Now.AddHours(1),
                Duration = "15m",
                Patient = patient.NhsNumber,
                PatientEntity = patient,
                ClinicianId = clinicianId,
                OrganisationId = organisationId,
                DepartmentId = departmentID,
                Clinician = clinician,
                Organisation = organisation,
                Department = department
            };
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            var appointmentDto = new AppointmentDto
            {
                Id = appointmentId,
                Time = appointment.Time,
                Duration = appointment.Duration,
                Department = appointment.Department.Name,
                Patient = appointment.Patient,
                Status = AppointmentStatus.Active
            };
            _mockMapper.Setup(m => m.Map<AppointmentDto>(It.IsAny<Appointment>()))
                        .Returns(appointmentDto);

            // Act
            var result = await _service.GetAppointmentByIdAsync(appointmentId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(appointmentId, result.Id);
        }

        [TestMethod]
        public async Task GetAppointmentByIdAsync_ReturnsNull_WhenAppointmentDoesNotExist()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();

            // Act
            var result = await _service.GetAppointmentByIdAsync(appointmentId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddAppointmentAsync_AddsAppointment_WhenValid()
        {
            // Arrange
            var patient = new Patient
            {
                NhsNumber = "1234567890",
                Name = "John Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Postcode = "AA1 1AA"
            };
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            var appointmentDto = new AppointmentDto
            {
                Id = Guid.NewGuid(),
                Time = DateTimeOffset.Now.AddHours(1),
                Duration = "15m",
                Patient = patient.NhsNumber,
                Clinician = "Dr. Smith",
                Department = "Cardiology",
                Postcode = "AA1 1AA",
                Status = AppointmentStatus.Active
            };

            var appointment = new Appointment
            {
                Id = appointmentDto.Id,
                Time = appointmentDto.Time,
                Duration = appointmentDto.Duration,
                Patient = appointmentDto.Patient,
                ClinicianId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid()
            };

            _mockMapper.Setup(m => m.Map<Appointment>(It.IsAny<AppointmentDto>()))
                        .Returns(appointment);

            // Act
            await _service.AddAppointmentAsync(appointmentDto);

            // Assert
            var addedAppointment = await _context.Appointments.FindAsync(appointmentDto.Id);
            Assert.IsNotNull(addedAppointment);
            Assert.AreEqual(appointmentDto.Duration, addedAppointment.Duration);
            Assert.AreEqual(appointmentDto.Patient, addedAppointment.Patient);
        }

        [TestMethod]
        public async Task AddAppointmentAsync_ThrowsException_WhenPatientNotFound()
        {
            // Arrange
            var appointmentDto = new AppointmentDto
            {
                Id = Guid.NewGuid(),
                Patient = "NonExistentPatient",
                Time = DateTimeOffset.Now.AddHours(1),
                Duration = "15m",
                Clinician = "Dr. Smith",
                Department = "Cardiology",
                Postcode = "AA1 1AA",
                Status = AppointmentStatus.Active
            };

            _mockLocalizer.Setup(l => l["PatientNotFound"]).Returns(new LocalizedString("PatientNotFound", "Patient not found"));

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _service.AddAppointmentAsync(appointmentDto));
            Assert.AreEqual("Patient not found", exception.Message);
        }
    }
}