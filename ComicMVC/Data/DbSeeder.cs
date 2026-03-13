using Microsoft.AspNetCore.Identity;

namespace ComicMVC.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            
            if (!await roleManager.RoleExistsAsync("Staff"))
            {
                await roleManager.CreateAsync(new IdentityRole("Staff"));
            }

            // ACCOUNT TO USE FOR VIDEO
            var staffEmail = "staff@comicmvc.local";
            var staffPassword = "Staff123!";

            var staffUser = await userManager.FindByEmailAsync(staffEmail);

            // Create the user(PUBLIC)
            if (staffUser == null)
            {
                staffUser = new IdentityUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(staffUser, staffPassword);

                if (!createResult.Succeeded)
                {
                    throw new Exception("Failed to create staff demo user: " +
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }

            // Add user to Staff role (saferail)
            if (!await userManager.IsInRoleAsync(staffUser, "Staff"))
            {
                var roleResult = await userManager.AddToRoleAsync(staffUser, "Staff");

                if (!roleResult.Succeeded)
                {
                    throw new Exception("Failed to assign Staff role: " +
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}