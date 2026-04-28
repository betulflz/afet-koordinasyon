using AfetYonetim.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AfetYonetim.Controllers.Afetzede
{
    [Authorize(Roles = "Afetzede")]
    [Route("Afetzede/[controller]")]
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Duyurular";

            var now = DateTime.UtcNow;

            var announcements = await _context.Announcements
                .Include(a => a.CreatedByAdmin)
                .OrderByDescending(a => a.IsUrgent)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View("~/Views/Afetzede/Announcement/Index.cshtml", announcements);
        }
    }
}