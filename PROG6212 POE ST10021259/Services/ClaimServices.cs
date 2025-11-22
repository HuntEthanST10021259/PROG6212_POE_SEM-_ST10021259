namespace PROG6212_POE_ST10021259.Services
{
    public static class ClaimService
    {
        private static List<Models.Claim> _claims = new List<Models.Claim>();
        private static int _nextId = 1;
        private static readonly object _lock = new object();

        public static int GetNextId()
        {
            lock (_lock)
            {
                return _nextId++;
            }
        }

        public static void AddClaim(Models.Claim claim)
        {
            lock (_lock)
            {
                _claims.Add(claim);
            }
        }

        public static List<Models.Claim> GetAllClaims()
        {
            lock (_lock)
            {
                return _claims.ToList();
            }
        }

        public static List<Models.Claim> GetPendingClaims()
        {
            lock (_lock)
            {
                return _claims.Where(c => c.Status == "Pending").ToList();
            }
        }

        public static List<Models.Claim> GetCoordinatorApprovedClaims()
        {
            lock (_lock)
            {
                return _claims.Where(c => c.Status == "CoordinatorApproved").ToList();
            }
        }

        public static List<Models.Claim> GetApprovedClaims()
        {
            lock (_lock)
            {
                return _claims.Where(c => c.Status == "Approved").ToList();
            }
        }

        public static List<Models.Claim> GetRejectedClaims()
        {
            lock (_lock)
            {
                return _claims.Where(c => c.Status == "Rejected").ToList();
            }
        }

        public static Models.Claim? GetClaimById(int id)
        {
            lock (_lock)
            {
                return _claims.FirstOrDefault(c => c.Id == id);
            }
        }

        public static bool CoordinatorApproveClaim(int id, string coordinatorName)
        {
            lock (_lock)
            {
                var claim = _claims.FirstOrDefault(c => c.Id == id);
                if (claim != null && claim.Status == "Pending")
                {
                    claim.Status = "CoordinatorApproved";
                    claim.CoordinatorApprovedDate = DateTime.Now;
                    claim.CoordinatorApprovedBy = coordinatorName;
                    return true;
                }
                return false;
            }
        }

        public static bool ManagerApproveClaim(int id, string managerName)
        {
            lock (_lock)
            {
                var claim = _claims.FirstOrDefault(c => c.Id == id);
                if (claim != null && claim.Status == "CoordinatorApproved")
                {
                    claim.Status = "Approved";
                    claim.ManagerApprovedDate = DateTime.Now;
                    claim.ManagerApprovedBy = managerName;
                    return true;
                }
                return false;
            }
        }

        public static bool RejectClaim(int id, string rejectedBy, string reason)
        {
            lock (_lock)
            {
                var claim = _claims.FirstOrDefault(c => c.Id == id);
                if (claim != null && (claim.Status == "Pending" || claim.Status == "CoordinatorApproved"))
                {
                    claim.Status = "Rejected";
                    claim.RejectedDate = DateTime.Now;
                    claim.RejectedBy = rejectedBy;
                    claim.RejectionReason = reason;
                    return true;
                }
                return false;
            }
        }

        public static bool UpdateClaimStatus(int id, string status)
        {
            lock (_lock)
            {
                var claim = _claims.FirstOrDefault(c => c.Id == id);
                if (claim != null)
                {
                    claim.Status = status;
                    return true;
                }
                return false;
            }
        }

        public static Dictionary<string, int> GetClaimStatistics()
        {
            lock (_lock)
            {
                return new Dictionary<string, int>
                {
                    { "Total", _claims.Count },
                    { "Pending", _claims.Count(c => c.Status == "Pending") },
                    { "CoordinatorApproved", _claims.Count(c => c.Status == "CoordinatorApproved") },
                    { "Approved", _claims.Count(c => c.Status == "Approved") },
                    { "Rejected", _claims.Count(c => c.Status == "Rejected") }
                };
            }
        }
    }
}