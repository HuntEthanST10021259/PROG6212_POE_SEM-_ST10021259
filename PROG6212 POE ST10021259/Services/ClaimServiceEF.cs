using Microsoft.EntityFrameworkCore;
using PROG6212_POE_ST10021259.Data;
using PROG6212_POE_ST10021259.Models;

namespace PROG6212_POE_ST10021259.Services
{
    public class ClaimServiceEF : IClaimService
    {
        private readonly ApplicationDbContext _context;

        public ClaimServiceEF(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddClaimAsync(Claim claim)
        {
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
            return claim.Id;
        }

        public async Task<List<Claim>> GetAllClaimsAsync()
        {
            return await _context.Claims.OrderByDescending(c => c.DateSubmitted).ToListAsync();
        }

        public async Task<List<Claim>> GetPendingClaimsAsync()
        {
            return await _context.Claims.Where(c => c.Status == "Pending").ToListAsync();
        }

        public async Task<List<Claim>> GetCoordinatorApprovedClaimsAsync()
        {
            return await _context.Claims.Where(c => c.Status == "CoordinatorApproved").ToListAsync();
        }

        public async Task<List<Claim>> GetApprovedClaimsAsync()
        {
            return await _context.Claims.Where(c => c.Status == "Approved").ToListAsync();
        }

        public async Task<List<Claim>> GetRejectedClaimsAsync()
        {
            return await _context.Claims.Where(c => c.Status == "Rejected").ToListAsync();
        }

        public async Task<Claim?> GetClaimByIdAsync(int id)
        {
            return await _context.Claims.FindAsync(id);
        }

        public async Task<bool> CoordinatorApproveClaimAsync(int id, string coordinatorName)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null && claim.Status == "Pending")
            {
                claim.Status = "CoordinatorApproved";
                claim.CoordinatorApprovedDate = DateTime.Now;
                claim.CoordinatorApprovedBy = coordinatorName;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ManagerApproveClaimAsync(int id, string managerName)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null && claim.Status == "CoordinatorApproved")
            {
                claim.Status = "Approved";
                claim.ManagerApprovedDate = DateTime.Now;
                claim.ManagerApprovedBy = managerName;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RejectClaimAsync(int id, string rejectedBy, string reason)
        {
            var claim = await _context.Claims.FindAsync(id);
            if (claim != null && (claim.Status == "Pending" || claim.Status == "CoordinatorApproved"))
            {
                claim.Status = "Rejected";
                claim.RejectedDate = DateTime.Now;
                claim.RejectedBy = rejectedBy;
                claim.RejectionReason = reason;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Dictionary<string, int>> GetClaimStatisticsAsync()
        {
            var claims = await _context.Claims.ToListAsync();
            return new Dictionary<string, int>
            {
                { "Total", claims.Count },
                { "Pending", claims.Count(c => c.Status == "Pending") },
                { "CoordinatorApproved", claims.Count(c => c.Status == "CoordinatorApproved") },
                { "Approved", claims.Count(c => c.Status == "Approved") },
                { "Rejected", claims.Count(c => c.Status == "Rejected") }
            };
        }

        public async Task<List<Claim>> GetClaimsByUserAsync(string userId)
        {
            return await _context.Claims.Where(c => c.SubmittedByUserId == userId)
                .OrderByDescending(c => c.DateSubmitted).ToListAsync();
        }
    }
}