namespace PROG6212_POE_ST10021259.Models
{
    public class ClaimValidationRules
    {
        public double MinHoursWorked { get; set; } = 0.01;
        public double MaxHoursWorked { get; set; } = 744;
        public double MinHourlyRate { get; set; } = 100.00;
        public double MaxHourlyRate { get; set; } = 2000.00;
        public double MaxTotalAmount { get; set; } = 100000.00;
        public bool RequiresSupportingDocument { get; set; } = false;
        public double SupportingDocumentThreshold { get; set; } = 50000.00;
    }

    public class ClaimValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public bool AutoApprovalEligible { get; set; }
        public string ValidationMessage { get; set; } = string.Empty;
    }
}