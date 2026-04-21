using AfetYonetim.Data;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class HelpRequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HelpRequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(
            RequestStatus? status, HelpCategory? category, UrgencyLevel? urgency,
            string? search, int? page)
        {
            ViewData["Title"] = "Yardım Talepleri";

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.HelpRequests
                .Include(r => r.User)
                .Include(r => r.Region)
                .AsQueryable();

            // Filtreler
            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            if (category.HasValue)
                query = query.Where(r => r.Category == category.Value);

            if (urgency.HasValue)
                query = query.Where(r => r.Urgency == urgency.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(r =>
                    r.Description.Contains(search) ||
                    (r.Location != null && r.Location.Contains(search)) ||
                    r.User!.Name.Contains(search) ||
                    r.User!.Surname.Contains(search));

            query = query.OrderByDescending(r => r.CreatedAt);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new HelpRequestListItem
                {
                    Id = r.Id,
                    UserFullName = r.User!.Name + " " + r.User.Surname,
                    Category = r.Category,
                    Urgency = r.Urgency,
                    Status = r.Status,
                    RegionName = r.Region!.RegionName,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            var model = new HelpRequestIndexViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Status = status,
                Category = category,
                Urgency = urgency,
                Search = search
            };

            return View("~/Views/Admin/HelpRequest/Index.cshtml", model);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            ViewData["Title"] = "Yardım Talebi Detayı";

            var request = await _context.HelpRequests
                .Include(r => r.User)
                .Include(r => r.Region)
                .Include(r => r.Assignments)
                    .ThenInclude(a => a.Volunteer)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
                return NotFound();

            // Gönüllü atama modali için aktif gönüllü listesi
            var volunteers = await _userManager.GetUsersInRoleAsync("Gonullu");
            var activeVolunteers = volunteers
                .Where(v => v.IsActive && !v.IsDeleted)
                .Select(v => new SelectListItem
                {
                    Value = v.Id,
                    Text = v.FullName
                })
                .ToList();

            var model = new HelpRequestDetailViewModel
            {
                Request = request,
                Assignments = request.Assignments.OrderByDescending(a => a.AssignedDate).ToList(),
                AvailableVolunteers = activeVolunteers
            };

            return View("~/Views/Admin/HelpRequest/Details.cshtml", model);
        }

        [HttpPost("Approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            var request = await _context.HelpRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            if (request.Status != RequestStatus.Bekliyor)
            {
                TempData["Error"] = "Sadece 'Bekliyor' durumundaki talepler onaylanabilir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.Status = RequestStatus.Onaylandi;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Talep başarıyla onaylandı.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost("Reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id)
        {
            var request = await _context.HelpRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            if (request.Status != RequestStatus.Bekliyor)
            {
                TempData["Error"] = "Sadece 'Bekliyor' durumundaki talepler reddedilebilir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.Status = RequestStatus.Reddedildi;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Talep reddedildi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost("Assign/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(Guid id, string volunteerId, string? notes)
        {
            var request = await _context.HelpRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            if (request.Status != RequestStatus.Onaylandi)
            {
                TempData["Error"] = "Sadece 'Onaylandı' durumundaki taleplere gönüllü atanabilir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (string.IsNullOrWhiteSpace(volunteerId))
            {
                TempData["Error"] = "Gönüllü seçiniz.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var assignment = new Assignment
            {
                HelpRequestId = id,
                VolunteerId = volunteerId,
                AssignedByAdminId = adminId,
                AssignedDate = DateTime.UtcNow,
                Status = AssignmentStatus.Atandi,
                Notes = notes
            };

            _context.Assignments.Add(assignment);
            request.Status = RequestStatus.Atandi;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Gönüllü başarıyla atandı.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}