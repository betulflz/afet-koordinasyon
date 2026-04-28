using AfetYonetim.Data;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Admin;
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

        public RegionController(ApplicationDbContext context)
        {
            _context = context;
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

            var model = new RegionIndexViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View("~/Views/Admin/Region/Index.cshtml", model);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Yeni Bölge";
            return View("~/Views/Admin/Region/Create.cshtml", new RegionFormViewModel());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Yeni Bölge";
                return View("~/Views/Admin/Region/Create.cshtml", model);
            }

            var region = new Region
            {
                RegionName = model.RegionName,
                City = model.City,
                District = model.District,
                RiskLevel = model.RiskLevel,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                IsActive = model.IsActive
            };

            _context.Regions.Add(region);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Bölge başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            ViewData["Title"] = "Bölge Düzenle";

            var region = await _context.Regions.FindAsync(id);
            if (region == null)
                return NotFound();

            var model = new RegionFormViewModel
            {
                Id = region.Id,
                RegionName = region.RegionName,
                City = region.City,
                District = region.District,
                RiskLevel = region.RiskLevel,
                Latitude = region.Latitude,
                Longitude = region.Longitude,
                IsActive = region.IsActive
            };

            return View("~/Views/Admin/Region/Edit.cshtml", model);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, RegionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Bölge Düzenle";
                return View("~/Views/Admin/Region/Edit.cshtml", model);
            }

            var region = await _context.Regions.FindAsync(id);
            if (region == null)
                return NotFound();

            region.RegionName = model.RegionName;
            region.City = model.City;
            region.District = model.District;
            region.RiskLevel = model.RiskLevel;
            region.Latitude = model.Latitude;
            region.Longitude = model.Longitude;
            region.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Bölge başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null)
                return NotFound();

            _context.Regions.Remove(region); // Soft delete (SaveChanges override)
            await _context.SaveChangesAsync();

            TempData["Success"] = "Bölge silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}