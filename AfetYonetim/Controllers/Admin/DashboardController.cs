using AfetYonetim.Data;
using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Admin;
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
            _context=context;
        }
        
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";

            var model = new DashboardViewModel
            {
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
                    .CountAsync(a => a.Status == AssignmentStatus.Atandi || a.Status == AssignmentStatus.Yolda)
            };

            return View("~/Views/Admin/Dashboard/Index.cshtml", model);
        }
    }
}