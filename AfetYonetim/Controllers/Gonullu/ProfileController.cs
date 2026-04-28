using AfetYonetim.Data;
using AfetYonetim.Models.Entities;
using AfetYonetim.Models.ViewModels.Gonullu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AfetYonetim.Controllers.Gonullu
{
    [Authorize(Roles = "Gonullu")]
    [Route("Gonullu/[controller]")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Profilim";

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var model = new ProfileViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                RegionId = user.RegionId,
                Email = user.Email ?? string.Empty,
                RegionList = await GetRegionSelectList()
            };

            return View("~/Views/Gonullu/Profile/Index.cshtml", model);
        }

        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Profilim";
                model.RegionList = await GetRegionSelectList();
                return View("~/Views/Gonullu/Profile/Index.cshtml", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.PhoneNumber = model.PhoneNumber;
            user.RegionId = model.RegionId;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profiliniz başarıyla güncellendi.";
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
    }
}