using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Moq;
using PANDA.Api.Data;
using PANDA.Api.Models;
using PANDA.Api.Models.DTOs;
using PANDA.Api.Services;

namespace PANDA.Api.Tests.Services
{
    [TestClass]
    public class PatientServiceTests
    {
        private PatientService _service;
        private Mock<IMapper> _mockMapper;
        private PandaContext _context;
        private Mock<IStringLocalizer<PatientService>> _mockLocalizer;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PandaContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PandaContext(options);
            _mockMapper = new Mock<IMapper>();
            _mockLocalizer = new Mock<IStringLocalizer<PatientService>>();
            _service = new PatientService(_context, _mockMapper.Object, _mockLocalizer.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetPatientByIdAsync_ReturnsPatientDto_WhenPatientExists()
        {
            // Arrange
            var patient = new Patient
            {
                NhsNumber = "1234567890",
                Name = "Jane Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Postcode = "AA1 1AA"
            };
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();

            var patientDto = new PatientDto
            {
                NhsNumber = "1234567890",
                Name = "Jane Doe",
                DateOfBirth = new DateTime(1980, 1, 1),
                Postcode = "AA1 1AA"
            };
            _mockMapper.Setup(m => m.Map<PatientDto>(It.IsAny<Patient>()))
                .Returns(patientDto);

            // Act
            var result = await _service.GetPatientByIdAsync(patient.NhsNumber);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(patient.NhsNumber, result.NhsNumber);
        }
    }
}
