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

            // Ensure Staff role exists
            if (!await roleManager.RoleExistsAsync("Staff"))
            {
                await roleManager.CreateAsync(new IdentityRole("Staff"));
            }

            // Make THIS email a Staff user (change if you want)
            var staffEmail = "staff@comicmvc.local";

            var staffUser = await userManager.FindByEmailAsync(staffEmail);
            if (staffUser != null && !await userManager.IsInRoleAsync(staffUser, "Staff"))
            {
                await userManager.AddToRoleAsync(staffUser, "Staff");
            }
        }
    }
}
