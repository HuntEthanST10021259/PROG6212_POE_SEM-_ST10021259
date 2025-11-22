using PROG6212_POE_ST10021259.Models;

namespace PROG6212_POE_ST10021259.Services
{
    public static class LecturerService
    {
        private static List<Lecturer> _lecturers = new List<Lecturer>();
        private static int _nextId = 1;
        private static readonly object _lock = new object();

        public static int GetNextId()
        {
            lock (_lock)
            {
                return _nextId++;
            }
        }

        public static void AddLecturer(Lecturer lecturer)
        {
            lock (_lock)
            {
                _lecturers.Add(lecturer);
            }
        }

        public static List<Lecturer> GetAllLecturers()
        {
            lock (_lock)
            {
                return _lecturers.ToList();
            }
        }

        public static Lecturer? GetLecturerById(int id)
        {
            lock (_lock)
            {
                return _lecturers.FirstOrDefault(l => l.Id == id);
            }
        }

        public static bool UpdateLecturer(Lecturer updated)
        {
            lock (_lock)
            {
                var lecturer = _lecturers.FirstOrDefault(l => l.Id == updated.Id);
                if (lecturer != null)
                {
                    lecturer.FirstName = updated.FirstName;
                    lecturer.LastName = updated.LastName;
                    lecturer.Email = updated.Email;
                    lecturer.Phone = updated.Phone;
                    lecturer.Department = updated.Department;
                    lecturer.EmployeeNumber = updated.EmployeeNumber;
                    lecturer.BankAccount = updated.BankAccount;
                    lecturer.BankName = updated.BankName;
                    lecturer.BranchCode = updated.BranchCode;
                    lecturer.IsActive = updated.IsActive;
                    lecturer.LastUpdated = DateTime.Now;
                    return true;
                }
                return false;
            }
        }

        public static bool DeleteLecturer(int id)
        {
            lock (_lock)
            {
                var lecturer = _lecturers.FirstOrDefault(l => l.Id == id);
                if (lecturer != null)
                {
                    _lecturers.Remove(lecturer);
                    return true;
                }
                return false;
            }
        }
    }
}