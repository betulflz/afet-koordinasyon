using AfetYonetim.Models.Enums;
using AfetYonetim.Data;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.ViewModels.Admin;
using AfetYonetim.Extensions;
using AfetYonetim.Services;
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
        private readonly ICurrentUserService _currentUser;

        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUser)
        {
            _context = context;
            _userManager = userManager;
            _currentUser = currentUser;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? role, int? page)
        {
            ViewData["Title"] = "Kullanıcı Yönetimi";
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var query = _userManager.Users.Include(u => u.Region).AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                var userIds = usersInRole.Select(u => u.Id).ToList();
                query = query.Where(u => userIds.Contains(u.Id));
                ViewBag.CurrentRole = role;
            }

            query = query.OrderBy(u => u.Name).ThenBy(u => u.Surname);
            int totalCount = await query.CountAsync();

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var list = new List<UserListItem>();
            foreach (var user in items)
            {
                var roles = await _userManager.GetRolesAsync(user);
                list.Add(new UserListItem
                {
                    Id = user.Id,
                    FullName = $"{user.Name} {user.Surname}",
                    Email = user.Email ?? "",
                    Roles = roles.ToList(),
                    RegionName = user.Region?.RegionName,
                    IsActive = user.IsActive
                });
            }

            var model = new UserIndexViewModel
            {
                Items = list,
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
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            // Faz 4: AuditLog
            _context.AddAuditLog(_currentUser, AuditAction.UserToggledActive, nameof(ApplicationUser), user.Id, $"Kullanıcı {(user.IsActive ? "aktif" : "pasif")} yapıldı: {user.Email}");
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Kullanıcı durumu {(user.IsActive ? "aktif" : "pasif")} olarak güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}