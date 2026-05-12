using AfetYonetim.Data;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Admin;
using AfetYonetim.Extensions;
using AfetYonetim.Services;
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
        private readonly ICurrentUserService _currentUser;

        public HelpRequestController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUser)
        {
            _context = context;
            _userManager = userManager;
            _currentUser = currentUser;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? status, int? page)
        {
            ViewData["Title"] = "Yardım Talepleri";
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.HelpRequests
                .Include(r => r.User)
                .Include(r => r.Region)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<RequestStatus>(status, out var parsedStatus))
            {
                query = query.Where(r => r.Status == parsedStatus);
                ViewBag.CurrentFilter = status;
            }

            query = query.OrderByDescending(r => r.CreatedAt);
            int totalCount = await query.CountAsync();

            // HATA BURADAYDI: Sadece items çekilmişti, ViewModel'in beklediği tipe dönüştürdük.
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
                    UserFullName = r.User!.Name + " " + r.User.Surname, // Düzeltilen Satır Burası!
                    RegionName = r.Region!.RegionName
                })
                .ToListAsync();

            var model = new HelpRequestIndexViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View("~/Views/Admin/HelpRequest/Index.cshtml", model);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            ViewData["Title"] = "Talep Detayı";

            var request = await _context.HelpRequests
                .Include(r => r.User)
                .Include(r => r.Region)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();

            var assignments = await _context.Assignments
                .Include(a => a.Volunteer)
                .Where(a => a.HelpRequestId == id)
                .OrderByDescending(a => a.AssignedDate)
                .ToListAsync();

            var volunteers = await _userManager.GetUsersInRoleAsync("Gonullu");
            var availableVolunteers = volunteers
                .Where(v => v.IsActive && !v.IsDeleted)
                .Select(v => new SelectListItem
                {
                    Value = v.Id,
                    Text = $"{v.Name} {v.Surname} ({v.Region?.RegionName ?? "Bölge Yok"})"
                }).ToList();

            var model = new HelpRequestDetailViewModel
            {
                Request = request,
                Assignments = assignments,
                AvailableVolunteers = availableVolunteers
            };

            return View("~/Views/Admin/HelpRequest/Details.cshtml", model);
        }

        [HttpPost("Approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            var request = await _context.HelpRequests.FindAsync(id);
            if (request == null) return NotFound();

            if (request.Status != RequestStatus.Bekliyor)
            {
                TempData["Error"] = "Sadece 'Bekliyor' durumundaki talepler onaylanabilir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.Status = RequestStatus.Onaylandi;

            // Faz 4: AuditLog
            _context.AddAuditLog(_currentUser, AuditAction.RequestApproved, nameof(HelpRequest), id.ToString(), $"#{id.ToString()[..8]} onaylandı");

            await _context.SaveChangesAsync();
            TempData["Success"] = "Talep başarıyla onaylandı.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost("Reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id)
        {
            var request = await _context.HelpRequests.FindAsync(id);
            if (request == null) return NotFound();

            if (request.Status != RequestStatus.Bekliyor)
            {
                TempData["Error"] = "Sadece 'Bekliyor' durumundaki talepler reddedilebilir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.Status = RequestStatus.Reddedildi;

            // Faz 4: AuditLog
            _context.AddAuditLog(_currentUser, AuditAction.RequestRejected, nameof(HelpRequest), id.ToString(), $"#{id.ToString()[..8]} reddedildi");

            await _context.SaveChangesAsync();
            TempData["Success"] = "Talep reddedildi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost("Assign/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(Guid id, string volunteerId, string? notes)
        {
            var request = await _context.HelpRequests.FindAsync(id);
            if (request == null) return NotFound();

            if (request.Status != RequestStatus.Onaylandi)
            {
                TempData["Error"] = "Sadece 'Onaylandı' durumundaki taleplere gönüllü atanabilir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (string.IsNullOrEmpty(volunteerId))
            {
                TempData["Error"] = "Lütfen bir gönüllü seçiniz.";
                return RedirectToAction(nameof(Details), new { id });
            }

            request.Status = RequestStatus.Atandi;

            var assignment = new Assignment
            {
                HelpRequestId = id,
                VolunteerId = volunteerId,
                AssignedByAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                AssignedDate = DateTime.UtcNow,
                Status = AssignmentStatus.Atandi,
                Notes = notes
            };

            _context.Assignments.Add(assignment);

            var volunteer = await _userManager.FindByIdAsync(volunteerId);
            var volunteerName = volunteer != null ? $"{volunteer.Name} {volunteer.Surname}" : "Bilinmeyen";

            // Faz 4: AuditLog
            _context.AddAuditLog(_currentUser, AuditAction.AssignmentCreated, nameof(Assignment), assignment.Id.ToString(), $"#{id.ToString()[..8]} → {volunteerName}'a atandı");

            await _context.SaveChangesAsync();
            TempData["Success"] = "Gönüllü başarıyla atandı.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}