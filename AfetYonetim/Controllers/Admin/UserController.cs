using AfetYonetim.Data;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int? page)
        {
            ViewData["Title"] = "Kullanıcılar";

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                        from ur in userRoles.DefaultIfEmpty()
                        join r in _context.Roles on ur.RoleId equals r.Id into roles
                        from r in roles.DefaultIfEmpty()
                        where !u.IsDeleted
                        orderby u.Name, u.Surname
                        select new UserListItem
                        {
                            Id = u.Id,
                            FullName = u.Name + " " + u.Surname,
                            Email = u.Email ?? string.Empty,
                            PhoneNumber = u.PhoneNumber,
                            Role = r.Name ?? "Bilinmiyor",
                            IsActive = u.IsActive,
                            CreatedAt = u.CreatedAt
                        };

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new UserIndexViewModel
            {
                Items = items,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return View("~/Views/Admin/User/Index.cshtml", model);
        }

        [HttpPost("ToggleActive/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            var status = user.IsActive ? "aktif" : "pasif";
            TempData["Success"] = $"Kullanıcı {status} yapıldı.";
            return RedirectToAction(nameof(Index));
        }
    }
}