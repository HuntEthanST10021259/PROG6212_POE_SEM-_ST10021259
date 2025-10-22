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

        public static Models.Claim? GetClaimById(int id)
        {
            lock (_lock)
            {
                return _claims.FirstOrDefault(c => c.Id == id);
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
    }
}