using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using PROG6212_POE_ST10021259.Controllers;
using PROG6212_POE_ST10021259.Models;
using PROG6212_POE_ST10021259.Services;
using Xunit;

namespace PROG6212_POE_ST10021259.Tests
{
    public class CoordinatorControllerTests
    {
        [Fact]
        public void VerifyClaims_ShouldReturnViewWithPendingClaims()
        {
            // Arrange
            var controller = new CoordinatorController();

            // Add a pending claim
            var claim = new Claim
            {
                Id = ClaimService.GetNextId(),
                HoursWorked = 40,
                HourlyRate = 500,
                Status = "Pending"
            };
            ClaimService.AddClaim(claim);

            // Act
            var result = controller.VerifyClaims() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Contains(model, c => c.Status == "Pending");
        }

        [Fact]
        public void Approve_WithValidId_ShouldSetSuccessMessage()
        {
            // Arrange
            var controller = new CoordinatorController();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var claim = new Claim
            {
                Id = ClaimService.GetNextId(),
                HoursWorked = 30,
                HourlyRate = 400,
                Status = "Pending"
            };
            ClaimService.AddClaim(claim);

            // Act
            var result = controller.Approve(claim.Id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("VerifyClaims", result.ActionName);
            Assert.True(controller.TempData.ContainsKey("Message"));
            Assert.Contains("approved", controller.TempData["Message"].ToString().ToLower());
        }

        [Fact]
        public void Approve_WithInvalidId_ShouldSetErrorMessage()
        {
            // Arrange
            var controller = new CoordinatorController();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = controller.Approve(99999) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(controller.TempData.ContainsKey("Error"));
            Assert.Contains("not found", controller.TempData["Error"].ToString().ToLower());
        }

        [Fact]
        public void Reject_WithValidId_ShouldSetSuccessMessage()
        {
            // Arrange
            var controller = new CoordinatorController();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var claim = new Claim
            {
                Id = ClaimService.GetNextId(),
                HoursWorked = 25,
                HourlyRate = 350,
                Status = "Pending"
            };
            ClaimService.AddClaim(claim);

            // Act
            var result = controller.Reject(claim.Id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("VerifyClaims", result.ActionName);
            Assert.True(controller.TempData.ContainsKey("Message"));
            Assert.Contains("rejected", controller.TempData["Message"].ToString().ToLower());
        }

        [Fact]
        public void Reject_WithInvalidId_ShouldSetErrorMessage()
        {
            // Arrange
            var controller = new CoordinatorController();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = controller.Reject(99999) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(controller.TempData.ContainsKey("Error"));
            Assert.Contains("not found", controller.TempData["Error"].ToString().ToLower());
        }

        [Fact]
        public void Approve_ShouldUpdateClaimStatusToApproved()
        {
            // Arrange
            var controller = new CoordinatorController();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var claim = new Claim
            {
                Id = ClaimService.GetNextId(),
                HoursWorked = 35,
                HourlyRate = 450,
                Status = "Pending"
            };
            ClaimService.AddClaim(claim);

            // Act
            controller.Approve(claim.Id);
            var updatedClaim = ClaimService.GetClaimById(claim.Id);

            // Assert
            Assert.Equal("Approved", updatedClaim.Status);
        }

        [Fact]
        public void Reject_ShouldUpdateClaimStatusToRejected()
        {
            // Arrange
            var controller = new CoordinatorController();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var claim = new Claim
            {
                Id = ClaimService.GetNextId(),
                HoursWorked = 20,
                HourlyRate = 300,
                Status = "Pending"
            };
            ClaimService.AddClaim(claim);

            // Act
            controller.Reject(claim.Id);
            var updatedClaim = ClaimService.GetClaimById(claim.Id);

            // Assert
            Assert.Equal("Rejected", updatedClaim.Status);
        }
    }
}