using Graduation.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Graduation.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // لو في يوزرات خلاص مفيش داعي نكمل
            if (await userManager.Users.AnyAsync()) return;

            // إنشاء الـ Roles
            var roles = new List<AppRole>
            {
                new AppRole { Name = "Member" },
                new AppRole { Name = "Admin" },
                new AppRole { Name = "Moderator" }
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            // إنشاء يوزر Member تجريبي
            var memberUser = new AppUser
            {
                UserName = "member",
                KnownAs = "Member User",
                Gender = "male"
            };

            await userManager.CreateAsync(memberUser, "Pa$$w0rd");
            await userManager.AddToRoleAsync(memberUser, "Member");

            // إنشاء يوزر Admin تجريبي
            var adminUser = new AppUser
            {
                UserName = "admin",
                KnownAs = "Admin User",
                Gender = "male"
            };

            await userManager.CreateAsync(adminUser, "Pa$$w0rd");
            await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Moderator" });
        }
    }
}
