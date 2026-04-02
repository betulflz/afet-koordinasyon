using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AfetYonetim.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[controller]")]
    public class AssignmentController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Görevlendirmeler";
            return View("~/Views/Admin/Assignment/Index.cshtml");
        }

        [HttpGet("Create")]
        public IActionResult Create(Guid? helpRequestId)
        {
            ViewData["Title"] = "Yeni Görevlendirme";
            return View("~/Views/Admin/Assignment/Create.cshtml");
        }
       
    }
}