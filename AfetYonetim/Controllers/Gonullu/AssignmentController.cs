using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetYonetim.Controllers.Gonullu
{
    [Authorize(Roles = "Gonullu")]
    [Route("Gonullu/[controller]")]
    public class AssignmentController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Görevlerim";
            return View("~/Views/Gonullu/Assignment/Index.cshtml");
        }

        [HttpGet("Details/{id}")]
        public IActionResult Details(Guid id)
        {
            ViewData["Title"] = "Görev Detayı";
            return View("~/Views/Gonullu/Assignment/Details.cshtml");
        }
    }
}