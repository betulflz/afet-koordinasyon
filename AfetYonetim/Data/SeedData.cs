using AfetYonetim.Models.Entities;
using AfetYonetim.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            // ---- Temel Kullanıcılar (Admin + Ayşe + Mehmet) ----
            var adminUser = await EnsureUser(userManager,
                "admin@afet.gov.tr", "Admin123!", "Sistem", "Yöneticisi", null, "Admin");
            var ayseUser = await EnsureUser(userManager,
                "ayse@example.com", "Ayse123!", "Ayşe", "Yılmaz", "05551234567", "Afetzede");
            var mehmetUser = await EnsureUser(userManager,
                "mehmet@example.com", "Mehmet123!", "Mehmet", "Kaya", "05559876543", "Gonullu");

            // ---- Faz 4: Yeni Demo Kullanıcılar (Zenginleştirme) ----
            var fatma = await EnsureUser(userManager, "fatma@example.com", "Demo123!", "Fatma", "Demir", "05441111111", "Afetzede");
            var ali = await EnsureUser(userManager, "ali@example.com", "Demo123!", "Ali", "Vural", "05442222222", "Afetzede");
            var zehra = await EnsureUser(userManager, "zehra@example.com", "Demo123!", "Zehra", "Aslan", "05443333333", "Afetzede");
            var mustafa = await EnsureUser(userManager, "mustafa@example.com", "Demo123!", "Mustafa", "Öz", "05444444444", "Afetzede");
            var selinAfet = await EnsureUser(userManager, "selin@example.com", "Demo123!", "Selin", "Korkmaz", "05445555555", "Afetzede");
            
            var ahmet = await EnsureUser(userManager, "ahmet@example.com", "Demo123!", "Ahmet", "Çelik", "05446666666", "Gonullu");
            var burak = await EnsureUser(userManager, "burak@example.com", "Demo123!", "Burak", "Öztürk", "05447777777", "Gonullu");
            var selinGonullu = await EnsureUser(userManager, "selin.gonullu@example.com", "Demo123!", "Selin", "Demir", "05448888888", "Gonullu");

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // ---- Bölgeler ----
            if (!context.Regions.Any())
            {
                context.Regions.AddRange(
                    new Region { RegionName = "Hatay Merkez", City = "Hatay", District = "Antakya", RiskLevel = RiskLevel.Kritik, Latitude = 36.2025, Longitude = 36.1602, IsActive = true },
                    new Region { RegionName = "Kahramanmaraş Merkez", City = "Kahramanmaraş", District = "Dulkadiroğlu", RiskLevel = RiskLevel.Kritik, Latitude = 37.5858, Longitude = 36.9371, IsActive = true },
                    new Region { RegionName = "Adıyaman Merkez", City = "Adıyaman", District = "Merkez", RiskLevel = RiskLevel.Yuksek, Latitude = 37.7648, Longitude = 38.2786, IsActive = true },
                    new Region { RegionName = "Gaziantep Merkez", City = "Gaziantep", District = "Şahinbey", RiskLevel = RiskLevel.Orta, Latitude = 37.0662, Longitude = 37.3833, IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // ---- Talepler, Atamalar ve Duyurular (Sadece DB boşsa) ----
            if (!context.HelpRequests.Any())
            {
                var regions = await context.Regions.ToListAsync();
                var hatay = regions.First(r => r.RegionName == "Hatay Merkez");
                var maras = regions.First(r => r.RegionName == "Kahramanmaraş Merkez");
                var adiyaman = regions.First(r => r.RegionName == "Adıyaman Merkez");
                var gantep = regions.First(r => r.RegionName == "Gaziantep Merkez");

                var afetzedeler = new[] { ayseUser, fatma, ali, zehra, mustafa, selinAfet };
                var gonulluler = new[] { mehmetUser, ahmet, burak, selinGonullu };

                var rnd = new Random(42); 
                var now = DateTime.UtcNow;
                var requests = new List<HelpRequest>();

                // 25 Talep Şablonu
                var template = new (RequestStatus s, int daysAgo, HelpCategory c, UrgencyLevel u, Region r, ApplicationUser usr, string desc)[]
                {
                    // Bekliyor (6)
                    (RequestStatus.Bekliyor, 0, HelpCategory.Gida, UrgencyLevel.Yuksek, hatay, fatma, "Aile için 1 haftalık gıda paketi gerekli"),
                    (RequestStatus.Bekliyor, 0, HelpCategory.Su, UrgencyLevel.Yuksek, hatay, ali, "İçme suyu, en az 20 litre"),
                    (RequestStatus.Bekliyor, 1, HelpCategory.Ilac, UrgencyLevel.Orta, maras, zehra, "Hipertansiyon ilacı (norvasc)"),
                    (RequestStatus.Bekliyor, 1, HelpCategory.Cadir, UrgencyLevel.Yuksek, adiyaman, mustafa, "4 kişilik aile çadırı"),
                    (RequestStatus.Bekliyor, 2, HelpCategory.Giysi, UrgencyLevel.Dusuk, gantep, selinAfet, "Çocuk kıyafeti, 5-7 yaş"),
                    (RequestStatus.Bekliyor, 2, HelpCategory.Su, UrgencyLevel.Orta, hatay, ayseUser, "Bebek maması ve süt"),

                    // Onaylandi (4)
                    (RequestStatus.Onaylandi, 2, HelpCategory.Gida, UrgencyLevel.Orta, maras, fatma, "Konserve gıda paketi"),
                    (RequestStatus.Onaylandi, 3, HelpCategory.Ilac, UrgencyLevel.Yuksek, hatay, ali, "Acil bandaj ve antiseptik"),
                    (RequestStatus.Onaylandi, 3, HelpCategory.Diger, UrgencyLevel.Dusuk, gantep, zehra, "Battaniye, 3 adet"),
                    (RequestStatus.Onaylandi, 4, HelpCategory.Cadir, UrgencyLevel.Orta, adiyaman, mustafa, "Yardımcı çadır"),

                    // Atandi (5)
                    (RequestStatus.Atandi, 3, HelpCategory.Gida, UrgencyLevel.Yuksek, hatay, selinAfet, "Acil gıda yardımı"),
                    (RequestStatus.Atandi, 4, HelpCategory.Su, UrgencyLevel.Orta, maras, ayseUser, "İçme suyu"),
                    (RequestStatus.Atandi, 4, HelpCategory.Cadir, UrgencyLevel.Yuksek, adiyaman, fatma, "Aile çadırı"),
                    (RequestStatus.Atandi, 5, HelpCategory.Ilac, UrgencyLevel.Orta, hatay, ali, "Reçeteli ilaç"),
                    (RequestStatus.Atandi, 5, HelpCategory.Giysi, UrgencyLevel.Dusuk, gantep, zehra, "Yetişkin kıyafet"),

                    // Yolda (3)
                    (RequestStatus.Yolda, 1, HelpCategory.Gida, UrgencyLevel.Yuksek, hatay, mustafa, "Gıda paketi (yolda)"),
                    (RequestStatus.Yolda, 1, HelpCategory.Su, UrgencyLevel.Orta, maras, selinAfet, "Su (yolda)"),
                    (RequestStatus.Yolda, 2, HelpCategory.Ilac, UrgencyLevel.Yuksek, adiyaman, ayseUser, "İlaç (yolda)"),

                    // TeslimEdildi (5)
                    (RequestStatus.TeslimEdildi, 4, HelpCategory.Gida, UrgencyLevel.Yuksek, hatay, fatma, "Teslim: gıda paketi"),
                    (RequestStatus.TeslimEdildi, 5, HelpCategory.Su, UrgencyLevel.Orta, maras, ali, "Teslim: su"),
                    (RequestStatus.TeslimEdildi, 5, HelpCategory.Ilac, UrgencyLevel.Yuksek, hatay, zehra, "Teslim: ilaç"),
                    (RequestStatus.TeslimEdildi, 6, HelpCategory.Cadir, UrgencyLevel.Orta, adiyaman, mustafa, "Teslim: çadır"),
                    (RequestStatus.TeslimEdildi, 6, HelpCategory.Giysi, UrgencyLevel.Dusuk, gantep, selinAfet, "Teslim: kıyafet"),

                    // Reddedildi (1)
                    (RequestStatus.Reddedildi, 5, HelpCategory.Diger, UrgencyLevel.Dusuk, hatay, ayseUser, "Mükerrer kayıt nedeniyle reddedildi."),

                    // IptalEdildi (1)
                    (RequestStatus.IptalEdildi, 4, HelpCategory.Su, UrgencyLevel.Orta, maras, fatma, "Kendi imkanlarıyla su temin etti."),
                };

                foreach (var t in template)
                {
                    var latVar = (rnd.NextDouble() - 0.5) * 0.1;
                    var lngVar = (rnd.NextDouble() - 0.5) * 0.1;
                    requests.Add(new HelpRequest
                    {
                        UserId = t.usr.Id,
                        Category = t.c,
                        Description = t.desc,
                        Location = t.r.City + " · " + t.r.District,
                        RegionId = t.r.Id,
                        Latitude = t.r.Latitude + latVar,
                        Longitude = t.r.Longitude + lngVar,
                        Urgency = t.u,
                        Status = t.s,
                        CreatedAt = now.AddDays(-t.daysAgo).AddHours(-rnd.Next(0, 12)),
                        UpdatedAt = t.s != RequestStatus.Bekliyor ? now.AddDays(-t.daysAgo).AddHours(rnd.Next(1, 12)) : null
                    });
                }

                context.HelpRequests.AddRange(requests);
                await context.SaveChangesAsync();

                // ---- Atamalar ----
                var assignedRequests = requests.Where(r => r.Status == RequestStatus.Atandi || r.Status == RequestStatus.Yolda || r.Status == RequestStatus.TeslimEdildi).ToList();
                var assignments = new List<Assignment>();
                int gIdx = 0;
                foreach (var req in assignedRequests)
                {
                    var gonullu = gonulluler[gIdx % gonulluler.Length];
                    gIdx++;

                    assignments.Add(new Assignment
                    {
                        HelpRequestId = req.Id,
                        VolunteerId = gonullu.Id,
                        AssignedByAdminId = adminUser.Id,
                        AssignedDate = req.CreatedAt.AddHours(2),
                        DeliveryDate = req.Status == RequestStatus.TeslimEdildi ? req.CreatedAt.AddHours(8) : null,
                        Status = req.Status == RequestStatus.Atandi ? AssignmentStatus.Atandi : 
                                 (req.Status == RequestStatus.Yolda ? AssignmentStatus.Yolda : AssignmentStatus.TeslimEdildi),
                        Notes = req.Status == RequestStatus.TeslimEdildi ? "Sorunsuz teslim edildi." : null,
                        CreatedAt = req.CreatedAt.AddHours(2)
                    });
                }
                context.Assignments.AddRange(assignments);
                await context.SaveChangesAsync();

                // ---- Duyurular ----
                context.Announcements.AddRange(
                    new Announcement { CreatedByAdminId = adminUser.Id, Title = "Hatay bölgesinde su talebinde artış", Content = "Antakya bölgesinde içme suyu talepleri arttı. Gönüllülerin bu bölgeye yoğunlaşması rica olunur.", IsUrgent = true, CreatedAt = now.AddHours(-2) },
                    new Announcement { CreatedByAdminId = adminUser.Id, Title = "Yeni gönüllü kayıt kampanyası", Content = "Sahaya katılmak isteyenler için yeni eğitim materyalleri yüklendi.", IsUrgent = false, CreatedAt = now.AddDays(-1) },
                    new Announcement { CreatedByAdminId = adminUser.Id, Title = "Gönüllü eğitim toplantısı", Content = "Cumartesi günü çevrimiçi bilgilendirme toplantısı yapılacaktır.", IsUrgent = false, CreatedAt = now.AddDays(-2) }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser> EnsureUser(UserManager<ApplicationUser> userManager, string email, string password, string name, string surname, string? phone, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null) return user;

            user = new ApplicationUser { UserName = email, Email = email, Name = name, Surname = surname, EmailConfirmed = true, IsActive = true, PhoneNumber = phone, CreatedAt = DateTime.UtcNow };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded) await userManager.AddToRoleAsync(user, role);
            return user;
        }
    }
}