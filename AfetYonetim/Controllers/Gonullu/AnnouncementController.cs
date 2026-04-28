using AfetYonetim.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AfetYonetim.Controllers.Gonullu
{
    [Authorize(Roles = "Gonullu")]
    [Route("Gonullu/[controller]")]
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

            var announcements = await _context.Announcements
                .Include(a => a.CreatedByAdmin)
                .OrderByDescending(a => a.IsUrgent)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View("~/Views/Gonullu/Announcement/Index.cshtml", announcements);
        }
    }
}