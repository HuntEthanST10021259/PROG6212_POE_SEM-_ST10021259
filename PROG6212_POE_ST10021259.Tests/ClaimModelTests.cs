using PROG6212_POE_ST10021259.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace PROG6212_POE_ST10021259.Tests
{
    public class ClaimModelTests
    {
        [Fact]
        public void TotalAmount_ShouldCalculateCorrectly()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 500
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(20000, totalAmount);
        }

        [Theory]
        [InlineData(10, 100, 1000)]
        [InlineData(40, 500, 20000)]
        [InlineData(0.5, 200, 100)]
        [InlineData(160, 350, 56000)]
        public void TotalAmount_WithVariousInputs_ShouldCalculateCorrectly(
            double hours, double rate, double expected)
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = hours,
                HourlyRate = rate
            };

            // Act
            var totalAmount = claim.TotalAmount;

            // Assert
            Assert.Equal(expected, totalAmount);
        }

        [Fact]
        public void Claim_DefaultStatus_ShouldBePending()
        {
            // Arrange & Act
            var claim = new Claim();

            // Assert
            Assert.Equal("Pending", claim.Status);
        }

        [Fact]
        public void Claim_DefaultDateSubmitted_ShouldBeRecent()
        {
            // Arrange & Act
            var claim = new Claim();

            // Assert
            Assert.True((DateTime.Now - claim.DateSubmitted).TotalSeconds < 1);
        }

        [Fact]
        public void HoursWorked_WithZeroValue_ShouldFailValidation()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 0,
                HourlyRate = 500
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains("HoursWorked"));
        }

        [Fact]
        public void HoursWorked_WithNegativeValue_ShouldFailValidation()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = -10,
                HourlyRate = 500
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains("HoursWorked"));
        }

        [Fact]
        public void HoursWorked_WithValueOver744_ShouldFailValidation()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 800,
                HourlyRate = 500
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains("HoursWorked"));
        }

        [Fact]
        public void HourlyRate_WithZeroValue_ShouldFailValidation()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 0
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.Contains(validationResults, v => v.MemberNames.Contains("HourlyRate"));
        }

        [Fact]
        public void Claim_WithValidData_ShouldPassValidation()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 500,
                Notes = "Valid claim"
            };

            // Act
            var validationResults = ValidateModel(claim);

            // Assert
            Assert.Empty(validationResults);
        }

        // Helper method to validate models
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}