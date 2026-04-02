using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetYonetim.Controllers.Afetzede
{
    [Authorize(Roles = "Afetzede")]
    [Route("Afetzede/[controller]")]
    public class ProfileController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Profilim";
            return View("~/Views/Afetzede/Profile/Index.cshtml");
        }
    }
}