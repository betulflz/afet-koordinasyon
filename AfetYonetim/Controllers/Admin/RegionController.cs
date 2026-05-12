using AfetYonetim.Models.Enums;
using AfetYonetim.Data;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.ViewModels.Admin;
using AfetYonetim.Extensions;
using AfetYonetim.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class RegionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public RegionController(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? page)
        {
            ViewData["Title"] = "Bölge Yönetimi";
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.Regions.OrderBy(r => r.City).ThenBy(r => r.RegionName);
            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RegionListItem
                {
                    Id = r.Id,
                    RegionName = r.RegionName,
                    City = r.City,
                    District = r.District,
                    RiskLevel = r.RiskLevel,
                    IsActive = r.IsActive
                })
                .ToListAsync();

            var riskPins = await _context.Regions
                .Where(r => r.IsActive)
                .Select(r => new AfetYonetim.Models.ViewModels.Admin.Shared.RegionMapPin
                {
                    Name = r.RegionName + " (" + r.City + ")",
                    Lat = r.Latitude,
                    Lng = r.Longitude,
                    RiskLevel = r.RiskLevel.ToString()
                })
                .ToListAsync();

            var model = new RegionIndexViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                RiskMapPins = riskPins
            };

            return View("~/Views/Admin/Region/Index.cshtml", model);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Yeni Bölge Ekle";
            return View("~/Views/Admin/Region/Create.cshtml");
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Region region)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Yeni Bölge Ekle";
                return View("~/Views/Admin/Region/Create.cshtml", region);
            }

            _context.Regions.Add(region);

            // Faz 4: AuditLog
            _context.AddAuditLog(_currentUser, AuditAction.RegionCreated, nameof(Region), region.Id.ToString(), $"Yeni bölge: {region.RegionName} ({region.City})");

            await _context.SaveChangesAsync();
            TempData["Success"] = "Bölge başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewData["Title"] = "Bölge Düzenle";
            var region = await _context.Regions.FindAsync(id);
            if (region == null) return NotFound();
            return View("~/Views/Admin/Region/Edit.cshtml", region);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Region region)
        {
            if (id != region.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Bölge Düzenle";
                return View("~/Views/Admin/Region/Edit.cshtml", region);
            }

            _context.Regions.Update(region);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Bölge başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null) return NotFound();

            var hasRequests = await _context.HelpRequests.AnyAsync(r => r.RegionId == id);
            if (hasRequests)
            {
                TempData["Error"] = "Bu bölgeye ait yardım talepleri bulunduğu için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            _context.Regions.Remove(region);

            // Faz 4: AuditLog
            _context.AddAuditLog(_currentUser, AuditAction.RegionDeleted, nameof(Region), region.Id.ToString(), $"Bölge silindi: {region.RegionName}");

            await _context.SaveChangesAsync();
            TempData["Success"] = "Bölge başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}