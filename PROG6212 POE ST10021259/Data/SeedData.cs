using Microsoft.AspNetCore.Identity;
using PROG6212_POE_ST10021259.Models;

namespace PROG6212_POE_ST10021259.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Create roles
            string[] roles = { "Lecturer", "Coordinator", "Manager", "HR" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create default users
            var users = new[]
            {
                new { Email = "lecturer@test.com", Password = "Lecturer123!", Role = "Lecturer", First = "John", Last = "Lecturer" },
                new { Email = "coordinator@test.com", Password = "Coordinator123!", Role = "Coordinator", First = "Jane", Last = "Coordinator" },
                new { Email = "manager@test.com", Password = "Manager123!", Role = "Manager", First = "Mike", Last = "Manager" },
                new { Email = "hr@test.com", Password = "Hr123456!", Role = "HR", First = "Sarah", Last = "HR" }
            };

            foreach (var u in users)
            {
                if (await userManager.FindByEmailAsync(u.Email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        FirstName = u.First,
                        LastName = u.Last,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, u.Password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, u.Role);
                    }
                }
            }
        }
    }
}