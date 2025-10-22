using PROG6212_POE_ST10021259.Models;
using PROG6212_POE_ST10021259.Services;
using Xunit;

namespace PROG6212_POE_ST10021259.Tests
{
    public class ClaimServiceTests
    {
        [Fact]
        public void AddClaim_ShouldAddClaimToList()
        {
            // Arrange
            var claim = new Claim
            {
                HoursWorked = 40,
                HourlyRate = 500,
                Notes = "Test claim"
            };

            // Act
            ClaimService.AddClaim(claim);
            var allClaims = ClaimService.GetAllClaims();

            // Assert
            Assert.Contains(allClaims, c => c.Notes == "Test claim");
        }

        [Fact]
        public void GetNextId_ShouldReturnUniqueIds()
        {
            // Act
            var id1 = ClaimService.GetNextId();
            var id2 = ClaimService.GetNextId();

            // Assert
            Assert.NotEqual(id1, id2);
            Assert.True(id2 > id1);
        }

        [Fact]
        public void GetPendingClaims_ShouldReturnOnlyPendingClaims()
        {
            // Arrange
            var pendingClaim = new Claim
            {
                Id = ClaimService.GetNextId(),
                HoursWorked = 30,
                HourlyRate = 400,
                Status = "Pending"
            };

            var approvedClaim = new Claim
            {
                Id = ClaimService.GetNextId(),
                HoursWorked = 25,
                HourlyRate = 350,
                Status = "Approved"
            };

            ClaimService.AddClaim(pendingClaim);
            ClaimService.AddClaim(approvedClaim);

            // Act
            var pendingClaims = ClaimService.GetPendingClaims();

            // Assert
            Assert.All(pendingClaims, c => Assert.Equal("Pending", c.Status));
        }

        [Fact]
        public void UpdateClaimStatus_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var claim = new Claim
            {
                Id = ClaimService.GetNextId(),
                HoursWorked = 20,
                HourlyRate = 300,
                Status = "Pending"
            };
            ClaimService.AddClaim(claim);

            // Act
            var result = ClaimService.UpdateClaimStatus(claim.Id, "Approved");

            // Assert
            Assert.True(result);
            var updatedClaim = ClaimService.GetClaimById(claim.Id);
            Assert.Equal("Approved", updatedClaim.Status);
        }

        [Fact]
        public void UpdateClaimStatus_WithInvalidId_ShouldReturnFalse()
        {
            // Act
            var result = ClaimService.UpdateClaimStatus(99999, "Approved");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetClaimById_WithValidId_ShouldReturnClaim()
        {
            // Arrange
            var claim = new Claim
            {
                Id = ClaimService.GetNextId(),
                HoursWorked = 15,
                HourlyRate = 250,
                Notes = "Find me"
            };
            ClaimService.AddClaim(claim);

            // Act
            var result = ClaimService.GetClaimById(claim.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Find me", result.Notes);
        }

        [Fact]
        public void GetClaimById_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var result = ClaimService.GetClaimById(99999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAllClaims_ShouldReturnAllAddedClaims()
        {
            // Arrange
            var initialCount = ClaimService.GetAllClaims().Count;

            var claim1 = new Claim { Id = ClaimService.GetNextId(), HoursWorked = 10, HourlyRate = 200 };
            var claim2 = new Claim { Id = ClaimService.GetNextId(), HoursWorked = 20, HourlyRate = 300 };

            ClaimService.AddClaim(claim1);
            ClaimService.AddClaim(claim2);

            // Act
            var allClaims = ClaimService.GetAllClaims();

            // Assert
            Assert.Equal(initialCount + 2, allClaims.Count);
        }
    }
}