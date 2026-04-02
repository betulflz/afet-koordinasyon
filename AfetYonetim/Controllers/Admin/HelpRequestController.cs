using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class HelpRequestController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Yardım Talepleri";
            return View("~/Views/Admin/HelpRequest/Index.cshtml");
        }

        [HttpGet("Details/{id}")]
        public IActionResult Details(Guid id)
        {
            ViewData["Title"] = "Yardım Talebi Detayı";
            return View("~/Views/Admin/HelpRequest/Details.cshtml");
        }
    }
}