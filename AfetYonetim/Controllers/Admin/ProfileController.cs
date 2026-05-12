using AfetYonetim.Models.Entities;
using AfetYonetim.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Profilim";

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var model = new AdminProfileViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email ?? string.Empty
            };

            return View("~/Views/Admin/Profile/Index.cshtml", model);
        }

        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AdminProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Title"] = "Profilim";
                return View("~/Views/Admin/Profile/Index.cshtml", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.PhoneNumber = model.PhoneNumber;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Profiliniz başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}