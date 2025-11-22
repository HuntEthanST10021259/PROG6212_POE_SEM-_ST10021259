using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Services;

namespace PROG6212_POE_ST10021259.Controllers
{
    [Authorize(Roles = "Coordinator")]
    public class CoordinatorController : Controller
    {
        private readonly ILogger<CoordinatorController> _logger;
        private readonly IClaimService _claimService;

        public CoordinatorController(ILogger<CoordinatorController> logger, IClaimService claimService)
        {
            _logger = logger;
            _claimService = claimService;
        }

        public async Task<IActionResult> VerifyClaims()
        {
            try
            {
                var pendingClaims = await _claimService.GetPendingClaimsAsync();
                return View(pendingClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading pending claims");
                TempData["Error"] = "An error occurred while loading pending claims. Please try again.";
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
                    return RedirectToAction("VerifyClaims");
                }

                var success = await _claimService.CoordinatorApproveClaimAsync(id, "Coordinator");

                if (success)
                {
                    _logger.LogInformation("Coordinator approved claim #{ClaimId}", id);
                    TempData["Message"] = $"Claim #{id} has been approved and forwarded to Manager for final review.";
                }
                else
                {
                    TempData["Error"] = $"Claim #{id} not found or already processed.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving claim #{ClaimId}", id);
                TempData["Error"] = "An error occurred while approving the claim. Please try again.";
            }

            return RedirectToAction("VerifyClaims");
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
                    return RedirectToAction("VerifyClaims");
                }

                if (string.IsNullOrWhiteSpace(reason))
                {
                    reason = "No reason provided";
                }

                var success = await _claimService.RejectClaimAsync(id, "Coordinator", reason);

                if (success)
                {
                    _logger.LogInformation("Coordinator rejected claim #{ClaimId}", id);
                    TempData["Message"] = $"Claim #{id} has been rejected.";
                }
                else
                {
                    TempData["Error"] = $"Claim #{id} not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting claim #{ClaimId}", id);
                TempData["Error"] = "An error occurred while rejecting the claim. Please try again.";
            }

            return RedirectToAction("VerifyClaims");
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