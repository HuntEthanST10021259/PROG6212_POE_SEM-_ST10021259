using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Models;
using PROG6212_POE_ST10021259.Services;
using System.IO;

namespace PROG6212_POE_ST10021259.Controllers
{
    public class LecturerController : Controller
    {
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitClaim(Claim claim, IFormFile? supportingDocument)
        {
            if (ModelState.IsValid)
            {
                // Assign unique ID from shared service
                claim.Id = ClaimService.GetNextId();
                claim.DateSubmitted = DateTime.Now;
                claim.Status = "Pending";

                // Handle file upload
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{claim.Id}_{supportingDocument.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        supportingDocument.CopyTo(stream);
                    }

                    claim.SupportingDocument = uniqueFileName;
                }

                // Save claim to shared storage
                ClaimService.AddClaim(claim);

                TempData["Message"] = $"Claim submitted successfully! Claim ID: {claim.Id}, Total Amount: R{claim.TotalAmount:F2}";
                return RedirectToAction("SubmitClaim");
            }

            return View(claim);
        }

        [HttpGet]
        public IActionResult TrackStatus()
        {
            var claims = ClaimService.GetAllClaims();
            return View(claims);
        }

        public IActionResult Test()
        {
            return Content("Lecturer controller is working!");
        }
    }
}