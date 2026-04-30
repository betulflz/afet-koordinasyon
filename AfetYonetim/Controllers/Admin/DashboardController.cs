using AfetYonetim.Data;
using AfetYonetim.Extensions;
using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Admin;
using AfetYonetim.Models.ViewModels.Admin.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";

            var model = new DashboardViewModel
            {
                // === Mevcut sayaçlar (Faz 2 — değişmedi) ===
                TotalRequests = await _context.HelpRequests.CountAsync(),
                PendingRequests = await _context.HelpRequests
                    .CountAsync(r => r.Status == RequestStatus.Bekliyor),
                ApprovedRequests = await _context.HelpRequests
                    .CountAsync(r => r.Status == RequestStatus.Onaylandi),
                AssignedRequests = await _context.HelpRequests
                    .CountAsync(r => r.Status == RequestStatus.Atandi || r.Status == RequestStatus.Yolda),
                CompletedRequests = await _context.HelpRequests
                    .CountAsync(r => r.Status == RequestStatus.TeslimEdildi),
                TotalVolunteers = await _context.Users
                    .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                    .Join(_context.Roles, x => x.ur.RoleId, r => r.Id, (x, r) => new { x.u, RoleName = r.Name })
                    .CountAsync(x => x.RoleName == "Gonullu" && !x.u.IsDeleted),
                TotalRegions = await _context.Regions.CountAsync(),
                ActiveAssignments = await _context.Assignments
                    .CountAsync(a => a.Status == AssignmentStatus.Atandi || a.Status == AssignmentStatus.Yolda),

                // === Faz 3: ActivePins ===
                ActivePins = await BuildActivePinsAsync(),

                // === Faz 3: CategoryStats ===
                CategoryStats = await _context.HelpRequests
                    .GroupBy(r => r.Category)
                    .Select(g => new CategoryStat
                    {
                        Category = g.Key.ToString(),
                        Count = g.Count()
                    })
                    .ToListAsync(),

                // === Faz 3: DailyTrend (son 7 gün, eksik günler 0 ile doldurulur) ===
                DailyTrend = await BuildDailyTrendAsync(),

                // === Faz 3: RegionStats (Top 8) ===
                RegionStats = await _context.HelpRequests
                    .Include(r => r.Region)
                    .GroupBy(r => r.Region!.RegionName)
                    .Select(g => new RegionStat
                    {
                        RegionName = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(s => s.Count)
                    .Take(8)
                    .ToListAsync(),

                // === Faz 3: RecentActivities (5 öğe — karışık aktivite) ===
                RecentActivities = await BuildRecentActivitiesAsync()
            };

            return View("~/Views/Admin/Dashboard/Index.cshtml", model);
        }

        // -------- Yardımcı metotlar --------

        private async Task<List<MapPin>> BuildActivePinsAsync()
        {
            var activeStatuses = new[]
            {
                RequestStatus.Bekliyor,
                RequestStatus.Onaylandi,
                RequestStatus.Atandi,
                RequestStatus.Yolda
            };

            var raw = await _context.HelpRequests
                .Where(r => activeStatuses.Contains(r.Status))
                .Include(r => r.Region)
                .Include(r => r.User)
                .Select(r => new
                {
                    r.Id,
                    r.Latitude,
                    r.Longitude,
                    RegLat = r.Region!.Latitude,
                    RegLng = r.Region!.Longitude,
                    RegName = r.Region!.RegionName,
                    Category = r.Category,
                    Status = r.Status,
                    Urgency = r.Urgency,
                    UserName = r.User!.Name + " " + r.User.Surname,
                    Location = r.Location
                })
                .ToListAsync();

            return raw.Select(r => new MapPin
            {
                Id = r.Id,
                Lat = r.Latitude == 0 ? r.RegLat : r.Latitude,
                Lng = r.Longitude == 0 ? r.RegLng : r.Longitude,
                Category = r.Category.GetDisplayName(),
                Status = r.Status.ToString(),
                Urgency = r.Urgency.ToString(),
                Title = r.Category.GetDisplayName() + " Talebi",
                Subtitle = r.UserName + " · " + (string.IsNullOrWhiteSpace(r.Location) ? r.RegName : r.Location),
                DetailUrl = "/Admin/HelpRequest/Details/" + r.Id
            }).ToList();
        }

        private async Task<List<DailyTrend>> BuildDailyTrendAsync()
        {
            var since = DateTime.UtcNow.Date.AddDays(-6);

            var raw = await _context.HelpRequests
                .Where(r => r.CreatedAt >= since)
                .GroupBy(r => r.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            return Enumerable.Range(0, 7)
                .Select(i => since.AddDays(i))
                .Select(d => new DailyTrend
                {
                    Label = d.ToString("dd.MM"),
                    Count = raw.FirstOrDefault(x => x.Date == d)?.Count ?? 0
                })
                .ToList();
        }

        private async Task<List<ActivityItem>> BuildRecentActivitiesAsync()
        {
            // 5 ayrı küçük sorgu — her biri Take(5). Yerel union ile birleştirilir, OrderBy.Take(5).
            var newRequests = await _context.HelpRequests
                .Include(r => r.User)
                .Include(r => r.Region)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .Select(r => new ActivityItem
                {
                    Type = "Create",
                    Title = "Yeni talep — " + r.Category.ToString(),
                    Subtitle = r.User!.Name + " " + r.User.Surname + " · " + r.Region!.RegionName,
                    When = r.CreatedAt,
                    IconClass = "fas fa-plus",
                    ColorClass = "bg-info"
                })
                .ToListAsync();

            var approved = await _context.HelpRequests
                .Where(r => r.Status == RequestStatus.Onaylandi
                         || r.Status == RequestStatus.Atandi
                         || r.Status == RequestStatus.Yolda
                         || r.Status == RequestStatus.TeslimEdildi)
                .Include(r => r.User)
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .Take(5)
                .Select(r => new ActivityItem
                {
                    Type = "Approve",
                    Title = "Talep onaylandı",
                    Subtitle = r.User!.Name + " " + r.User.Surname + " · " + r.Category.ToString(),
                    When = r.UpdatedAt ?? r.CreatedAt,
                    IconClass = "fas fa-check",
                    ColorClass = "bg-success"
                })
                .ToListAsync();

            var rejected = await _context.HelpRequests
                .Where(r => r.Status == RequestStatus.Reddedildi)
                .Include(r => r.User)
                .OrderByDescending(r => r.UpdatedAt ?? r.CreatedAt)
                .Take(5)
                .Select(r => new ActivityItem
                {
                    Type = "Reject",
                    Title = "Talep reddedildi",
                    Subtitle = (r.User != null ? r.User.Name + " " + r.User.Surname : "Bilinmeyen") + " · " + r.Category.ToString(),
                    When = r.UpdatedAt ?? r.CreatedAt,
                    IconClass = "fas fa-times",
                    ColorClass = "bg-danger"
                })
                .ToListAsync();

            var assigned = await _context.Assignments
                .Include(a => a.Volunteer)
                .Include(a => a.HelpRequest)
                    .ThenInclude(h => h!.Region)
                .OrderByDescending(a => a.AssignedDate)
                .Take(5)
                .Select(a => new ActivityItem
                {
                    Type = "Assign",
                    Title = "Gönüllü atandı",
                    Subtitle = a.Volunteer!.Name + " " + a.Volunteer.Surname + " → " + a.HelpRequest!.Region!.RegionName + ", " + a.HelpRequest.Category.ToString(),
                    When = a.AssignedDate,
                    IconClass = "fas fa-user",
                    ColorClass = "bg-primary"
                })
                .ToListAsync();

            var delivered = await _context.Assignments
                .Where(a => a.Status == AssignmentStatus.TeslimEdildi && a.DeliveryDate.HasValue)
                .Include(a => a.Volunteer)
                .Include(a => a.HelpRequest)
                .OrderByDescending(a => a.DeliveryDate)
                .Take(5)
                .Select(a => new ActivityItem
                {
                    Type = "Deliver",
                    Title = a.Volunteer!.Name + " " + a.Volunteer.Surname + " teslim etti",
                    Subtitle = a.HelpRequest!.Category.ToString(),
                    When = a.DeliveryDate!.Value,
                    IconClass = "fas fa-truck",
                    ColorClass = "bg-success"
                })
                .ToListAsync();

            return newRequests
                .Concat(approved)
                .Concat(rejected)
                .Concat(assigned)
                .Concat(delivered)
                .OrderByDescending(a => a.When)
                .Take(5)
                .ToList();
        }
    }
}