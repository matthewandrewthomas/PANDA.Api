using PANDA.Api.Validators;

namespace PANDA.Api.Tests.Validators
{
    [TestClass]
    public class NhsNumberValidatorTests
    {
        private NhsNumberValidator _validator;

        [TestInitialize]
        public void Setup()
        {
            _validator = new NhsNumberValidator();
        }

        [TestMethod]
        public void IsValidNhsNumber_ReturnsTrue_ForValidNhsNumber()
        {
            // Arrange
            string validNhsNumber = "1373645350";

            // Act
            var result = _validator.Validate(validNhsNumber);

            // Assert
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public void IsValidNhsNumber_ReturnsFalse_ForInvalidNhsNumber()
        {
            // Arrange
            string invalidNhsNumber = "1234567899";

            // Act
            var result = _validator.Validate(invalidNhsNumber);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void IsValidNhsNumber_ReturnsFalse_ForNhsNumberWithIncorrectLength()
        {
            // Arrange
            string shortNhsNumber = "123456789"; // 9 digits
            string longNhsNumber = "12345678901"; // 11 digits

            // Act
            var shortResult = _validator.Validate(shortNhsNumber);
            var longResult = _validator.Validate(longNhsNumber);

            // Assert
            Assert.IsFalse(shortResult.IsValid);
            Assert.IsFalse(longResult.IsValid);
        }
    }
}
