using Microsoft.AspNetCore.Mvc;
using PROG6212_POE_ST10021259.Models;
using System.IO;

namespace PROG6212_POE_ST10021259.Controllers
{
    public class LecturerController : Controller
    {
        // Static list to store claims (in-memory storage)
        private static List<Claim> _claims = new List<Claim>();
        private static int _nextId = 1;

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
                // Assign unique ID
                claim.Id = _nextId++;
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

                // Save claim to storage
                _claims.Add(claim);

                TempData["Message"] = $"Claim submitted successfully! Claim ID: {claim.Id}, Total Amount: R{claim.TotalAmount:F2}";
                return RedirectToAction("SubmitClaim");
            }

            return View(claim);
        }

        [HttpGet]
        public IActionResult TrackStatus()
        {
            return View(_claims);
        }

        public IActionResult Test()
        {
            return Content("Lecturer controller is working!");
        }
    }
}