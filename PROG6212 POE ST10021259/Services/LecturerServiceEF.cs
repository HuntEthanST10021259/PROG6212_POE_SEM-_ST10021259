using Microsoft.EntityFrameworkCore;
using PROG6212_POE_ST10021259.Data;
using PROG6212_POE_ST10021259.Models;

namespace PROG6212_POE_ST10021259.Services
{
    public class LecturerServiceEF : ILecturerService
    {
        private readonly ApplicationDbContext _context;

        public LecturerServiceEF(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddLecturerAsync(Lecturer lecturer)
        {
            _context.Lecturers.Add(lecturer);
            await _context.SaveChangesAsync();
            return lecturer.Id;
        }

        public async Task<List<Lecturer>> GetAllLecturersAsync()
        {
            return await _context.Lecturers.OrderBy(l => l.LastName).ToListAsync();
        }

        public async Task<Lecturer?> GetLecturerByIdAsync(int id)
        {
            return await _context.Lecturers.FindAsync(id);
        }

        public async Task<bool> UpdateLecturerAsync(Lecturer updated)
        {
            var lecturer = await _context.Lecturers.FindAsync(updated.Id);
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
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteLecturerAsync(int id)
        {
            var lecturer = await _context.Lecturers.FindAsync(id);
            if (lecturer != null)
            {
                _context.Lecturers.Remove(lecturer);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}