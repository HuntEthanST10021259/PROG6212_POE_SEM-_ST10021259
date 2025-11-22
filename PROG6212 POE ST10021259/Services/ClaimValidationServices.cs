using PROG6212_POE_ST10021259.Models;

namespace PROG6212_POE_ST10021259.Services
{
    public class ClaimValidationService
    {
        private readonly ClaimValidationRules _rules;

        public ClaimValidationService()
        {
            _rules = new ClaimValidationRules();
        }

        public ClaimValidationResult ValidateClaim(Claim claim)
        {
            var result = new ClaimValidationResult
            {
                IsValid = true,
                AutoApprovalEligible = true
            };

            // Check hours worked range
            if (claim.HoursWorked < _rules.MinHoursWorked || claim.HoursWorked > _rules.MaxHoursWorked)
            {
                result.IsValid = false;
                result.Errors.Add($"Hours worked must be between {_rules.MinHoursWorked} and {_rules.MaxHoursWorked}");
            }

            // Check minimum hourly rate
            if (claim.HourlyRate < _rules.MinHourlyRate)
            {
                result.Warnings.Add($"Hourly rate (R{claim.HourlyRate:F2}) is below recommended minimum (R{_rules.MinHourlyRate:F2})");
                result.AutoApprovalEligible = false;
            }

            // Check maximum hourly rate
            if (claim.HourlyRate > _rules.MaxHourlyRate)
            {
                result.Warnings.Add($"Hourly rate (R{claim.HourlyRate:F2}) exceeds standard maximum (R{_rules.MaxHourlyRate:F2})");
                result.AutoApprovalEligible = false;
            }

            // Check total amount threshold
            if (claim.TotalAmount > _rules.MaxTotalAmount)
            {
                result.Warnings.Add($"Total amount (R{claim.TotalAmount:F2}) exceeds maximum threshold (R{_rules.MaxTotalAmount:F2})");
                result.AutoApprovalEligible = false;
            }

            // Check if supporting document is required
            if (claim.TotalAmount > _rules.SupportingDocumentThreshold && string.IsNullOrEmpty(claim.SupportingDocument))
            {
                result.Warnings.Add($"Claims over R{_rules.SupportingDocumentThreshold:F2} require supporting documents");
                result.AutoApprovalEligible = false;
            }

            // Check for unusual hours
            if (claim.HoursWorked > 160)
            {
                result.Warnings.Add($"Hours worked ({claim.HoursWorked}) exceeds typical monthly hours (160)");
            }

            // Set validation message
            result.ValidationMessage = result.IsValid
                ? (result.AutoApprovalEligible
                    ? "Claim is valid and eligible for auto-approval"
                    : "Claim is valid but requires manual review")
                : "Claim has validation errors";

            return result;
        }

        public ClaimValidationRules GetValidationRules()
        {
            return _rules;
        }
    }
}