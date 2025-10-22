using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Services;

namespace PROG6212_POE_ST10021259.Controllers
{
    public class CoordinatorController : Controller
    {
        // GET: Coordinator/VerifyClaims
        public IActionResult VerifyClaims()
        {
            var pendingClaims = ClaimService.GetPendingClaims();
            return View(pendingClaims);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            var success = ClaimService.UpdateClaimStatus(id, "Approved");
            if (success)
            {
                TempData["Message"] = $"Claim #{id} has been approved successfully!";
            }
            else
            {
                TempData["Error"] = $"Claim #{id} not found.";
            }
            return RedirectToAction("VerifyClaims");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            var success = ClaimService.UpdateClaimStatus(id, "Rejected");
            if (success)
            {
                TempData["Message"] = $"Claim #{id} has been rejected.";
            }
            else
            {
                TempData["Error"] = $"Claim #{id} not found.";
            }
            return RedirectToAction("VerifyClaims");
        }
    }
}