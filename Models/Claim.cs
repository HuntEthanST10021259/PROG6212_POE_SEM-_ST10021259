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

        public string Status { get; set; } = "Pending";

        // Calculated property
        public double TotalAmount => HoursWorked * HourlyRate;
    }
}