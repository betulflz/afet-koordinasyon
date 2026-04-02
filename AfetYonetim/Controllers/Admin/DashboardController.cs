using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class DashboardController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            return View("~/Views/Admin/Dashboard/Index.cshtml");
        }
    }
}