using PROG6212_POE_ST10021259.Models;

namespace PROG6212_POE_ST10021259.Services
{
    public interface ILecturerService
    {
        Task<int> AddLecturerAsync(Lecturer lecturer);
        Task<List<Lecturer>> GetAllLecturersAsync();
        Task<Lecturer?> GetLecturerByIdAsync(int id);
        Task<bool> UpdateLecturerAsync(Lecturer lecturer);
        Task<bool> DeleteLecturerAsync(int id);
    }
}