using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Models;
using PROG6212_POE_ST10021259.Services;

namespace PROG6212_POE_ST10021259.Controllers
{
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : Controller
    {
        private readonly ILogger<LecturerController> _logger;
        private readonly IClaimService _claimService;
        private readonly ClaimValidationService _validationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private const long MaxFileSize = 10 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".xlsx" };

        public LecturerController(
            ILogger<LecturerController> logger,
            IClaimService claimService,
            ClaimValidationService validationService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _claimService = claimService;
            _validationService = validationService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile? supportingDocument)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var validationResult = _validationService.ValidateClaim(claim);
                    if (!validationResult.IsValid)
                    {
                        foreach (var error in validationResult.Errors)
                            ModelState.AddModelError("", error);
                        return View(claim);
                    }

                    if (supportingDocument != null && supportingDocument.Length > 0)
                    {
                        if (supportingDocument.Length > MaxFileSize)
                        {
                            ModelState.AddModelError("supportingDocument", "File size cannot exceed 10MB.");
                            return View(claim);
                        }

                        var ext = Path.GetExtension(supportingDocument.FileName).ToLowerInvariant();
                        if (!AllowedExtensions.Contains(ext))
                        {
                            ModelState.AddModelError("supportingDocument", "Only PDF, DOC, DOCX, and XLSX files are allowed.");
                            return View(claim);
                        }

                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await supportingDocument.CopyToAsync(stream);

                        claim.SupportingDocument = uniqueFileName;
                    }

                    var user = await _userManager.GetUserAsync(User);
                    claim.SubmittedByUserId = user?.Id;
                    claim.SubmittedByName = user?.FullName;
                    claim.DateSubmitted = DateTime.Now;
                    claim.Status = "Pending";

                    await _claimService.AddClaimAsync(claim);

                    var warningMsg = validationResult.Warnings.Count > 0
                        ? " Note: " + string.Join("; ", validationResult.Warnings)
                        : "";

                    TempData["Message"] = $"Claim submitted successfully! Total: R{claim.TotalAmount:F2}.{warningMsg}";
                    return RedirectToAction("SubmitClaim");
                }
                return View(claim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting claim");
                TempData["Error"] = "An error occurred. Please try again.";
                return View(claim);
            }
        }

        [HttpGet]
        public async Task<IActionResult> TrackStatus()
        {
            var user = await _userManager.GetUserAsync(User);
            var claims = await _claimService.GetClaimsByUserAsync(user?.Id ?? "");
            return View(claims);
        }
    }
}