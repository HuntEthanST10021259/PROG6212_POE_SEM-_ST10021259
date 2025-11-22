using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Services;

namespace PROG6212_POE_ST10021259.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;
        private readonly IClaimService _claimService;

        public ManagerController(ILogger<ManagerController> logger, IClaimService claimService)
        {
            _logger = logger;
            _claimService = claimService;
        }

        public async Task<IActionResult> ReviewClaims()
        {
            try
            {
                var coordinatorApprovedClaims = await _claimService.GetCoordinatorApprovedClaimsAsync();
                return View(coordinatorApprovedClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading coordinator approved claims");
                TempData["Error"] = "An error occurred while loading claims. Please try again.";
                return View(new List<Models.Claim>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Invalid claim ID.";
                    return RedirectToAction("ReviewClaims");
                }

                var success = await _claimService.ManagerApproveClaimAsync(id, "Manager");

                if (success)
                {
                    _logger.LogInformation("Manager approved claim #{ClaimId}", id);
                    TempData["Message"] = $"Claim #{id} has been approved successfully! Payment will be processed.";
                }
                else
                {
                    TempData["Error"] = $"Unable to approve Claim #{id}. It may not be coordinator-approved yet.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving claim #{ClaimId}", id);
                TempData["Error"] = "An error occurred while approving the claim. Please try again.";
            }

            return RedirectToAction("ReviewClaims");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Invalid claim ID.";
                    return RedirectToAction("ReviewClaims");
                }

                if (string.IsNullOrWhiteSpace(reason))
                {
                    reason = "No reason provided";
                }

                var success = await _claimService.RejectClaimAsync(id, "Manager", reason);

                if (success)
                {
                    _logger.LogInformation("Manager rejected claim #{ClaimId}", id);
                    TempData["Message"] = $"Claim #{id} has been rejected.";
                }
                else
                {
                    TempData["Error"] = $"Unable to reject Claim #{id}.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting claim #{ClaimId}", id);
                TempData["Error"] = "An error occurred while rejecting the claim. Please try again.";
            }

            return RedirectToAction("ReviewClaims");
        }

        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var stats = await _claimService.GetClaimStatisticsAsync();
                ViewBag.Statistics = stats;

                var recentClaims = (await _claimService.GetAllClaimsAsync())
                    .OrderByDescending(c => c.DateSubmitted)
                    .Take(10)
                    .ToList();

                return View(recentClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                ViewBag.Statistics = new Dictionary<string, int>
                {
                    { "Total", 0 },
                    { "Pending", 0 },
                    { "CoordinatorApproved", 0 },
                    { "Approved", 0 },
                    { "Rejected", 0 }
                };
                TempData["Error"] = "An error occurred while loading the dashboard.";
                return View(new List<Models.Claim>());
            }
        }
    }
}