using PROG6212_POE_ST10021259.Models;
using PROG6212_POE_ST10021259.Services;
using Xunit;

namespace PROG6212_POE_ST10021259.Tests
{
    public class ClaimValidationServiceTests
    {
        private readonly ClaimValidationService _validationService;

        public ClaimValidationServiceTests()
        {
            _validationService = new ClaimValidationService();
        }

        [Fact]
        public void ValidateClaim_WithValidData_ShouldPass()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 500
            };

            // Act
            var result = _validationService.ValidateClaim(claim);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void ValidateClaim_WithTooManyHours_ShouldFail()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 800,
                HourlyRate = 500
            };

            // Act
            var result = _validationService.ValidateClaim(claim);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Hours worked must be between"));
        }

        [Fact]
        public void ValidateClaim_WithLowHourlyRate_ShouldWarn()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 50
            };

            // Act
            var result = _validationService.ValidateClaim(claim);

            // Assert
            Assert.True(result.IsValid);
            Assert.Contains(result.Warnings, w => w.Contains("below recommended minimum"));
            Assert.False(result.AutoApprovalEligible);
        }

        [Fact]
        public void ValidateClaim_WithHighHourlyRate_ShouldWarn()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 2500
            };

            // Act
            var result = _validationService.ValidateClaim(claim);

            // Assert
            Assert.True(result.IsValid);
            Assert.Contains(result.Warnings, w => w.Contains("exceeds standard maximum"));
            Assert.False(result.AutoApprovalEligible);
        }

        [Fact]
        public void ValidateClaim_WithHighTotalAmount_ShouldWarn()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 200,
                HourlyRate = 600
            };

            // Act
            var result = _validationService.ValidateClaim(claim);

            // Assert
            Assert.True(result.IsValid);
            Assert.Contains(result.Warnings, w => w.Contains("exceeds maximum threshold"));
            Assert.False(result.AutoApprovalEligible);
        }

        [Fact]
        public void ValidateClaim_WithHighAmountNoDocument_ShouldWarn()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 150,
                HourlyRate = 500,
                SupportingDocument = null
            };

            // Act
            var result = _validationService.ValidateClaim(claim);

            // Assert
            Assert.True(result.IsValid);
            Assert.Contains(result.Warnings, w => w.Contains("require supporting documents"));
            Assert.False(result.AutoApprovalEligible);
        }

        [Fact]
        public void ValidateClaim_WithExcessiveHours_ShouldWarn()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 200,
                HourlyRate = 500
            };

            // Act
            var result = _validationService.ValidateClaim(claim);

            // Assert
            Assert.True(result.IsValid);
            Assert.Contains(result.Warnings, w => w.Contains("exceeds typical monthly hours"));
        }

        [Theory]
        [InlineData(0, 500, false)]
        [InlineData(750, 500, false)]
        [InlineData(40, 500, true)]
        [InlineData(160, 500, true)]
        public void ValidateClaim_HoursWorkedRange_ShouldValidateCorrectly(double hours, double rate, bool expectedValid)
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = hours,
                HourlyRate = rate
            };

            // Act
            var result = _validationService.ValidateClaim(claim);

            // Assert
            Assert.Equal(expectedValid, result.IsValid);
        }

        [Fact]
        public void GetValidationRules_ShouldReturnRules()
        {
            // Act
            var rules = _validationService.GetValidationRules();

            // Assert
            Assert.NotNull(rules);
            Assert.True(rules.MinHoursWorked > 0);
            Assert.True(rules.MaxHoursWorked > 0);
            Assert.True(rules.MinHourlyRate > 0);
        }
    }
}