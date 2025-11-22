using PROG6212_POE_ST10021259.Models;

namespace PROG6212_POE_ST10021259.Services
{
    public interface IClaimService
    {
        Task<int> AddClaimAsync(Claim claim);
        Task<List<Claim>> GetAllClaimsAsync();
        Task<List<Claim>> GetPendingClaimsAsync();
        Task<List<Claim>> GetCoordinatorApprovedClaimsAsync();
        Task<List<Claim>> GetApprovedClaimsAsync();
        Task<List<Claim>> GetRejectedClaimsAsync();
        Task<Claim?> GetClaimByIdAsync(int id);
        Task<bool> CoordinatorApproveClaimAsync(int id, string coordinatorName);
        Task<bool> ManagerApproveClaimAsync(int id, string managerName);
        Task<bool> RejectClaimAsync(int id, string rejectedBy, string reason);
        Task<Dictionary<string, int>> GetClaimStatisticsAsync();
        Task<List<Claim>> GetClaimsByUserAsync(string userId);
    }
}