using ASM.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ASM.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();

            // Tạo role nếu chưa có
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Tạo admin mặc định nếu chưa có
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrator",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, "Admin@123");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Không thể tạo tài khoản admin: " + string.Join(", ", createResult.Errors));
                }
            }
        }

    }
}