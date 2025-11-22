using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Models;
using PROG6212_POE_ST10021259.Services;

namespace PROG6212_POE_ST10021259.Controllers
{
    [Authorize(Roles = "HR")]
    public class HRController : Controller
    {
        private readonly ILogger<HRController> _logger;
        private readonly IClaimService _claimService;
        private readonly ILecturerService _lecturerService;

        public HRController(
            ILogger<HRController> logger,
            IClaimService claimService,
            ILecturerService lecturerService)
        {
            _logger = logger;
            _claimService = claimService;
            _lecturerService = lecturerService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var stats = await _claimService.GetClaimStatisticsAsync();
            var approvedClaims = await _claimService.GetApprovedClaimsAsync();
            var totalPayable = approvedClaims.Sum(c => c.TotalAmount);

            ViewBag.Statistics = stats;
            ViewBag.TotalPayable = totalPayable;
            ViewBag.LecturerCount = (await _lecturerService.GetAllLecturersAsync()).Count;

            return View(approvedClaims);
        }

        public async Task<IActionResult> ApprovedClaims()
        {
            var claims = await _claimService.GetApprovedClaimsAsync();
            return View(claims);
        }

        public async Task<IActionResult> GenerateInvoice(int? id)
        {
            if (id.HasValue)
            {
                var claim = await _claimService.GetClaimByIdAsync(id.Value);
                if (claim == null || claim.Status != "Approved")
                {
                    TempData["Error"] = "Claim not found or not approved.";
                    return RedirectToAction("ApprovedClaims");
                }
                return View("SingleInvoice", claim);
            }

            var claims = await _claimService.GetApprovedClaimsAsync();
            return View(claims);
        }

        public async Task<IActionResult> PaymentReport()
        {
            var claims = await _claimService.GetApprovedClaimsAsync();
            var report = new PaymentReportViewModel
            {
                GeneratedDate = DateTime.Now,
                Claims = claims,
                TotalAmount = claims.Sum(c => c.TotalAmount),
                TotalClaims = claims.Count
            };
            return View(report);
        }

        // Lecturer Management
        public async Task<IActionResult> ManageLecturers()
        {
            var lecturers = await _lecturerService.GetAllLecturersAsync();
            return View(lecturers);
        }

        [HttpGet]
        public IActionResult AddLecturer()
        {
            return View(new Lecturer());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLecturer(Lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                lecturer.DateAdded = DateTime.Now;
                await _lecturerService.AddLecturerAsync(lecturer);

                TempData["Message"] = $"Lecturer {lecturer.FullName} added successfully.";
                return RedirectToAction("ManageLecturers");
            }
            return View(lecturer);
        }

        [HttpGet]
        public async Task<IActionResult> EditLecturer(int id)
        {
            var lecturer = await _lecturerService.GetLecturerByIdAsync(id);
            if (lecturer == null)
            {
                TempData["Error"] = "Lecturer not found.";
                return RedirectToAction("ManageLecturers");
            }
            return View(lecturer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLecturer(Lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                var success = await _lecturerService.UpdateLecturerAsync(lecturer);
                if (success)
                {
                    TempData["Message"] = "Lecturer updated successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to update lecturer.";
                }
                return RedirectToAction("ManageLecturers");
            }
            return View(lecturer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLecturer(int id)
        {
            var success = await _lecturerService.DeleteLecturerAsync(id);
            if (success)
            {
                TempData["Message"] = "Lecturer deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete lecturer.";
            }
            return RedirectToAction("ManageLecturers");
        }
    }
}