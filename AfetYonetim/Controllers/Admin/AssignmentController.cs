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
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(AssignmentStatus? status, int? page)
        {
            ViewData["Title"] = "Görevlendirmeler";

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _context.Assignments
                .Include(a => a.Volunteer)
                .Include(a => a.HelpRequest)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);

            query = query.OrderByDescending(a => a.AssignedDate);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AssignmentListItem
                {
                    Id = a.Id,
                    VolunteerFullName = a.Volunteer!.Name + " " + a.Volunteer.Surname,
                    RequestCategory = a.HelpRequest!.Category,
                    RequestDescription = a.HelpRequest.Description.Length > 80
                        ? a.HelpRequest.Description.Substring(0, 80) + "..."
                        : a.HelpRequest.Description,
                    Status = a.Status,
                    AssignedDate = a.AssignedDate,
                    DeliveryDate = a.DeliveryDate
                })
                .ToListAsync();

            var model = new AssignmentIndexViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Status = status
            };

            return View("~/Views/Admin/Assignment/Index.cshtml", model);
        }
    }
}