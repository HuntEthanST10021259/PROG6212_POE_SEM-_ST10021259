using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Services;

namespace PROG6212_POE_ST10021259.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager/ApproveApprovedClaims
        public IActionResult ApproveApprovedClaims()
        {
            try
            {
                var approvedClaims = ClaimService.GetApprovedClaims();
                return View(approvedClaims);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading approved claims. Please try again.";
                return View(new List<Models.Claim>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalApprove(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Invalid claim ID.";
                    return RedirectToAction("ApproveApprovedClaims");
                }

                var claim = ClaimService.GetClaimById(id);
                if (claim == null)
                {
                    TempData["Error"] = $"Claim #{id} not found.";
                    return RedirectToAction("ApproveApprovedClaims");
                }

                if (claim.Status != "Approved")
                {
                    TempData["Error"] = $"Claim #{id} has not been approved by coordinator yet.";
                    return RedirectToAction("ApproveApprovedClaims");
                }

                var success = ClaimService.UpdateClaimStatus(id, "FinalApproved");
                if (success)
                {
                    TempData["Message"] = $"Claim #{id} has been finally approved! Payment of R{claim.TotalAmount:F2} can be processed.";
                }
                else
                {
                    TempData["Error"] = $"Failed to approve Claim #{id}.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while approving the claim. Please try again.";
            }

            return RedirectToAction("ApproveApprovedClaims");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalReject(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["Error"] = "Invalid claim ID.";
                    return RedirectToAction("ApproveApprovedClaims");
                }

                var claim = ClaimService.GetClaimById(id);
                if (claim == null)
                {
                    TempData["Error"] = $"Claim #{id} not found.";
                    return RedirectToAction("ApproveApprovedClaims");
                }

                var success = ClaimService.UpdateClaimStatus(id, "Rejected");
                if (success)
                {
                    TempData["Message"] = $"Claim #{id} has been rejected by manager.";
                }
                else
                {
                    TempData["Error"] = $"Failed to reject Claim #{id}.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while rejecting the claim. Please try again.";
            }

            return RedirectToAction("ApproveApprovedClaims");
        }
    }
}