using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PROG6212_POE_ST10021259.Models;

namespace PROG6212_POE_ST10021259.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Claim entity
            builder.Entity<Claim>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.HoursWorked).IsRequired();
                entity.Property(e => e.HourlyRate).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.SupportingDocument).HasMaxLength(500);
                entity.Property(e => e.RejectionReason).HasMaxLength(500);
            });

            // Configure Lecturer entity
            builder.Entity<Lecturer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.EmployeeNumber).HasMaxLength(50);
                entity.Property(e => e.BankAccount).HasMaxLength(50);
                entity.Property(e => e.BankName).HasMaxLength(100);
                entity.Property(e => e.BranchCode).HasMaxLength(20);
            });
        }
    }
}