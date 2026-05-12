using AfetYonetim.Data;
using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AfetYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Ana Sayfa";

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
                .Take(50)
                .Select(r => new
                {
                    Lat = r.Latitude == 0 ? r.Region!.Latitude : r.Latitude,
                    Lng = r.Longitude == 0 ? r.Region!.Longitude : r.Longitude,
                    Urgency = r.Urgency
                })
                .ToListAsync();

            var model = new PublicLandingViewModel
            {
                TotalRequests = await _context.HelpRequests.CountAsync(),
                CompletedRequests = await _context.HelpRequests
                    .CountAsync(r => r.Status == RequestStatus.TeslimEdildi),
                ActiveAssignments = await _context.Assignments
                    .CountAsync(a => a.Status == AssignmentStatus.Atandi
                                  || a.Status == AssignmentStatus.Yolda),
                ActiveVolunteers = await _context.Users
                    .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                    .Join(_context.Roles, x => x.ur.RoleId, r => r.Id, (x, r) => new { x.u, RoleName = r.Name })
                    .CountAsync(x => x.RoleName == "Gonullu" && x.u.IsActive && !x.u.IsDeleted),

                MapPoints = raw.Select(r => new PublicMapPoint
                {
                    Lat = r.Lat,
                    Lng = r.Lng,
                    Urgency = r.Urgency.ToString()
                }).ToList(),

                RecentAnnouncements = await _context.Announcements
                    .OrderByDescending(a => a.IsUrgent)
                    .ThenByDescending(a => a.CreatedAt)
                    .Take(3)
                    .Select(a => new PublicAnnouncement
                    {
                        Title = a.Title,
                        Content = a.Content.Length > 200
                            ? a.Content.Substring(0, 200) + "..."
                            : a.Content,
                        IsUrgent = a.IsUrgent,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}