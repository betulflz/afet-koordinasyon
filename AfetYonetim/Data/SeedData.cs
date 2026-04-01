using AfetYonetim.Models.Entities;
using AfetYonetim.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace AfetYonetim.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ---- Rolleri Oluştur ----
            string[] roleNames = { "Admin", "Afetzede", "Gonullu" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // ---- Admin Kullanıcı Oluştur ----
            var adminEmail = "admin@afet.gov.tr";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Sistem",
                    Surname = "Yöneticisi",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // ---- Örnek Afetzede Oluştur ----
            var afetzedeEmail = "ayse@example.com";
            var afetzedeUser = await userManager.FindByEmailAsync(afetzedeEmail);

            if (afetzedeUser == null)
            {
                afetzedeUser = new ApplicationUser
                {
                    UserName = afetzedeEmail,
                    Email = afetzedeEmail,
                    Name = "Ayşe",
                    Surname = "Yılmaz",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    PhoneNumber = "05551234567"
                };

                var result = await userManager.CreateAsync(afetzedeUser, "Ayse123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(afetzedeUser, "Afetzede");
                }
            }

            // ---- Örnek Gönüllü Oluştur ----
            var gonulluEmail = "mehmet@example.com";
            var gonulluUser = await userManager.FindByEmailAsync(gonulluEmail);

            if (gonulluUser == null)
            {
                gonulluUser = new ApplicationUser
                {
                    UserName = gonulluEmail,
                    Email = gonulluEmail,
                    Name = "Mehmet",
                    Surname = "Kaya",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    PhoneNumber = "05559876543"
                };

                var result = await userManager.CreateAsync(gonulluUser, "Mehmet123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(gonulluUser, "Gonullu");
                }
            }

            // ---- Örnek Bölgeler Oluştur ----
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Regions.Any())
            {
                var regions = new List<Region>
                {
                    new Region
                    {
                        RegionName = "Hatay Merkez",
                        City = "Hatay",
                        District = "Antakya",
                        RiskLevel = RiskLevel.Kritik,
                        Latitude = 36.2025,
                        Longitude = 36.1602,
                        IsActive = true
                    },
                    new Region
                    {
                        RegionName = "Kahramanmaraş Merkez",
                        City = "Kahramanmaraş",
                        District = "Dulkadiroğlu",
                        RiskLevel = RiskLevel.Kritik,
                        Latitude = 37.5858,
                        Longitude = 36.9371,
                        IsActive = true
                    },
                    new Region
                    {
                        RegionName = "Adıyaman Merkez",
                        City = "Adıyaman",
                        District = "Merkez",
                        RiskLevel = RiskLevel.Yuksek,
                        Latitude = 37.7648,
                        Longitude = 38.2786,
                        IsActive = true
                    },
                    new Region
                    {
                        RegionName = "Gaziantep Merkez",
                        City = "Gaziantep",
                        District = "Şahinbey",
                        RiskLevel = RiskLevel.Orta,
                        Latitude = 37.0662,
                        Longitude = 37.3833,
                        IsActive = true
                    }
                };

                context.Regions.AddRange(regions);
                await context.SaveChangesAsync();
            }
        }
    }
}