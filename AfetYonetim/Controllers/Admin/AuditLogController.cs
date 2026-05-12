using AfetYonetim.Data;
using AfetYonetim.Models.Enums;
using AfetYonetim.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class AuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(
            DateTime? fromDate, DateTime? toDate,
            string? userId, AuditAction? action,
            int? page)
        {
            ViewData["Title"] = "Audit Log";

            int pageSize = 20;
            int pageNumber = page ?? 1;

            var query = _context.AuditLogs.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(a => a.Timestamp >= fromDate.Value);
            if (toDate.HasValue)
                query = query.Where(a => a.Timestamp <= toDate.Value.AddDays(1));
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(a => a.UserId == userId);
            if (action.HasValue)
                query = query.Where(a => a.Action == action.Value);

            query = query.OrderByDescending(a => a.Timestamp);

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditLogListItem
                {
                    Id = a.Id,
                    UserFullName = a.UserFullNameSnapshot ?? a.UserId,
                    Action = a.Action,
                    EntityName = a.EntityName,
                    Description = a.Description,
                    Timestamp = a.Timestamp
                })
                .ToListAsync();

            // Filtre dropdown'ları
            var userList = await _context.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.Name)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Name + " " + u.Surname + " (" + u.Email + ")"
                })
                .ToListAsync();

            var model = new AuditLogIndexViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                FromDate = fromDate,
                ToDate = toDate,
                UserId = userId,
                Action = action,
                UserList = userList
            };

            return View("~/Views/Admin/AuditLog/Index.cshtml", model);
        }
    }
}