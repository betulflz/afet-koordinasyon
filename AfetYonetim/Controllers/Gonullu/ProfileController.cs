using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetYonetim.Controllers.Gonullu
{
    [Authorize(Roles = "Gonullu")]
    [Route("Gonullu/[controller]")]
    public class ProfileController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Profilim";
            return View("~/Views/Gonullu/Profile/Index.cshtml");
        }
    }
}