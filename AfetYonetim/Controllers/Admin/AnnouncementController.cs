using AfetYonetim.Data;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? page)
        {
            ViewData["Title"] = "Duyurular";

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.Announcements
                .Include(a => a.CreatedByAdmin)
                .OrderByDescending(a => a.CreatedAt);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AnnouncementListItem
                {
                    Id = a.Id,
                    Title = a.Title,
                    IsUrgent = a.IsUrgent,
                    ExpiresAt = a.ExpiresAt,
                    CreatedAt = a.CreatedAt,
                    AdminFullName = a.CreatedByAdmin!.Name + " " + a.CreatedByAdmin.Surname
                })
                .ToListAsync();

            var model = new AnnouncementIndexViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View("~/Views/Admin/Announcement/Index.cshtml", model);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Yeni Duyuru";
            return View("~/Views/Admin/Announcement/Create.cshtml", new AnnouncementFormViewModel());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnouncementFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Yeni Duyuru";
                return View("~/Views/Admin/Announcement/Create.cshtml", model);
            }

            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var announcement = new Announcement
            {
                CreatedByAdminId = adminId,
                Title = model.Title,
                Content = model.Content,
                IsUrgent = model.IsUrgent,
                ExpiresAt = model.ExpiresAt
            };

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Duyuru başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewData["Title"] = "Duyuru Düzenle";

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return NotFound();

            var model = new AnnouncementFormViewModel
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content,
                IsUrgent = announcement.IsUrgent,
                ExpiresAt = announcement.ExpiresAt
            };

            return View("~/Views/Admin/Announcement/Edit.cshtml", model);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, AnnouncementFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Duyuru Düzenle";
                return View("~/Views/Admin/Announcement/Edit.cshtml", model);
            }

            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return NotFound();

            announcement.Title = model.Title;
            announcement.Content = model.Content;
            announcement.IsUrgent = model.IsUrgent;
            announcement.ExpiresAt = model.ExpiresAt;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Duyuru başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            ViewData["Title"] = "Duyuru Detayı";

            var announcement = await _context.Announcements
                .Include(a => a.CreatedByAdmin)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (announcement == null)
                return NotFound();

            return View("~/Views/Admin/Announcement/Details.cshtml", announcement);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
                return NotFound();

            _context.Announcements.Remove(announcement); // SaveChanges override soft-delete yapar
            await _context.SaveChangesAsync();

            TempData["Success"] = "Duyuru silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}