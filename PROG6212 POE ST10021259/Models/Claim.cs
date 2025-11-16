using System.ComponentModel.DataAnnotations;

namespace PROG6212_POE_ST10021259.Models
{
    public class Claim
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0.01, 744, ErrorMessage = "Hours worked must be between 0.01 and 744")]
        [Display(Name = "Hours Worked")]
        public double HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0.01, 10000, ErrorMessage = "Hourly rate must be between 0.01 and 10000")]
        [Display(Name = "Hourly Rate (R)")]
        public double HourlyRate { get; set; }

        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }

        public string? SupportingDocument { get; set; }

        public DateTime DateSubmitted { get; set; } = DateTime.Now;

        // Enhanced Status with two-level approval
        public string Status { get; set; } = "Pending";

        // New properties for two-level approval
        public DateTime? CoordinatorApprovedDate { get; set; }
        public string? CoordinatorApprovedBy { get; set; }

        public DateTime? ManagerApprovedDate { get; set; }
        public string? ManagerApprovedBy { get; set; }

        public DateTime? RejectedDate { get; set; }
        public string? RejectedBy { get; set; }
        public string? RejectionReason { get; set; }

        // Calculated property with auto-calculation
        public double TotalAmount => Math.Round(HoursWorked * HourlyRate, 2);

        // Status helper methods
        public bool IsPendingCoordinatorReview => Status == "Pending";
        public bool IsPendingManagerReview => Status == "CoordinatorApproved";
        public bool IsFullyApproved => Status == "Approved";
        public bool IsRejected => Status == "Rejected";
    }
}