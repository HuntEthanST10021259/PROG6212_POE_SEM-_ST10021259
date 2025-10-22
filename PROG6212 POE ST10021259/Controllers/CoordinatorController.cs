using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Services;

namespace PROG6212_POE_ST10021259.Controllers
{
    public class CoordinatorController : Controller
    {
        // GET: Coordinator/VerifyClaims
        public IActionResult VerifyClaims()
        {
            try
            {
                var pendingClaims = ClaimService.GetPendingClaims();
                return View(pendingClaims);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading pending claims. Please try again.";
                return View(new List<Models.Claim>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Invalid claim ID.";
                    return RedirectToAction("VerifyClaims");
                }

                var success = ClaimService.UpdateClaimStatus(id, "Approved");
                if (success)
                {
                    TempData["Message"] = $"Claim #{id} has been approved successfully!";
                }
                else
                {
                    TempData["Error"] = $"Claim #{id} not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while approving the claim. Please try again.";
            }

            return RedirectToAction("VerifyClaims");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Invalid claim ID.";
                    return RedirectToAction("VerifyClaims");
                }

                var success = ClaimService.UpdateClaimStatus(id, "Rejected");
                if (success)
                {
                    TempData["Message"] = $"Claim #{id} has been rejected.";
                }
                else
                {
                    TempData["Error"] = $"Claim #{id} not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while rejecting the claim. Please try again.";
            }

            return RedirectToAction("VerifyClaims");
        }
    }
}