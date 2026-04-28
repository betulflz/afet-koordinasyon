using AfetYonetim.Data;
using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Gonullu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AfetYonetim.Controllers.Gonullu
{
    [Authorize(Roles = "Gonullu")]
    [Route("Gonullu/[controller]")]
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? page)
        {
            ViewData["Title"] = "Görevlerim";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.Assignments
                .Include(a => a.HelpRequest)
                .Where(a => a.VolunteerId == userId)
                .OrderByDescending(a => a.AssignedDate);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AssignmentListItem
                {
                    Id = a.Id,
                    RequestCategory = a.HelpRequest!.Category,
                    RequestDescription = a.HelpRequest.Description.Length > 80
                        ? a.HelpRequest.Description.Substring(0, 80) + "..."
                        : a.HelpRequest.Description,
                    RequestLocation = a.HelpRequest.Location,
                    Status = a.Status,
                    AssignedDate = a.AssignedDate
                })
                .ToListAsync();

            var model = new AssignmentListViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View("~/Views/Gonullu/Assignment/Index.cshtml", model);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            ViewData["Title"] = "Görev Detayı";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var assignment = await _context.Assignments
                .Include(a => a.HelpRequest)
                    .ThenInclude(h => h!.Region)
                .Include(a => a.HelpRequest)
                    .ThenInclude(h => h!.User)
                .FirstOrDefaultAsync(a => a.Id == id && a.VolunteerId == userId);

            if (assignment == null)
                return NotFound();

            var model = new AssignmentDetailViewModel
            {
                Assignment = assignment,
                RequesterFullName = assignment.HelpRequest?.User?.FullName ?? "Bilinmiyor",
                RequesterPhone = assignment.HelpRequest?.User?.PhoneNumber
            };

            return View("~/Views/Gonullu/Assignment/Details.cshtml", model);
        }

        [HttpPost("Accept/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var assignment = await _context.Assignments
                .Include(a => a.HelpRequest)
                .FirstOrDefaultAsync(a => a.Id == id && a.VolunteerId == userId);

            if (assignment == null)
                return NotFound();

            if (assignment.Status != AssignmentStatus.Atandi)
            {
                TempData["Error"] = "Sadece 'Atandı' durumundaki görevler kabul edilebilir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            assignment.Status = AssignmentStatus.Yolda;
            assignment.HelpRequest!.Status = RequestStatus.Yolda;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Görevi kabul ettiniz. Yola çıktınız!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost("Complete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid id, string? notes)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var assignment = await _context.Assignments
                .Include(a => a.HelpRequest)
                .FirstOrDefaultAsync(a => a.Id == id && a.VolunteerId == userId);

            if (assignment == null)
                return NotFound();

            if (assignment.Status != AssignmentStatus.Yolda)
            {
                TempData["Error"] = "Sadece 'Yolda' durumundaki görevler teslim edilebilir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            assignment.Status = AssignmentStatus.TeslimEdildi;
            assignment.DeliveryDate = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(notes))
                assignment.Notes = notes;

            assignment.HelpRequest!.Status = RequestStatus.TeslimEdildi;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Görev başarıyla teslim edildi!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost("Reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var assignment = await _context.Assignments
                .Include(a => a.HelpRequest)
                .FirstOrDefaultAsync(a => a.Id == id && a.VolunteerId == userId);

            if (assignment == null)
                return NotFound();

            if (assignment.Status != AssignmentStatus.Atandi)
            {
                TempData["Error"] = "Sadece 'Atandı' durumundaki görevler reddedilebilir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            assignment.Status = AssignmentStatus.Iptal;
            assignment.HelpRequest!.Status = RequestStatus.Onaylandi; // Yeniden atanabilir
            await _context.SaveChangesAsync();

            TempData["Success"] = "Görev reddedildi. Talep yeniden atanmak üzere listeye döndü.";
            return RedirectToAction(nameof(Index));
        }
    }
}