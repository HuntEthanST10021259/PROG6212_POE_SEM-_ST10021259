using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Models;
using PROG6212_POE_ST10021259.Services;
using System.IO;

namespace PROG6212_POE_ST10021259.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ILogger<LecturerController> _logger;
        private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
        private readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".xlsx" };

        public LecturerController(ILogger<LecturerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitClaim(Claim claim, IFormFile? supportingDocument)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate file if uploaded
                    if (supportingDocument != null && supportingDocument.Length > 0)
                    {
                        // Check file size
                        if (supportingDocument.Length > MaxFileSize)
                        {
                            ModelState.AddModelError("supportingDocument",
                                "File size cannot exceed 10MB.");
                            return View(claim);
                        }

                        // Check file extension
                        var fileExtension = Path.GetExtension(supportingDocument.FileName).ToLowerInvariant();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("supportingDocument",
                                "Only PDF, DOC, DOCX, and XLSX files are allowed.");
                            return View(claim);
                        }

                        // Validate file name
                        if (string.IsNullOrWhiteSpace(supportingDocument.FileName))
                        {
                            ModelState.AddModelError("supportingDocument",
                                "Invalid file name.");
                            return View(claim);
                        }
                    }

                    // Assign unique ID from shared service
                    claim.Id = ClaimService.GetNextId();
                    claim.DateSubmitted = DateTime.Now;
                    claim.Status = "Pending";

                    // Handle file upload
                    if (supportingDocument != null && supportingDocument.Length > 0)
                    {
                        try
                        {
                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                            // Create directory if it doesn't exist
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            // Generate unique file name to prevent overwriting
                            var fileExtension = Path.GetExtension(supportingDocument.FileName);
                            var uniqueFileName = $"{claim.Id}_{Guid.NewGuid()}{fileExtension}";
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            // Save file
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                supportingDocument.CopyTo(stream);
                            }

                            claim.SupportingDocument = uniqueFileName;
                            _logger.LogInformation($"File uploaded successfully: {uniqueFileName}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error uploading file");
                            ModelState.AddModelError("supportingDocument",
                                "An error occurred while uploading the file. Please try again.");
                            return View(claim);
                        }
                    }

                    // Save claim to shared storage
                    ClaimService.AddClaim(claim);

                    TempData["Message"] = $"Claim submitted successfully! Claim ID: {claim.Id}, Total Amount: R{claim.TotalAmount:F2}";
                    _logger.LogInformation($"Claim {claim.Id} submitted successfully");

                    return RedirectToAction("SubmitClaim");
                }

                return View(claim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while submitting claim");
                TempData["Error"] = "An unexpected error occurred while submitting your claim. Please try again or contact support.";
                return View(claim);
            }
        }

        [HttpGet]
        public IActionResult TrackStatus()
        {
            try
            {
                var claims = ClaimService.GetAllClaims();
                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving claims");
                TempData["Error"] = "An error occurred while retrieving your claims. Please try again.";
                return View(new List<Claim>());
            }
        }

        public IActionResult Test()
        {
            return Content("Lecturer controller is working!");
        }
    }
}