using AfetYonetim.Data;
using AfetYonetim.Extensions;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Afetzede;
using AfetYonetim.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace AfetYonetim.Controllers.Afetzede
{
    [Authorize(Roles = "Afetzede")]
    [Route("Afetzede/[controller]")]
    public class HelpRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public HelpRequestController(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? page)
        {
            ViewData["Title"] = "Taleplerim";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.HelpRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new HelpRequestListItem
                {
                    Id = r.Id,
                    Category = r.Category,
                    Urgency = r.Urgency,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    Location = r.Location
                })
                .ToListAsync();

            var model = new HelpRequestListViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View("~/Views/Afetzede/HelpRequest/Index.cshtml", model);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Yeni Talep Oluştur";

            var model = new HelpRequestCreateViewModel
            {
                RegionList = await GetRegionSelectList()
            };

            ViewBag.RegionCoords = await BuildRegionCoordsJson();

            return View("~/Views/Afetzede/HelpRequest/Create.cshtml", model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HelpRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Yeni Talep Oluştur";
                model.RegionList = await GetRegionSelectList();
                ViewBag.RegionCoords = await BuildRegionCoordsJson();
                return View("~/Views/Afetzede/HelpRequest/Create.cshtml", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Lat/Long fallback (Faz 3)
            double finalLat, finalLng;
            string regionName = "";
            if (model.Latitude.HasValue && model.Longitude.HasValue)
            {
                finalLat = model.Latitude.Value;
                finalLng = model.Longitude.Value;
                var region = await _context.Regions.FindAsync(model.RegionId);
                regionName = region?.RegionName ?? "";
            }
            else
            {
                var region = await _context.Regions.FindAsync(model.RegionId);
                if (region == null)
                {
                    ModelState.AddModelError(nameof(model.RegionId), "Geçersiz bölge seçimi.");
                    model.RegionList = await GetRegionSelectList();
                    ViewBag.RegionCoords = await BuildRegionCoordsJson();
                    return View("~/Views/Afetzede/HelpRequest/Create.cshtml", model);
                }
                finalLat = region.Latitude;
                finalLng = region.Longitude;
                regionName = region.RegionName;
            }

            var request = new HelpRequest
            {
                UserId = userId,
                Category = model.Category,
                Description = model.Description,
                Location = model.Location,
                RegionId = model.RegionId,
                Urgency = model.Urgency,
                Latitude = finalLat,
                Longitude = finalLng,
                Status = RequestStatus.Bekliyor
            };

            _context.HelpRequests.Add(request);

            // Faz 4: AuditLog (Yeni Talep açıldı logu)
            _context.AddAuditLog(_currentUser,
                AuditAction.RequestCreated,
                nameof(HelpRequest),
                request.Id.ToString(),
                $"Yeni talep: {model.Category} · {regionName}");

            await _context.SaveChangesAsync();

            TempData["Success"] = "Yardım talebiniz başarıyla oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            ViewData["Title"] = "Talep Detayı";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var request = await _context.HelpRequests
                .Include(r => r.Region)
                .Include(r => r.Assignments)
                    .ThenInclude(a => a.Volunteer)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (request == null)
                return NotFound();

            return View("~/Views/Afetzede/HelpRequest/Details.cshtml", request);
        }

        // Faz 4: YENİ ACTION — Talep İptal
        [HttpPost("Cancel/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var request = await _context.HelpRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (request == null)
                return NotFound();

            if (request.Status != RequestStatus.Bekliyor && request.Status != RequestStatus.Onaylandi)
            {
                TempData["Error"] = "Bu talep artık iptal edilemez (gönüllü atanmış olabilir).";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.Status = RequestStatus.IptalEdildi;

            // Faz 4: AuditLog (Talep iptal edildi logu)
            _context.AddAuditLog(_currentUser,
                AuditAction.RequestCancelled,
                nameof(HelpRequest),
                id.ToString(),
                $"Talep iptal edildi: #{id.ToString()[..8]} ({request.Category})");

            await _context.SaveChangesAsync();

            TempData["Success"] = "Talebiniz iptal edildi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetRegionSelectList()
        {
            return await _context.Regions
                .Where(r => r.IsActive)
                .OrderBy(r => r.RegionName)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.RegionName + " (" + r.City + ")"
                })
                .ToListAsync();
        }

        private async Task<string> BuildRegionCoordsJson()
        {
            var dict = await _context.Regions
                .Where(r => r.IsActive)
                .ToDictionaryAsync(
                    r => r.Id.ToString(),
                    r => new[] { r.Latitude, r.Longitude });
            return JsonSerializer.Serialize(dict, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}