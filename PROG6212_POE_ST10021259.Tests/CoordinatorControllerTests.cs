using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using PROG6212_POE_ST10021259.Controllers;
using PROG6212_POE_ST10021259.Models;
using PROG6212_POE_ST10021259.Services;
using Xunit;

namespace PROG6212_POE_ST10021259.Tests
{
    public class CoordinatorControllerTests
    {
        private readonly Mock<ILogger<CoordinatorController>> _mockLogger;
        private readonly Mock<IClaimService> _mockClaimService;

        public CoordinatorControllerTests()
        {
            _mockLogger = new Mock<ILogger<CoordinatorController>>();
            _mockClaimService = new Mock<IClaimService>();
        }

        [Fact]
        public async Task VerifyClaims_ShouldReturnViewWithPendingClaims()
        {
            // Arrange
            var pendingClaims = new List<Claim>
            {
                new Claim
                {
                    Id = 1,
                    HoursWorked = 40,
                    HourlyRate = 500,
                    Status = "Pending"
                }
            };

            _mockClaimService.Setup(s => s.GetPendingClaimsAsync())
                .ReturnsAsync(pendingClaims);

            var controller = new CoordinatorController(_mockLogger.Object, _mockClaimService.Object);

            // Act
            var result = await controller.VerifyClaims() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<List<Claim>>(result.Model);
            Assert.Single(model);
            Assert.Contains(model, c => c.Status == "Pending");
        }

        [Fact]
        public async Task Approve_WithValidId_ShouldSetSuccessMessage()
        {
            // Arrange
            _mockClaimService.Setup(s => s.CoordinatorApproveClaimAsync(1, "Coordinator"))
                .ReturnsAsync(true);

            var controller = new CoordinatorController(_mockLogger.Object, _mockClaimService.Object);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = await controller.Approve(1) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("VerifyClaims", result.ActionName);
            Assert.True(controller.TempData.ContainsKey("Message"));
            Assert.Contains("approved", controller.TempData["Message"].ToString().ToLower());
        }

        [Fact]
        public async Task Approve_WithInvalidId_ShouldSetErrorMessage()
        {
            // Arrange
            _mockClaimService.Setup(s => s.CoordinatorApproveClaimAsync(99999, "Coordinator"))
                .ReturnsAsync(false);

            var controller = new CoordinatorController(_mockLogger.Object, _mockClaimService.Object);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = await controller.Approve(99999) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(controller.TempData.ContainsKey("Error"));
            Assert.Contains("not found", controller.TempData["Error"].ToString().ToLower());
        }

        [Fact]
        public async Task Reject_WithValidId_ShouldSetSuccessMessage()
        {
            // Arrange
            _mockClaimService.Setup(s => s.RejectClaimAsync(1, "Coordinator", It.IsAny<string>()))
                .ReturnsAsync(true);

            var controller = new CoordinatorController(_mockLogger.Object, _mockClaimService.Object);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = await controller.Reject(1, "Test reason") as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("VerifyClaims", result.ActionName);
            Assert.True(controller.TempData.ContainsKey("Message"));
            Assert.Contains("rejected", controller.TempData["Message"].ToString().ToLower());
        }

        [Fact]
        public async Task Reject_WithInvalidId_ShouldSetErrorMessage()
        {
            // Arrange
            _mockClaimService.Setup(s => s.RejectClaimAsync(99999, "Coordinator", It.IsAny<string>()))
                .ReturnsAsync(false);

            var controller = new CoordinatorController(_mockLogger.Object, _mockClaimService.Object);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            var result = await controller.Reject(99999, "Test reason") as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(controller.TempData.ContainsKey("Error"));
            Assert.Contains("not found", controller.TempData["Error"].ToString().ToLower());
        }

        [Fact]
        public async Task Approve_ShouldCallServiceMethod()
        {
            // Arrange
            _mockClaimService.Setup(s => s.CoordinatorApproveClaimAsync(1, "Coordinator"))
                .ReturnsAsync(true);

            var controller = new CoordinatorController(_mockLogger.Object, _mockClaimService.Object);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            await controller.Approve(1);

            // Assert
            _mockClaimService.Verify(s => s.CoordinatorApproveClaimAsync(1, "Coordinator"), Times.Once);
        }

        [Fact]
        public async Task Reject_ShouldCallServiceMethod()
        {
            // Arrange
            _mockClaimService.Setup(s => s.RejectClaimAsync(1, "Coordinator", "Test reason"))
                .ReturnsAsync(true);

            var controller = new CoordinatorController(_mockLogger.Object, _mockClaimService.Object);
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            // Act
            await controller.Reject(1, "Test reason");

            // Assert
            _mockClaimService.Verify(s => s.RejectClaimAsync(1, "Coordinator", "Test reason"), Times.Once);
        }

        [Fact]
        public async Task Dashboard_ShouldReturnViewWithStatistics()
        {
            // Arrange
            var stats = new Dictionary<string, int>
            {
                { "Total", 10 },
                { "Pending", 5 },
                { "CoordinatorApproved", 3 },
                { "Approved", 2 },
                { "Rejected", 0 }
            };

            var claims = new List<Claim>
            {
                new Claim { Id = 1, HoursWorked = 40, HourlyRate = 500, Status = "Pending", DateSubmitted = DateTime.Now }
            };

            _mockClaimService.Setup(s => s.GetClaimStatisticsAsync()).ReturnsAsync(stats);
            _mockClaimService.Setup(s => s.GetAllClaimsAsync()).ReturnsAsync(claims);

            var controller = new CoordinatorController(_mockLogger.Object, _mockClaimService.Object);

            // Act
            var result = await controller.Dashboard() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ViewData["Statistics"]);
        }
    }
}